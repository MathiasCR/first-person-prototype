using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Ability : MonoBehaviour, IAbility
{
    [SerializeField] protected string m_AbilityName;
    [TextArea][SerializeField] protected string m_AbilityDescription;
    [SerializeField] protected float m_AbilityAmount;
    [SerializeField] protected float m_AbilityRange;
    [SerializeField] protected float m_AbilitySpeed;
    [SerializeField] protected float m_AbilityCooldown;
    [SerializeField] protected LayerMask m_AbilityLayerMask;
    [SerializeField] protected List<AudioClip> m_AbilityAudioSounds;
    [SerializeField] protected List<AbilityResourceType> m_AbilityResourceTypes;

    public GameObject Owner => m_Owner;
    public string AbilityName { get => m_AbilityName; }
    public string AbilityDescription { get => m_AbilityDescription; }
    public float AbilityAmount { get => m_AbilityAmount; }
    public float AbilityRange { get => m_AbilityRange; }
    public float AbilitySpeed { get => m_AbilitySpeed; }
    public float AbilityCooldown { get => m_AbilityCooldown; }
    public LayerMask AbilityLayerMask { get => m_AbilityLayerMask; }
    public List<AudioClip> AbilityAudioSounds { get => m_AbilityAudioSounds; }
    public List<AbilityResourceType> AbilityResourceTypes { get => m_AbilityResourceTypes; }
    public AbilityAnimationData AbilityAnimationData { get => m_AbilityAnimationData; }

    public event Action AbilityEnded;

    protected GameObject m_Owner;
    protected SFXManager m_SFXManager;
    protected PlayerAnimator m_PlayerAnimator;
    protected Vector3 m_Center = new Vector3(0.5f, 0.5f, 0);
    protected AbilityAnimationData m_AbilityAnimationData;
    protected List<OwnerResource> m_OwnerResources = new List<OwnerResource>();

    public virtual void InitAbility(GameObject owner)
    {
        //Debug.Log("Ability InitAbility");
        m_Owner = owner;
        m_SFXManager = GetComponent<SFXManager>();
        m_PlayerAnimator = m_Owner.GetComponentInChildren<PlayerAnimator>();

        foreach (AbilityResourceType resource in m_AbilityResourceTypes)
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

            m_OwnerResources.Add(ownerResource);
        }
    }

    public virtual void AbilityStart()
    {
        //Debug.Log("Ability AbilityStart");
        RemoveResources();
        m_PlayerAnimator.OnPlayAbilitySound += PlayAbilitySound;
        m_PlayerAnimator.OnAbilityEnd += AbilityEnd;
    }

    public virtual void PlayAbilitySound()
    {
        Debug.Log("Ability PlayAbilitySound");
        m_PlayerAnimator.OnPlayAbilitySound -= PlayAbilitySound;
        m_SFXManager.PlayAudioOnce(AbilityAudioSounds[0]);
    }

    public virtual void AbilityEnd()
    {
        //Debug.Log("Ability AbilityEnd");
        GameManager.Instance.PlayerManager.PlayerAction.IsActing = false;
        m_PlayerAnimator.OnPlayAbilitySound -= PlayAbilitySound;
        m_PlayerAnimator.OnAbilityEnd -= AbilityEnd;
        AbilityEnded?.Invoke();
    }

    public virtual bool CanCastAbility()
    {
        foreach (OwnerResource resource in m_OwnerResources)
        {
            if (resource.Resource?.Current <= resource.Cost)
            {
                return false;
            }
        }

        return true;
    }

    public void RemoveResources()
    {
        foreach (OwnerResource resource in m_OwnerResources)
        {
            resource.Resource?.Remove(resource.Cost, gameObject);
        }
    }

    public virtual void OnDestroy()
    {
        if (m_PlayerAnimator != null) m_PlayerAnimator.OnAbilityEnd -= AbilityEnd;
    }

    protected IEnumerator SlowDownAnimation(float time, float slowDown)
    {
        float duration = 0f;
        m_PlayerAnimator.PAnimator.SetFloat("Speed", slowDown);

        while (duration < time)
        {
            duration += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        m_PlayerAnimator.PAnimator.SetFloat("Speed", 1);
    }
}
