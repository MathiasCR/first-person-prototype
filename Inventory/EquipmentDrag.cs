using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Equippable Item;
    public int SlotIndex;

    public event Action OnDragEnd;
    public event Action<ItemType> OnDragBegin;

    private Image m_Image;
    private Vector3 m_Position;
    private RectTransform m_PanelRectTransform;

    private void Start()
    {
        m_Image = GetComponent<Image>();
        m_Position = transform.localPosition;
    }

    public void SetItem(Equippable item, int index)
    {
        Item = item;
        SlotIndex = index;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Item == null)
        {
            eventData.pointerDrag = null;
            return;
        }
        m_PanelRectTransform = GameManager.Instance.PlayerManager.InventoryUI.GetEquipmentPanel(Item.ItemType);
        m_PanelRectTransform.SetAsLastSibling();
        OnDragBegin?.Invoke(Item.ItemType);
        m_Image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnDragEnd?.Invoke();
        if (m_Image != null) m_Image.raycastTarget = true;
        transform.localPosition = m_Position;
    }
}
