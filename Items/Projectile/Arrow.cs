using UnityEngine;

public class Arrow : Projectile
{
    public override void FireProjectile()
    {
        base.FireProjectile();
        gameObject.layer = 0;
        gameObject.transform.GetChild(0).gameObject.layer = 0;
    }

    public override void OnCollisionEnter(Collision collision)
    {
        m_Rigidbody.velocity = Vector3.zero;

        Debug.Log("Hit for : " + Mathf.RoundToInt(m_Damage));
        if (collision.collider.transform.root.TryGetComponent(out Health health))
        {
            gameObject.transform.SetParent(collision.collider.transform, true);
            gameObject.transform.position = collision.GetContact(0).point - gameObject.transform.forward / 2;
            health.Remove(Mathf.RoundToInt(m_Damage), m_Owner);
        }

        PrepareProjectileToStop(true);
        Destroy(gameObject.GetComponent<Collider>());
        Destroy(gameObject.GetComponent<Rigidbody>());

        m_ProjectileHitSystem.TriggerHitSystem(collision);

        DestroyProjectile(30f);
    }
}
