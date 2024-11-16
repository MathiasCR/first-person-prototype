using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected bool m_FollowHand;
    [SerializeField] protected float m_SizePerCharge;
    [SerializeField] protected float m_SpeedPerCharge;
    [SerializeField] protected float m_DamagePerCharge;
    [SerializeField] protected HitSystem m_ProjectileHitSystem;

    protected bool m_Fired;
    protected bool m_Stopped;
    protected float m_Speed;
    protected float m_Damage;
    protected GameObject m_Owner;
    protected Collider m_Collider;
    protected Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    protected void Start()
    {
        m_Fired = false;
        m_Stopped = false;
        m_Collider = GetComponent<Collider>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Collider.enabled = false;
        m_Rigidbody.useGravity = false;
        m_Rigidbody.freezeRotation = false;
        m_Rigidbody.isKinematic = false;

        if (m_FollowHand)
        {
            transform.position = GameManager.Instance.PlayerManager.LeftHand.position;
        }
    }

    protected void Update()
    {
        if (m_Stopped) return;

        if (m_Fired)
        {
            m_Rigidbody.rotation = Quaternion.LookRotation(m_Rigidbody.velocity);
        }
        else
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 2f), (Screen.height / 1.9f), 10f));

            transform.LookAt(point);

            if (m_FollowHand)
            {
                transform.position = GameManager.Instance.PlayerManager.LeftHand.position;
            }
            else
            {
                transform.position = transform.parent.position;
            }
        }
    }

    public void InitProjectile(GameObject owner, float damage, float speed)
    {
        m_Owner = owner;
        m_Speed = speed;
        m_Damage = damage;
    }

    public void UpdateDamage(float chargeTime)
    {
        m_Damage += (chargeTime * m_DamagePerCharge);
    }

    public void UpdateSpeed(float chargeTime)
    {
        m_Speed += (chargeTime * m_SpeedPerCharge);
    }

    public void UpdateSize(float chargeTime)
    {
        float scale = chargeTime / m_SizePerCharge;
        transform.localScale += Vector3.one * scale;
    }

    public virtual void FireProjectile()
    {
        Debug.Log("Fire at : " + m_Speed);
        PrepareProjectileToStop(false);
        m_Rigidbody.AddForce(transform.forward * m_Speed, ForceMode.Impulse);
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        m_ProjectileHitSystem.TriggerHitSystem(collision);

        Debug.Log("Hit for : " + Mathf.RoundToInt(m_Damage));
        if (collision.collider.transform.root.TryGetComponent(out Health health))
        {
            health.Remove(Mathf.RoundToInt(m_Damage), m_Owner);
        }

        DestroyProjectile(0f);
    }

    protected void PrepareProjectileToStop(bool isStopped)
    {
        m_Fired = !isStopped;
        m_Stopped = isStopped;
        m_Collider.enabled = !isStopped;
        m_Rigidbody.useGravity = !isStopped;
        m_Rigidbody.freezeRotation = true;
    }

    protected void DestroyProjectile(float time)
    {
        Destroy(gameObject, time);
    }
}
