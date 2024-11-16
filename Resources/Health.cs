using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Basic behavior for tracking the health of an object.
/// </summary>
public class Health : Resource
{
    [Tooltip("Notifies listeners that this object has recieved a hit")]
    public OnHitEvent OnHit;

    [Tooltip("Notifies listeners that this object has died")]
    public UnityEvent Died;

    public event Action OnHitCallback;

    private bool m_IsDead;
    private bool m_IsBlocking;
    private bool m_IsInvulnerable;
    private float m_DamageReduction;

    public bool IsBlocking { get => m_IsBlocking; set => m_IsBlocking = value; }
    public bool IsInvulnerable { get => m_IsInvulnerable; set => m_IsInvulnerable = value; }
    public float DamageReduction { get => m_DamageReduction; set => m_DamageReduction = value; }

    // Heals the GameObject, up to the maximum value and notifies listeners
    public override void Add(float amount, GameObject origin)
    {
        // Don't heal if already dead
        if (m_IsDead)
            return;

        base.Add(amount, origin);
    }

    // Applies damage to the GameObject.
    public override void Remove(float amount, GameObject origin)
    {
        // If already dead, do nothing
        if (m_IsDead || m_IsInvulnerable)
            return;

        if (m_IsBlocking && TryGetComponent(out Stamina stamina))
        {
            stamina.Remove(amount * m_DamageReduction, origin);

            amount -= (amount * m_DamageReduction);
        }

        base.Remove(amount, origin);

        OnHit?.Invoke(origin);
        OnHitCallback?.Invoke();

        // Check for death condition.
        if (m_Current <= 0)
        {
            Die();
        }
    }

    public void MakeImmuneForDuration(float duration)
    {
        m_IsInvulnerable = true;
        Invoke(nameof(EndImmunity), duration);
    }

    private void EndImmunity()
    {
        m_IsInvulnerable = false;
    }

    // Notify listeners that this object is dead and disable the GameObject to prevent further interaction.
    private void Die()
    {
        // Only die once
        if (m_IsDead)
            return;

        m_IsDead = true;
        Died?.Invoke();
    }
}

[System.Serializable]
public class OnHitEvent : UnityEvent<GameObject>
{
}
