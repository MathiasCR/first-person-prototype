using UnityEngine;

public abstract class Equippable : Item
{
    [SerializeField] protected AudioClip m_EquipAudioClip;
    [SerializeField] protected AudioClip m_UnEquipAudioClip;

    public AudioClip EquipAudioClip { get => m_EquipAudioClip; }
    public AudioClip UnEquipAudioClip { get => m_UnEquipAudioClip; }
    public GameObject Owner { get => m_Owner; }

    protected GameObject m_Owner;

    public void SetOwner(GameObject owner)
    {
        m_Owner = owner;
    }

    public abstract string GetSubtype();
}
