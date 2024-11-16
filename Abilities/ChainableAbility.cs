using System.Collections.Generic;
using UnityEngine;

public abstract class ChainableAbility : Ability, IChainable
{
    [SerializeField] protected List<AbilityChainStat> m_AbilityChainStats;

    public int CurrentChain { get => m_CurrentChain; }
    public List<AbilityChainStat> AbilityChainStats { get => m_AbilityChainStats; }

    protected int m_CurrentChain = 0;

    public override void InitAbility(GameObject owner)
    {
        //Debug.Log("ChainableAbility InitAbility");
        base.InitAbility(owner);
        m_CurrentChain = 0;
    }

    public override void AbilityStart()
    {
        Debug.Log("ChainableAbility AbilityStart");
        RemoveResources();
        if (m_CurrentChain == 0) m_PlayerAnimator.OnAbilityEnd += AbilityEnd;
        m_PlayerAnimator.OnPlayAbilitySound += PlayAbilitySound;
        m_PlayerAnimator.OnAbilityNextChain += AbilityNextChain;
        m_PlayerAnimator.StartAnimation(m_AbilityAnimationData, true);
    }

    public override void PlayAbilitySound()
    {
        Debug.Log("ChainableAbility PlayAbilitySound : " + m_CurrentChain);
        m_PlayerAnimator.OnPlayAbilitySound -= PlayAbilitySound;
        m_SFXManager.PlayAudioOnce(AbilityAudioSounds[m_CurrentChain]);
    }

    public virtual void AbilityNextChain()
    {
        Debug.Log("ChainableAbility AbilityNextChain");
        m_PlayerAnimator.OnAbilityNextChain -= AbilityNextChain;
        GameManager.Instance.PlayerManager.PlayerAction.IsActing = false;
        m_CurrentChain++;
    }

    public override void AbilityEnd()
    {
        Debug.Log("ChainableAbility AbilityEnd");
        base.AbilityEnd();
        m_PlayerAnimator.OnAbilityNextChain -= AbilityNextChain;
        m_CurrentChain = 0;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (m_PlayerAnimator != null) m_PlayerAnimator.OnAbilityNextChain -= AbilityNextChain;
    }
}
