using UnityEngine;

public class HealPotionAbility : PotionAbility
{
    [SerializeField] protected float m_HealAmount;

    public float HealAmount { get => m_HealAmount; }

    public override void AbilityStart()
    {
        Debug.Log("HealPotionAbility AbilityStart");
        base.AbilityStart();
        m_PlayerAnimator.StartAnimation(m_AbilityAnimationData, true);
    }

    public override void AbilityEnd()
    {
        Debug.Log("HealPotionAbility AbilityEnd");
        Owner.GetComponent<Health>().Add(HealAmount, gameObject);
    }
}
