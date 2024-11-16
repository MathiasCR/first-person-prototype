using UnityEngine;
using UnityEngine.Events;

public abstract class Resource : MonoBehaviour, IResource
{
    [SerializeField] protected float m_Max;
    [SerializeField] protected float m_Current;
    [SerializeField] protected float m_RegenRate;
    [SerializeField] protected float m_RegenDelay;
    [SerializeField] protected bool m_ResetOnStart;

    [Tooltip("Notifies listeners of updated value")]
    public UnityEvent<float> ResourceChanged;

    public bool StopRegen { get => m_StopRegen; set => m_StopRegen = value; }
    public float Max => m_Max;
    public float Current => m_Current;
    public float RegenRate => m_RegenRate;
    public float RegenDelay => m_RegenDelay;
    public bool ResetOnStart => m_ResetOnStart;

    protected bool m_StopRegen = false;
    protected float m_TimeSinceLastUse = 0f;

    private void Awake()
    {
        if (m_ResetOnStart)
            m_Current = Max;
    }

    private void Start()
    {
        ResourceChanged?.Invoke(m_Current);
    }

    private void Update()
    {
        m_TimeSinceLastUse += Time.deltaTime;

        if (m_TimeSinceLastUse >= RegenDelay)
        {
            Regen();
        }
    }

    public virtual void Add(float amount, GameObject origin)
    {
        m_Current += amount;

        if (m_Current > Max)
            m_Current = Max;

        ResourceChanged?.Invoke(m_Current);
    }

    public virtual void Remove(float amount, GameObject origin)
    {
        m_Current -= amount;

        if (m_Current <= 0)
        {
            m_Current = 0;
        }

        m_TimeSinceLastUse = 0f;
        ResourceChanged?.Invoke(m_Current);
    }

    public virtual void Regen()
    {
        if (m_Current < m_Max && !m_StopRegen)
        {
            m_Current += m_RegenRate * Time.deltaTime;
            ResourceChanged?.Invoke(m_Current);
        }
    }

    public bool CanUse(float cost)
    {
        return m_Current >= cost;
    }
}
