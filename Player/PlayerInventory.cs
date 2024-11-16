using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Equippable Armor => m_Armor;
    public Equippable ItemInHands => m_ItemInHands;
    public Dictionary<int, Equippable> Weapons => m_Weapons;
    public Dictionary<Item, int> InventoryItems => m_InventoryItems;
    public Dictionary<int, KeyValuePair<Equippable, int>> BeltItems => m_BeltItems;

    private Equippable m_Armor;
    private Equippable m_ItemInHands;
    private bool m_ReadyToEquip = false;
    private Equippable m_tempItemToEquip;
    private Dictionary<int, Equippable> m_Weapons = new Dictionary<int, Equippable>();
    private Dictionary<Item, int> m_InventoryItems = new Dictionary<Item, int>();
    private Dictionary<int, KeyValuePair<Equippable, int>> m_BeltItems = new Dictionary<int, KeyValuePair<Equippable, int>>();

    public event Action OnInHandsRemoved;
    public event Action<Item, int> OnInHandsUpdated;
    public event Action<Item, int> OnArmorItemUpdated;
    public event Action<Item, int> OnInventoryItemUpdated;
    public event Action<Item, int, int> OnBeltItemUpdated;
    public event Action<Item, int, int> OnWeaponItemUpdated;

    public void AddItemToInventory(Item item, int stack)
    {
        if (m_InventoryItems.ContainsKey(item))
        {
            m_InventoryItems[item] += stack;
            OnInventoryItemUpdated?.Invoke(item, m_InventoryItems[item]);
            return;
        }

        m_InventoryItems.Add(item, stack);
        OnInventoryItemUpdated?.Invoke(item, stack);
    }

    public void RemoveItemFromInventory(Item item)
    {
        if (m_InventoryItems.ContainsKey(item))
        {
            m_InventoryItems[item]--;

            if (m_InventoryItems[item] <= 0)
            {
                OnInventoryItemUpdated?.Invoke(item, m_InventoryItems[item]);
                m_InventoryItems.Remove(item);
            }
        }
    }

    public void EquipItemFromInventory(Equippable item, int index)
    {
        int stack = m_InventoryItems[item];
        switch (item.ItemType)
        {
            case ItemType.Armor:
                m_Armor = item;
                OnArmorItemUpdated?.Invoke(item, 1);
                break;
            case ItemType.Weapon:
                m_Weapons.Add(index, item);
                OnWeaponItemUpdated?.Invoke(item, 1, index);
                break;
            case ItemType.Belt:
                m_BeltItems.Add(index, new KeyValuePair<Equippable, int>(item, stack));
                OnBeltItemUpdated?.Invoke(item, stack, index);
                break;
        }

        OnInventoryItemUpdated?.Invoke(item, 0);
        m_InventoryItems.Remove(item);
    }

    public void MoveItemToInventory(Equippable item, int index)
    {
        if (m_ItemInHands != null && item.ItemName == m_ItemInHands.ItemName) StartRemoveFromHands();

        int stack = 1;
        switch (item.ItemType)
        {
            case ItemType.Armor:
                m_Armor = null;
                OnArmorItemUpdated?.Invoke(item, 0);
                break;
            case ItemType.Weapon:
                int weaponIndex = m_Weapons.FirstOrDefault(weapon => weapon.Value.ItemName == item.ItemName).Key;
                m_Weapons.Remove(weaponIndex);
                OnWeaponItemUpdated?.Invoke(item, 0, weaponIndex);
                break;
            case ItemType.Belt:
                int beltIndex = m_BeltItems.FirstOrDefault(beltItem => beltItem.Value.Key.ItemName == item.ItemName).Key;
                stack = m_BeltItems[beltIndex].Value;
                m_BeltItems.Remove(beltIndex);
                OnBeltItemUpdated?.Invoke(item, 0, beltIndex);
                break;
        }

        AddItemToInventory(item, stack);
    }

    public void MoveItemFromEquipSlotToAnother(ItemType type, int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex) return;

        switch (type)
        {
            case ItemType.Weapon:
                Equippable movedWeapon = m_Weapons[fromIndex];
                Equippable previousWeapon = null;

                if (m_Weapons.ContainsKey(toIndex))
                {
                    previousWeapon = m_Weapons[toIndex];
                    m_Weapons[toIndex] = movedWeapon;
                    m_Weapons[fromIndex] = previousWeapon;
                    OnWeaponItemUpdated?.Invoke(movedWeapon, 1, toIndex);
                    OnWeaponItemUpdated?.Invoke(previousWeapon, 1, fromIndex);
                }
                else
                {
                    m_Weapons.Remove(fromIndex);
                    m_Weapons.Add(toIndex, movedWeapon);
                    OnWeaponItemUpdated?.Invoke(movedWeapon, 0, fromIndex);
                    OnWeaponItemUpdated?.Invoke(movedWeapon, 1, toIndex);
                }

                if (m_ItemInHands == null) break;

                if (m_ItemInHands.ItemName == movedWeapon.ItemName)
                {
                    OnInHandsUpdated?.Invoke(movedWeapon, toIndex);
                }
                else if (previousWeapon != null && m_ItemInHands.ItemName == previousWeapon.ItemName)
                {
                    OnInHandsUpdated?.Invoke(previousWeapon, fromIndex);
                }

                break;
            case ItemType.Belt:
                KeyValuePair<Equippable, int> movedItem = m_BeltItems[fromIndex];
                KeyValuePair<Equippable, int> previousItem = new KeyValuePair<Equippable, int>();

                if (m_BeltItems.ContainsKey(toIndex))
                {
                    previousItem = m_BeltItems[toIndex];
                    m_BeltItems[toIndex] = movedItem;
                    m_BeltItems[fromIndex] = previousItem;
                    OnBeltItemUpdated?.Invoke(movedItem.Key, movedItem.Value, toIndex);
                    OnBeltItemUpdated?.Invoke(previousItem.Key, previousItem.Value, fromIndex);
                }
                else
                {
                    m_BeltItems.Remove(fromIndex);
                    m_BeltItems.Add(toIndex, movedItem);
                    OnBeltItemUpdated?.Invoke(movedItem.Key, 0, fromIndex);
                    OnBeltItemUpdated?.Invoke(movedItem.Key, movedItem.Value, toIndex);
                }

                if (m_ItemInHands == null) break;

                if (m_ItemInHands.ItemName == movedItem.Key.ItemName)
                {
                    OnInHandsUpdated?.Invoke(movedItem.Key, toIndex);
                }
                else if (m_ItemInHands.ItemName == previousItem.Key?.ItemName)
                {
                    OnInHandsUpdated?.Invoke(previousItem.Key, fromIndex);
                }

                break;
        }
    }

    public void TakeItemInHands(ItemType type, int index)
    {
        Equippable item = null;
        m_tempItemToEquip = null;

        switch (type)
        {
            case ItemType.Weapon:
                item = m_Weapons.GetValueOrDefault(index);
                break;
            case ItemType.Belt:
                item = m_BeltItems.GetValueOrDefault(index).Key;
                break;
        }

        if (item != null && (m_ItemInHands == null || item.ItemName != m_ItemInHands.ItemName))
        {
            StartRemoveFromHands();
            m_tempItemToEquip = item;
            StartCoroutine(WaitUnequipBeforeEquip(item));
            OnInHandsUpdated?.Invoke(item, index);
        }
        else
        {
            // Already Equipped so no anim
            GameManager.Instance.PlayerManager.PlayerAction.IsActing = false;
        }
    }

    private IEnumerator WaitUnequipBeforeEquip(Equippable item)
    {
        while (!m_ReadyToEquip)
        {
            yield return null;
        }

        GameManager.Instance.PlayerManager.PlayerAnimator.SetLayer(item.GetSubtype(), 1);
        GameManager.Instance.PlayerManager.PlayerAnimator.SetTrigger("Equip");
        GameManager.Instance.PlayerManager.PlayerAnimator.AnimationEquipAddItem += InstantiateItemGO;
    }

    private void InstantiateItemGO()
    {
        GameManager.Instance.PlayerManager.PlayerAnimator.AnimationEquipAddItem -= InstantiateItemGO;
        if (m_tempItemToEquip == null) return;
        if (m_ItemInHands != null) Destroy(m_ItemInHands.gameObject);

        Equippable go = Instantiate(m_tempItemToEquip, GameManager.Instance.PlayerManager.WeaponPlaceHolder, false);
        go.SetOwner(gameObject);
        m_ItemInHands = go;
        m_ItemInHands.GetComponent<SFXManager>().PlayAudioOnce(m_ItemInHands.EquipAudioClip);
        m_ReadyToEquip = false;
    }

    private void StartRemoveFromHands()
    {
        OnInHandsRemoved?.Invoke();

        if (m_ItemInHands != null)
        {
            m_ReadyToEquip = false;
            GameManager.Instance.PlayerManager.PlayerAnimator.SetTrigger("Unequip");
            m_ItemInHands.GetComponent<SFXManager>().PlayAudioOnce(m_ItemInHands.UnEquipAudioClip);
            GameManager.Instance.PlayerManager.PlayerAnimator.AnimationUnEquipRemoveItem += RemoveItemGO;
            GameManager.Instance.PlayerManager.PlayerAnimator.AnimationUnEquipEnded += EndRemoveFromHands;
        }
        else
        {
            m_ReadyToEquip = true;
        }
    }

    private void RemoveItemGO()
    {
        GameManager.Instance.PlayerManager.PlayerAnimator.AnimationUnEquipRemoveItem -= RemoveItemGO;
        Destroy(m_ItemInHands.gameObject);
    }

    public void EndRemoveFromHands()
    {
        GameManager.Instance.PlayerManager.PlayerAnimator.AnimationUnEquipEnded -= EndRemoveFromHands;
        GameManager.Instance.PlayerManager.PlayerAnimator.SetLayer(m_ItemInHands.GetSubtype(), 0);
        m_ReadyToEquip = true;
    }

    public void RemoveItemFromStack()
    {
        switch (m_ItemInHands.ItemType)
        {
            case ItemType.Belt:
                int index = m_BeltItems.FirstOrDefault(beltItem => beltItem.Value.Key.ItemName == m_ItemInHands.ItemName).Key;

                if (m_BeltItems[index].Value - 1 <= 0)
                {
                    OnBeltItemUpdated?.Invoke(m_BeltItems[index].Key, 0, index);
                    m_BeltItems.Remove(index);
                    StartRemoveFromHands();
                }
                else
                {
                    m_BeltItems[index] = new KeyValuePair<Equippable, int>(m_BeltItems[index].Key, m_BeltItems[index].Value - 1);
                    OnBeltItemUpdated?.Invoke(m_BeltItems[index].Key, m_BeltItems[index].Value, index);
                }

                break;
        }
    }
}
