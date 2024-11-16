using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDrop : MonoBehaviour, IDropHandler
{
    public int SlotIndex;

    public void OnDrop(PointerEventData eventData)
    {
        EquipmentDrag draggable = eventData.pointerDrag.GetComponentInParent<EquipmentDrag>();
        if (draggable != null && draggable.Item != null)
        {
            GameManager.Instance.PlayerManager.PlayerInventory.MoveItemToInventory(draggable.Item, SlotIndex);
        }
    }
}
