using UnityEngine;

public class MeleeChainableAbility : ChainableAbility, IMeleeAbility
{
    public override void InitAbility(GameObject owner)
    {
        //Debug.Log("MeleeChainableAbility InitAbility");
        base.InitAbility(owner);
        m_AbilityAnimationData.AbilityAnimatorParameterName = GetType().Name;
        m_AbilityAnimationData.AbilityAnimatorParameterType = AnimatorControllerParameterType.Trigger;
    }

    public override void AbilityStart()
    {
        base.AbilityStart();
        m_PlayerAnimator.OnCheckAbilityHit += CheckAbilityHit;
    }

    public void CheckAbilityHit()
    {
        m_PlayerAnimator.OnCheckAbilityHit -= CheckAbilityHit;
        Ray ray = Camera.main.ViewportPointToRay(m_Center);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, m_AbilityChainStats[m_CurrentChain].Range, m_AbilityLayerMask)
            && hit.transform.root.TryGetComponent(out Health targetHealth))
        {
            StartCoroutine(SlowDownAnimation(0.1f, 0.1f));
            if (m_Owner.TryGetComponent(out IResourceModifier modifier))
            {
                targetHealth.Remove(m_AbilityChainStats[m_CurrentChain].Amount, modifier.ModifierGO);
                return;
            }

            targetHealth.Remove(m_AbilityChainStats[m_CurrentChain].Amount, null);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (m_PlayerAnimator != null) m_PlayerAnimator.OnCheckAbilityHit -= CheckAbilityHit;
    }
}
