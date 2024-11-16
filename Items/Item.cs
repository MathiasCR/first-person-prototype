using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected bool m_IsEquippable;
    [SerializeField] protected Sprite m_ItemIcon;
    [SerializeField] protected string m_ItemName;
    [TextArea][SerializeField] protected string m_ItemDetail;
    [SerializeField] protected ItemType m_ItemType;

    public bool IsEquippable { get => m_IsEquippable; }
    public Sprite ItemIcon { get => m_ItemIcon; }
    public string ItemName { get => m_ItemName; }
    public string ItemDetail { get => m_ItemDetail; }
    public ItemType ItemType { get => m_ItemType; }
}

public enum ItemType
{
    All,
    Armor,
    Weapon,
    Belt,
    Material,
    KeyItem
}

public enum BeltType
{
    Torch,
    Potion,
    Artefact
}