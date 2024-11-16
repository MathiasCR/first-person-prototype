using UnityEngine;

public interface IProjectileAbility
{
    bool HasProjectileAtIdle { get; }
    float ProjectileSpeed { get; }
    GameObject Projectile { get; }
    Transform ProjectileSpawner { get; }
    Projectile CurrentProjectile { get; }
    AudioClip CreateProjectileAudio { get; }
    AudioClip FireProjectileAudio { get; }

    void CreateProjectile();
    void FireProjectile();
    void CreateProjectilePlayAudio();
    void FireProjectilePlayAudio();
}
