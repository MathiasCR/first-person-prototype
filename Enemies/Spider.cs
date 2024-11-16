using System.Collections;
using UnityEngine;

public class Spider : MeleeEnemy
{
    [SerializeField] protected float m_JumpSpeed;
    [SerializeField] protected float m_AttachDamage;
    [SerializeField] protected Collider m_JumpCollider;
    [SerializeField] protected Transform m_HeadPosition;
    [SerializeField] protected float m_FreeAttachTreshold;
    [SerializeField] protected AnimationClip m_JumpAnimation;
    [SerializeField] protected ParticleSystem m_WeaveParticleEffect;

    protected bool m_IsJumping;
    protected bool m_IsAttached;
    protected float m_HealthOnAttach;
    protected Vector3 m_StartPosition;
    protected Vector3 m_EndPosition;

    protected override void Start()
    {
        base.Start();
        m_IsJumping = false;
        m_IsAttached = false;
    }

    protected override void Update()
    {
        if (m_IsDead)
        {
            if (m_IsAttached && m_Target.TryGetComponent(out PlayerManager playerManager))
            {
                m_IsAttached = false;
                playerManager.PlayerAnimator.StandUpFromBackFall();
            }
            return;
        }

        if (m_IsJumping)
        {
            // Déplacer l'objet en interpolant entre la position de départ et la cible
            transform.position = Vector3.MoveTowards(transform.position, m_EndPosition, Time.deltaTime * m_JumpSpeed);
        }
        else if (m_IsAttached && (m_HealthOnAttach - m_Health.Current) > m_FreeAttachTreshold)
        {
            m_IsAttached = false;
            m_Animator.SetBool("AttachedAttack", false);
            if (m_Target.TryGetComponent(out PlayerManager playerManager))
            {
                playerManager.PlayerAnimator.StandUpFromBackFall();
            }
            AbilityEnded();
        }
        else
        {
            base.Update();
        }
    }

    public override void Ability()
    {
        //FaceTarget();
        m_Animator.SetTrigger("Jump");
        m_IsUsingAbility = true;
        m_CanAct = false;
    }

    public void Jump()
    {
        //FaceTarget();
        if (IsTargetInLineOfSight(m_Target.gameObject))
        {
            m_StartPosition = transform.position;
            m_EndPosition = m_Target.transform.position;
            m_EndPosition.y = transform.position.y;
            m_IsJumping = true;
            float distance = Vector3.Distance(m_StartPosition, m_EndPosition);
            float speedMultiplier = (m_AbilityMaxRange - distance) / m_AbilityMaxRange;
            m_Animator.SetFloat("JumpSpeed", 1 + speedMultiplier);
            transform.LookAt(m_EndPosition);
            m_JumpCollider.enabled = true;
        }
        else
        {
            m_CanAct = true;
            m_IsUsingAbility = false;
            m_Animator.SetTrigger("StopJump");
        }
    }

    public void AttachedAttack()
    {
        m_AudioSource.PlayOneShot(m_AttackSound);
        m_Target.Remove(m_AttachDamage, gameObject);
    }

    public void OnTriggerEnter(Collider collider)
    {
        Debug.Log("CheckIfAttached");
        if (collider.TryGetComponent(out Health targetHealth))
        {
            Debug.Log("Attached");
            m_JumpCollider.enabled = false;
            targetHealth.Remove(m_AbilityDamage, gameObject);

            m_IsAttached = true;
            m_IsJumping = false;
            m_HealthOnAttach = m_Health.Current;
            if (m_Target != targetHealth) m_Target = targetHealth;


            if (targetHealth.TryGetComponent(out PlayerManager playerManager))
            {
                playerManager.PlayerAnimator.FallOnBackAndLookAt(m_HeadPosition);
                m_Animator.SetBool("AttachedAttack", true);
            }

            transform.position = targetHealth.transform.position;
            transform.LookAt(targetHealth.transform.position);
        }
    }

    public override void OnHurt(GameObject origin)
    {
        if (m_IsJumping) return;
        base.OnHurt(origin);
    }

    public override void OnDied()
    {
        base.OnDied();
        if (m_IsJumping)
        {
            m_Animator.SetLayerWeight(1, 1);
            m_Animator.SetTrigger("JumpDie");
        }
    }

    public void OnStartWeaving()
    {
        m_WeaveParticleEffect.Play();
    }

    public void OnStopWeaving()
    {
        m_WeaveParticleEffect.Stop();
    }

    public override void AbilityEnded()
    {
        m_CanAct = true;
        m_IsJumping = false;
        m_JumpCollider.enabled = false;
        StartCoroutine(AbilityCooldownCoroutine());
    }

    private IEnumerator AbilityCooldownCoroutine()
    {
        float delay = 0f;

        while (delay < m_AbilityCooldown)
        {
            delay += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        m_IsUsingAbility = false;
    }
}
