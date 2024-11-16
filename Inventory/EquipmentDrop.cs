using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentDrop : MonoBehaviour, IDropHandler
{
    [SerializeField] private List<ItemType> m_SlotType;

    public int SlotIndex;

    public void OnDrop(PointerEventData eventData)
    {
        InventoryDrag inventoryDrag = eventData.pointerDrag.GetComponentInParent<InventoryDrag>();
        if (inventoryDrag != null && inventoryDrag.Item != null && m_SlotType.Contains(inventoryDrag.Item.ItemType))
        {
            GameManager.Instance.PlayerManager.PlayerInventory.EquipItemFromInventory(inventoryDrag.Item, SlotIndex);
        }

        EquipmentDrag equipmentDrag = eventData.pointerDrag.GetComponentInParent<EquipmentDrag>();
        if (equipmentDrag != null && equipmentDrag.Item != null && m_SlotType.Contains(equipmentDrag.Item.ItemType))
        {
            GameManager.Instance.PlayerManager.PlayerInventory.MoveItemFromEquipSlotToAnother(equipmentDrag.Item.ItemType, equipmentDrag.SlotIndex, SlotIndex);
        }
    }
}
