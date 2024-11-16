using System;
using System.Collections.Generic;
using UnityEngine;

public interface IAbility
{
    GameObject Owner { get; }
    string AbilityName { get; }
    string AbilityDescription { get; }
    float AbilityAmount { get; }
    float AbilityRange { get; }
    float AbilitySpeed { get; }
    float AbilityCooldown { get; }
    LayerMask AbilityLayerMask { get; }
    List<AudioClip> AbilityAudioSounds { get; }
    List<AbilityResourceType> AbilityResourceTypes { get; }
    AbilityAnimationData AbilityAnimationData { get; }
    void InitAbility(GameObject owner);
    void AbilityStart();
    void PlayAbilitySound();
    void AbilityEnd();
    bool CanCastAbility();
    void RemoveResources();
}

[Serializable]
public struct AbilityAnimationData
{
    public string AbilityAnimatorParameterName;
    public AnimatorControllerParameterType AbilityAnimatorParameterType;
}

[Serializable]
public struct AbilityResourceType
{
    public ResourceType ResourceType;
    public float Cost;
}

[Serializable]
public struct OwnerResource
{
    public Resource Resource;
    public float Cost;
}
