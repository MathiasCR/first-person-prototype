using UnityEngine;

public class AnomalyCollisionEffect : MonoBehaviour
{
    [SerializeField] private float m_Force;
    [SerializeField] private float m_Damage;
    [SerializeField] private float m_ImmuneDuration;

    private GameObject m_target;
    private float m_KnockbackTimer;
    private Vector3 m_KnockbackDirection;
    private bool m_IsKnockbackActive = false;

    private void Update()
    {
        // Si le knockback est actif
        if (m_IsKnockbackActive)
        {
            // Appliquer le knockback
            m_target.transform.position += m_KnockbackDirection * m_Force * Time.deltaTime;

            // Réduire le timer
            m_KnockbackTimer -= Time.deltaTime;

            // Arrêter le knockback une fois le timer expiré
            if (m_KnockbackTimer <= 0f)
            {
                m_IsKnockbackActive = false;
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out Health healthTarget))
        {
            healthTarget.Remove(m_Damage, gameObject);

            if (m_Force > 0)
            {
                Vector3 knockbackDirection = (other.transform.position - transform.position);
                m_KnockbackDirection = new Vector3(Mathf.Ceil(knockbackDirection.x), 0f, Mathf.Ceil(knockbackDirection.z));

                m_target = other;
                m_IsKnockbackActive = true;
                m_KnockbackTimer = 0.3f;
            }
        }
    }
}
