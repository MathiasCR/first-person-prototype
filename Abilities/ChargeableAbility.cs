using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChargeableAbility : Ability, IChargeable
{
    [SerializeField] protected bool m_ChargeableSize;
    [SerializeField] protected bool m_ChargeableRange;
    [SerializeField] protected bool m_ChargeableDamage;
    [SerializeField] protected bool m_NeedCompletion;
    [SerializeField] protected bool m_ReleaseOnCompletion;
    [SerializeField] protected float m_ChargeTime;
    [SerializeField] protected AudioClip m_ChargeAudioClip;
    [SerializeField] protected List<AbilityResourceType> m_DrainedResourceTypes;

    public AudioClip ChargeAudioClip { get => m_ChargeAudioClip; }
    public bool IsHolding { get => m_IsHolding; }
    public bool NeedCompletion { get => m_NeedCompletion; }
    public bool ChargeableSize { get => m_ChargeableSize; }
    public bool ChargeableRange { get => m_ChargeableRange; }
    public bool ChargeableDamage { get => m_ChargeableDamage; }
    public bool ReleaseOnCompletion { get => m_ReleaseOnCompletion; }
    public bool ChargeCompleted { get => m_ChargeCompleted; }
    public float ChargeTime { get => m_ChargeTime; }
    public List<AbilityResourceType> DrainedResourceTypes { get => m_DrainedResourceTypes; }

    protected bool m_IsHolding;
    protected bool m_ChargeCompleted;
    protected float m_CurrentChargeTime;
    protected Animator m_WeaponAnimator;
    protected List<OwnerResource> m_OwnerDrainedResources = new List<OwnerResource>();
    protected AbilityAnimationData m_IdleAbilityAnimationData = new AbilityAnimationData();

    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_IsHolding && m_ChargeCompleted && m_ReleaseOnCompletion)
        {
            AbilityRelease();
        }

        if (m_IsHolding && !CanHoldAbility())
        {
            if (m_NeedCompletion)
            {
                AbilityEnd();
                return;
            }

            AbilityRelease();
        }

        if (m_IsHolding)
        {
            DrainResources();
        }
    }

    public override void InitAbility(GameObject owner)
    {
        //Debug.Log("ChargeableAbility InitAbility");
        base.InitAbility(owner);
        m_IdleAbilityAnimationData.AbilityAnimatorParameterName = "Idle";
        m_IdleAbilityAnimationData.AbilityAnimatorParameterType = AnimatorControllerParameterType.Trigger;

        foreach (AbilityResourceType resource in m_DrainedResourceTypes)
        {
            OwnerResource ownerResource = new OwnerResource();
            ownerResource.Cost = resource.Cost;

            switch (resource.ResourceType)
            {
                case ResourceType.Health:
                    ownerResource.Resource = m_Owner.GetComponent<Health>();
                    break;
                case ResourceType.Mana:
                    ownerResource.Resource = m_Owner.GetComponent<Mana>();
                    break;
                case ResourceType.Stamina:
                    ownerResource.Resource = m_Owner.GetComponent<Stamina>();
                    break;
            }

            m_OwnerDrainedResources.Add(ownerResource);
        }

        if (m_Owner.TryGetComponent(out PlayerManager playerManager) && playerManager.PlayerInventory.ItemInHands.TryGetComponent(out Animator weaponAnimator))
        {
            m_WeaponAnimator = weaponAnimator;
        }
    }

    public override void AbilityStart()
    {
        //Debug.Log("ChargeableAbility AbilityStart");
        base.AbilityStart();
        m_PlayerAnimator.OnAbilityCharge += AbilityCharge;
        m_PlayerAnimator.OnAbilityChargeAudio += AbilityChargePlayAudio;
    }

    public virtual void AbilityCharge()
    {
        //Debug.Log("ChargeableAbility AbilityCharge");
        m_PlayerAnimator.OnAbilityCharge -= AbilityCharge;

        m_WeaponAnimator.SetBool("Charge", true);
        m_IsHolding = true;
        m_CurrentChargeTime = 0f;
        StartCoroutine(ChargeTimer());
    }

    public virtual void AbilityChargePlayAudio()
    {
        //Debug.Log("ChargeableAbility AbilityChargePlayAudio");
        m_PlayerAnimator.OnAbilityChargeAudio -= AbilityChargePlayAudio;

        m_SFXManager.PlayAudioOnce(m_ChargeAudioClip);
    }

    public virtual void AbilityRelease()
    {
        //Debug.Log("ChargeableAbility AbilityRelease");
        m_PlayerAnimator.OnAbilityCharge -= AbilityCharge;
        m_PlayerAnimator.OnAbilityChargeAudio -= AbilityChargePlayAudio;

        m_IsHolding = false;
        if (m_NeedCompletion && !m_ChargeCompleted)
        {
            m_PlayerAnimator.StartAnimation(m_IdleAbilityAnimationData, true);
        }
        else
        {
            m_WeaponAnimator.SetBool("Charge", false);
            m_PlayerAnimator.StartAnimation(m_AbilityAnimationData, false);
        }
    }

    public override void AbilityEnd()
    {
        //Debug.Log("ChargeableAbility AbilityEnd");
        base.AbilityEnd();
        m_PlayerAnimator.OnAbilityCharge -= AbilityCharge;
        m_PlayerAnimator.OnAbilityChargeAudio -= AbilityChargePlayAudio;

        m_IsHolding = false;
        m_ChargeCompleted = false;
        StopCoroutine(ChargeTimer());
    }

    public void DrainResources()
    {
        foreach (OwnerResource resource in m_OwnerDrainedResources)
        {
            if (resource.Resource != null && resource.Resource.CanUse(resource.Cost))
            {
                resource.Resource.Remove(resource.Cost * Time.deltaTime, m_Owner);
            }
        }
    }

    private bool CanHoldAbility()
    {
        foreach (OwnerResource resource in m_OwnerDrainedResources)
        {
            if (resource.Resource?.Current <= resource.Cost)
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator ChargeTimer()
    {
        while (m_CurrentChargeTime < m_ChargeTime)
        {
            m_CurrentChargeTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        m_ChargeCompleted = true;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

    }
}
