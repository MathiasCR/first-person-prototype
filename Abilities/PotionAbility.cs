using System;
using UnityEngine;

public abstract class PotionAbility : Ability
{
    public event Action OnPotionUsed;

    protected void Start()
    {
        m_AbilityAnimationData.AbilityAnimatorParameterName = GetType().BaseType.Name;
        m_AbilityAnimationData.AbilityAnimatorParameterType = AnimatorControllerParameterType.Trigger;
    }

    public override void AbilityEnd()
    {
        Debug.Log("AbilityCallback PotionAbility");
        base.AbilityEnd();
        OnPotionUsed?.Invoke();
    }
}
