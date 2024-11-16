using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, ISlot
{
    [SerializeField] private Image m_Icon;
    [SerializeField] private GameObject m_Stack;
    [SerializeField] private Color m_DefaultColor;
    [SerializeField] private Color m_AvailableColor;
    [SerializeField] private Sprite m_DefautSlotIcon;
    [SerializeField] private TextMeshProUGUI m_StackNbr;

    public Image Icon { get => m_Icon; }
    public GameObject Stack { get => m_Stack; }
    public TextMeshProUGUI StackNbr { get => m_StackNbr; }
    public Sprite DefautSlotIcon { get => m_DefautSlotIcon; }
    public ItemSlot ItemSlot { get => m_ItemSlot; set => m_ItemSlot = value; }

    private ItemSlot m_ItemSlot;

    public void UpdateIconImage()
    {
        float transparence = 0.1f;
        m_Icon.sprite = m_DefautSlotIcon;

        if (m_Stack != null)
        {
            m_Stack.SetActive(false);
        }

        if (m_Icon.TryGetComponent(out EquipmentDrag equipmentDrag))
        {
            equipmentDrag.SetItem(null, 0);
            equipmentDrag.OnDragBegin -= GameManager.Instance.PlayerManager.InventoryUI.ChangeEquipmentIconColorByType;
            equipmentDrag.OnDragEnd -= GameManager.Instance.PlayerManager.InventoryUI.ResetEquipmentIconColor;
        }

        if (m_ItemSlot != null)
        {
            m_Icon.sprite = m_ItemSlot.Item.ItemIcon;
            transparence = 1f;

            if (m_ItemSlot.Item.TryGetComponent(out Equippable equippable) == true)
            {
                EquipmentDrag drag = m_Icon.GetComponent<EquipmentDrag>();
                drag.SetItem(equippable, m_ItemSlot.Index);
                drag.OnDragBegin += GameManager.Instance.PlayerManager.InventoryUI.ChangeEquipmentIconColorByType;
                drag.OnDragEnd += GameManager.Instance.PlayerManager.InventoryUI.ResetEquipmentIconColor;
            }

            if (m_Stack != null && m_ItemSlot.Stack > 1)
            {
                m_Stack.SetActive(true);
                m_StackNbr.text = m_ItemSlot.Stack.ToString();
            }
        }

        Color color = m_Icon.color;
        color.a = transparence;
        m_Icon.color = color;
    }

    public void ChangeIconColor(bool isAvailable)
    {
        if (isAvailable)
        {
            GetComponent<Image>().color = m_AvailableColor;
        }
        else
        {
            GetComponent<Image>().color = m_DefaultColor;
        }
    }
}
