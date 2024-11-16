using UnityEngine;

public class Potion : Equippable
{
    [SerializeField] protected PotionAbility m_PotionAbility;

    public PotionAbility PotionAbility { get => m_PotionAbility; }

    private void Start()
    {
        m_PotionAbility.InitAbility(m_Owner);
        m_PotionAbility.OnPotionUsed += OnPotionUsed;
    }

    public override string GetSubtype()
    {
        return BeltType.Potion.ToString();
    }

    private void OnPotionUsed()
    {
        GameManager.Instance.PlayerManager.PlayerInventory.RemoveItemFromStack();
    }
}
