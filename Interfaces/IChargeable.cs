using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IChargeable
{
    bool IsHolding { get; }
    bool NeedCompletion { get; }
    bool ReleaseOnCompletion { get; }
    bool ChargeableDamage { get; }
    bool ChargeableRange { get; }
    bool ChargeableSize { get; }
    bool ChargeCompleted { get; }
    float ChargeTime { get; }
    List<AbilityResourceType> DrainedResourceTypes { get; }
    AudioClip ChargeAudioClip { get; }
    void AbilityCharge();
    void AbilityChargePlayAudio();
    void AbilityRelease();
    IEnumerator ChargeTimer();
    void DrainResources();
}
