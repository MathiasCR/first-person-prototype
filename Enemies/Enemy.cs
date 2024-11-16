using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour, IMovable, IHostile
{
    [SerializeField] protected Health m_Health;
    [SerializeField] protected float m_AggroRange;
    [SerializeField] protected float m_AggroSpeed;
    [SerializeField] protected float m_VisualRange;
    [SerializeField] protected Activity m_Activity;
    [SerializeField] protected float m_FeelingRange;
    [SerializeField] protected float m_RoamingSpeed;
    [SerializeField] protected float m_AttackDamage;
    [SerializeField] protected float m_AbilityDamage;
    [SerializeField] protected float m_SightAngle;
    [SerializeField] protected Animator m_Animator;
    [SerializeField] protected float m_AttackRange;
    [SerializeField] protected float m_AbilityMinRange;
    [SerializeField] protected float m_AbilityMaxRange;
    [SerializeField] protected float m_AttackCooldown;
    [SerializeField] protected AudioClip m_AttackSound;
    [SerializeField] protected float m_AbilityCooldown;
    [SerializeField] protected NavMeshAgent m_NavMeshAgent;
    [SerializeField] protected LayerMask m_TargetLayerMask;
    [SerializeField] protected ParticleSystem m_BuryEnemyParticleSystem;
    [SerializeField] protected List<Renderer> m_MeshesWithRotatedMaterial;

    public bool Moving { get => m_Moving; }
    public bool CanAct { get => m_CanAct; }
    public Health Target { get => m_Target; }
    public Activity Activity { get => m_Activity; }
    public float SightAngle { get => m_SightAngle; }
    public float AggroRange { get => m_AggroRange; }
    public float AggroSpeed { get => m_AggroSpeed; }
    public float VisualRange { get => m_VisualRange; }
    public float RoamingSpeed { get => m_RoamingSpeed; }
    public float FeelingRange { get => m_FeelingRange; }
    public Animator E_Animator { get => m_Animator; }
    public bool IsAttacking { get => m_IsAttacking; }
    public Vector3 SpawnPoint { get => m_SpawnPoint; }
    public float AttackRange { get => m_AttackRange; }
    public float AttackDamage { get => m_AttackDamage; }
    public float AbilityDamage { get => m_AbilityDamage; }
    public bool IsUsingAbility { get => m_IsUsingAbility; }
    public float AbilityMinRange { get => m_AbilityMinRange; }
    public float AbilityMaxRange { get => m_AbilityMaxRange; }
    public float AttackCooldown { get => m_AttackCooldown; }
    public float AbilityCooldown { get => m_AbilityCooldown; }
    public NavMeshAgent E_NavMeshAgent { get => m_NavMeshAgent; }
    public LayerMask TargetLayerMask { get => m_TargetLayerMask; }

    public StateMachine CurrentState;

    protected bool m_IsDead;
    protected bool m_Moving;
    protected Health m_Target;
    protected bool m_IsAttacking;
    protected bool m_IsUsingAbility;
    protected bool m_CanAct;
    protected Vector3 m_SpawnPoint;
    protected AudioSource m_AudioSource;
    protected AnimationState m_PreviousAnimationState;

    protected virtual void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();

        foreach (Renderer mesh in m_MeshesWithRotatedMaterial)
        {
            mesh.material.mainTextureOffset = new Vector2(Random.Range(1.01f, 1.99f), Random.Range(1.01f, 1.99f));
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_Target = null;
        m_IsDead = false;
        m_Moving = false;
        m_CanAct = true;
        m_IsAttacking = false;
        m_IsUsingAbility = false;
        m_SpawnPoint = transform.position;
        m_NavMeshAgent.stoppingDistance = m_AttackRange;
        SetState(new RoamingState(this));
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_IsDead) return;

        m_Moving = m_NavMeshAgent.velocity != Vector3.zero;
        m_Animator.SetBool("Moving", m_Moving);

        CurrentState.UpdateState();
    }

    public void SetState(StateMachine state)
    {
        if (CurrentState != null)
        {
            CurrentState.ExitState();
        }

        CurrentState = state;
        CurrentState.EnterState();
    }

    public void MoveTo(Vector3 destination)
    {
        m_NavMeshAgent.isStopped = false;
        m_NavMeshAgent.SetDestination(destination);
    }

    public void Stop()
    {
        m_NavMeshAgent.isStopped = true;
    }

    public bool IsTargetInLineOfSight(GameObject target)
    {
        Vector3 targetDir = target.transform.position - transform.position;
        Ray ray = new Ray(transform.position, targetDir);
        if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit hitInf, m_AggroRange) && hitInf.collider.name == target.name)
        {
            float angle = Vector3.Angle(targetDir, transform.forward);

            if (angle < m_SightAngle)
            {
                return true;
            }
        }

        return false;
    }

    public void SetTarget(Health target)
    {
        m_Target = target;
    }

    protected void FaceTarget()
    {
        Vector3 lookPos = m_Target.transform.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.1f);
    }

    public virtual void Attack()
    {
        m_Animator.SetTrigger("Attack");
        m_IsAttacking = true;
        m_CanAct = false;
    }

    public abstract void Ability();

    public void AttackEnded()
    {
        m_CanAct = true;
        StartCoroutine(AttackCooldownCoroutine());
    }

    public abstract void AbilityEnded();

    private IEnumerator AttackCooldownCoroutine()
    {
        float delay = 0f;

        while (delay < m_AttackCooldown)
        {
            delay += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        m_IsAttacking = false;
    }

    public virtual void OnHurt(GameObject origin)
    {
        //Layer 1 is the Hit Layer
        m_Animator.SetLayerWeight(1, 1);
        m_Animator.SetTrigger("Hurt");

        if (CurrentState is not AggressiveState && origin.TryGetComponent(out Health health))
        {
            Debug.Log("Hurt by : " + origin.name);
            SetState(new AggressiveState(this, health));
        }
    }

    public void OnHurtEnded()
    {
        m_Animator.SetLayerWeight(1, 0);
    }

    public virtual void OnDied()
    {
        Stop();
        m_IsDead = true;
        m_NavMeshAgent.enabled = false;
        m_Animator.SetTrigger("Die");
    }

    public void OnDiedEnded()
    {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(5f);

        float duration = 0f;
        Vector3 position = new Vector3(transform.position.x, 0f, transform.position.z);
        ParticleSystem ps = Instantiate(m_BuryEnemyParticleSystem, position, Quaternion.identity);
        ps.Play();

        while (duration < 2f)
        {
            gameObject.transform.localPosition += Vector3.down * Time.deltaTime;
            duration += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        Destroy(gameObject);
    }

    public void ModifySpeed(float amout)
    {
        m_AggroSpeed = amout * (10 / m_AggroSpeed);
    }
}
