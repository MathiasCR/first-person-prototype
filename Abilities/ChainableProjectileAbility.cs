using UnityEngine;

public class ChainableProjectileAbility : ChainableAbility, IProjectileAbility
{
    public bool HasProjectileAtIdle => throw new System.NotImplementedException();

    public float ProjectileSpeed => throw new System.NotImplementedException();

    public GameObject Projectile => throw new System.NotImplementedException();

    public Transform ProjectileSpawner => throw new System.NotImplementedException();

    public Projectile CurrentProjectile => throw new System.NotImplementedException();

    public AudioClip CreateProjectileAudio => throw new System.NotImplementedException();

    public AudioClip FireProjectileAudio => throw new System.NotImplementedException();

    public void CreateProjectile()
    {
        throw new System.NotImplementedException();
    }

    public void CreateProjectilePlayAudio()
    {
        throw new System.NotImplementedException();
    }

    public void FireProjectile()
    {
        throw new System.NotImplementedException();
    }

    public void FireProjectilePlayAudio()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
