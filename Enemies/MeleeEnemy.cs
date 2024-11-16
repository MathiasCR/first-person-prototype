using UnityEngine;

public abstract class MeleeEnemy : Enemy, IMeleeEnemy
{
    public void CheckIfAttackLanded()
    {
        Ray ray = new Ray(transform.position, transform.forward + transform.up / 2);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, m_AttackRange, TargetLayerMask) && hit.transform.TryGetComponent(out Health targetHealth))
        {
            Debug.Log("player hit");
            m_AudioSource.PlayOneShot(m_AttackSound);
            targetHealth.Remove(m_AttackDamage, gameObject);
        }
    }
}
