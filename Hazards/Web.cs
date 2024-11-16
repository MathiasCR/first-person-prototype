using System;
using UnityEngine;

public class Web : MonoBehaviour
{
    [SerializeField] private Modifier m_SpeedDebuff;
    [SerializeField] private ParticleSystem m_DestroyParticleSystem;

    public event Action OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Spider") return;

        if (other.TryGetComponent(out ModifierManager modifierManager))
        {
            modifierManager.AddModifier(m_SpeedDebuff);
            ParticleSystem ps = Instantiate(m_DestroyParticleSystem, other.transform.position + other.transform.forward, Quaternion.identity, other.transform);
            ps.Emit(10);
            OnTrigger?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Spider") return;

        if (collision.gameObject.TryGetComponent(out ModifierManager modifierManager))
        {
            modifierManager.AddModifier(m_SpeedDebuff);
            ParticleSystem ps = Instantiate(m_DestroyParticleSystem, collision.transform.position + collision.transform.forward, Quaternion.identity, collision.transform);
            ps.Emit(3);
            Destroy(gameObject);
        }
    }
}
