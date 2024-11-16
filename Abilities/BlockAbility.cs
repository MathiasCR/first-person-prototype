using UnityEngine;

public class BlockAbility : Ability
{
    [SerializeField] protected float m_DamageReduction = 0.6f;

    public float DamageReduction { get => m_DamageReduction; }

    private Health m_OwnerHealth;

    public override void InitAbility(GameObject owner)
    {
        //Debug.Log("BlockAbility InitAbility");
        base.InitAbility(owner);
        m_AbilityAnimationData.AbilityAnimatorParameterName = GetType().Name;
        m_AbilityAnimationData.AbilityAnimatorParameterType = AnimatorControllerParameterType.Bool;
    }

    public override void AbilityStart()
    {
        //Debug.Log("BlockAbility AbilityStart");
        base.AbilityStart();
        m_OwnerHealth = Owner.GetComponent<Health>();
        m_OwnerHealth.IsBlocking = true;
        m_OwnerHealth.DamageReduction = m_DamageReduction;
        m_OwnerHealth.OnHitCallback += m_PlayerAnimator.BlockedHitAnimation;
        m_PlayerAnimator.StartAnimation(m_AbilityAnimationData, true);
    }

    public override void AbilityEnd()
    {
        //Debug.Log("BlockAbility AbilityEnd");
        base.AbilityEnd();
        m_OwnerHealth.OnHitCallback -= m_PlayerAnimator.BlockedHitAnimation;
        m_OwnerHealth.IsBlocking = false;
    }

    public void AbilityRelease()
    {
        m_PlayerAnimator.StartAnimation(m_AbilityAnimationData, false);
        AbilityEnd();
    }
}