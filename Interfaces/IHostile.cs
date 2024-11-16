using UnityEngine;

public interface IHostile
{
    float AttackDamage { get; }
    float AbilityDamage { get; }
    float SightAngle { get; }
    float AttackRange { get; }
    float AbilityMinRange { get; }
    float AbilityMaxRange { get; }
    float AttackCooldown { get; }
    float AbilityCooldown { get; }
    LayerMask TargetLayerMask { get; }

    void Attack();
    void Ability();
    bool IsTargetInLineOfSight(GameObject target);
}