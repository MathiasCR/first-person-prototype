using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Equippable Item;

    public event Action OnDragEnd;
    public event Action<ItemType> OnDragBegin;

    private Image m_Image;
    private Toggle m_Toggle;
    private Vector3 m_Position;
    private RectTransform m_PanelRectTransform;

    private void Start()
    {
        m_Image = GetComponent<Image>();
        m_Position = transform.localPosition;
        m_Toggle = GetComponentInParent<Toggle>();
        m_PanelRectTransform = GameManager.Instance.PlayerManager.InventoryUI.ItemsPanel.GetComponent<RectTransform>();
    }

    public void SetItem(Equippable item)
    {
        Item = item;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Item == null) eventData.pointerDrag = null;
        m_Toggle.isOn = true;
        m_Toggle.isOn = false;
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
