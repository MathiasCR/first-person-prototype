using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Button m_UseBtn;
    [SerializeField] private int m_NbrBeltSlots;
    [SerializeField] private int m_NbrWeaponSlots;
    [SerializeField] private int m_NbrInventorySlots;
    [SerializeField] private GameObject m_BeltPanel;
    [SerializeField] private GameObject m_ArmorPanel;
    [SerializeField] private GameObject m_ItemsPanel;
    [SerializeField] private GameObject m_WeaponPanel;
    [SerializeField] private GameObject m_FilterPanel;
    [SerializeField] private GameObject m_FilterPrefab;
    [SerializeField] private Color m_ColorDefaultFilter;
    [SerializeField] private Color m_ColorSelectedFilter;
    [SerializeField] private GameObject m_InventoryPanel;
    [SerializeField] private GameObject m_ContextualMenu;
    [SerializeField] private GameObject m_BeltSlotPrefab;
    [SerializeField] private GameObject m_ArmorSlotPrefab;
    [SerializeField] private GameObject m_WeaponSlotPrefab;
    [SerializeField] private GameObject m_InventorySlotPrefab;
    [SerializeField] private TextMeshProUGUI m_DetailPanelDetails;
    [SerializeField] private TextMeshProUGUI m_DetailPanelItemName;
    [SerializeField] private TextMeshProUGUI m_DetailPanelFirstAbility;
    [SerializeField] private TextMeshProUGUI m_DetailPanelSecondAbility;

    public GameObject BeltPanel => m_BeltPanel;
    public GameObject ArmorPanel => m_ArmorPanel;
    public GameObject ItemsPanel => m_ItemsPanel;
    public GameObject WeaponPanel => m_WeaponPanel;

    private PlayerInventory m_Inventory;

    private Vector3 m_HiddenPosition;
    private Vector3 m_OpenedPosition;
    private float m_InventoryAnimSpeed = 200f;
    private float m_InventoryAnimDuration = 0.2f;

    private ToggleGroup m_ToggleGroup;
    private ItemSlot m_ArmorItemSlot;
    private EquipmentSlot m_ArmorSlot;
    private ItemType m_CurrentFilter = ItemType.All;
    private List<ItemSlot> m_ItemSlots = new List<ItemSlot>();
    private List<ItemSlot> m_FilteredItemSlots = new List<ItemSlot>();
    private List<ItemSlot> m_BeltItemSlots = new List<ItemSlot>();
    private List<ItemSlot> m_WeaponItemSlots = new List<ItemSlot>();
    private List<GameObject> m_InventoryFilters = new List<GameObject>();
    private List<EquipmentSlot> m_BeltSlots = new List<EquipmentSlot>();
    private List<EquipmentSlot> m_WeaponSlots = new List<EquipmentSlot>();
    private List<InventorySlot> m_InventorySlots = new List<InventorySlot>();


    private void Awake()
    {
        m_ToggleGroup = m_ItemsPanel.GetComponent<ToggleGroup>();

        GameObject armorSlot = Instantiate(m_ArmorSlotPrefab, m_ArmorPanel.transform);
        m_ArmorSlot = armorSlot.GetComponent<EquipmentSlot>();

        m_BeltSlots = InitializeInventorySlots(m_NbrBeltSlots, m_BeltSlotPrefab, m_BeltPanel);
        m_WeaponSlots = InitializeInventorySlots(m_NbrWeaponSlots, m_WeaponSlotPrefab, m_WeaponPanel);

        for (int i = 0; i < m_NbrInventorySlots; i++)
        {
            int index = i;
            GameObject go = Instantiate(m_InventorySlotPrefab, m_ItemsPanel.transform);
            go.GetComponent<Toggle>().group = m_ToggleGroup;
            go.GetComponent<Toggle>().onValueChanged.AddListener((value) => { ChangeToggleColor(go, index, value); });
            m_InventorySlots.Add(go.GetComponent<InventorySlot>());
        };

        foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
        {
            GameObject go = Instantiate(m_FilterPrefab, m_FilterPanel.transform);
            go.name = itemType.ToString();
            go.GetComponent<Button>().onClick.AddListener(() => FilterInventoryByType(itemType));
            m_InventoryFilters.Add(go);
        }
    }

    private void Start()
    {
        m_Inventory = GameManager.Instance.PlayerManager.PlayerInventory;
        m_Inventory.OnBeltItemUpdated += UpdateBeltInSlotItem;
        m_Inventory.OnArmorItemUpdated += UpdateArmorInSlotItem;
        m_Inventory.OnWeaponItemUpdated += UpdateWeaponInSlotItem;
        m_Inventory.OnInventoryItemUpdated += UpdateItemInSlotItem;

        m_HiddenPosition = new Vector3(-2300f, m_InventoryPanel.transform.position.y, m_InventoryPanel.transform.position.z);
        m_OpenedPosition = new Vector3(1000f, m_InventoryPanel.transform.position.y, m_InventoryPanel.transform.position.z);
    }

    private List<EquipmentSlot> InitializeInventorySlots(int nbrSlots, GameObject prefab, GameObject parent)
    {
        List<EquipmentSlot> temp = new List<EquipmentSlot>();
        for (int i = 0; i < nbrSlots; i++)
        {
            GameObject go = Instantiate(prefab, parent.transform);
            EquipmentDrop drop = go.GetComponent<EquipmentDrop>();
            drop.SlotIndex = i;
            temp.Add(go.GetComponent<EquipmentSlot>());
        };

        return temp;
    }

    private void UpdateItemInSlotItem(Item item, int stack)
    {
        if (stack <= 0)
        {
            ItemSlot itemToRemove = m_ItemSlots.Find(itemSlot => itemSlot.Item == item);
            if (itemToRemove == null) return;
            m_ItemSlots.Remove(itemToRemove);
        }
        else
        {
            ItemSlot itemSlot = m_ItemSlots.Find(itemSlot => itemSlot.Item == item);
            if (itemSlot != null)
            {
                itemSlot.Stack = stack;
            }
            else
            {
                itemSlot = new ItemSlot(item, stack, 0);
                m_ItemSlots.Add(itemSlot);
            }
        }

        FilterInventoryByType(m_CurrentFilter);
    }

    private void UpdateWeaponInSlotItem(Item item, int stack, int index)
    {
        int slotIndex = m_WeaponItemSlots.FindIndex(slot => slot.Item == item);

        if (slotIndex != -1)
        {
            m_WeaponItemSlots.RemoveAt(slotIndex);

            if (stack > 0)
            {
                m_WeaponItemSlots.Insert(slotIndex, new ItemSlot(item, stack, index));
            }
        }
        else
        {
            m_WeaponItemSlots.Add(new ItemSlot(item, stack, index));
        }

        UpdateShownItems();
    }

    private void UpdateBeltInSlotItem(Item item, int stack, int index)
    {
        int slotIndex = m_BeltItemSlots.FindIndex(slot => slot.Item == item);

        if (slotIndex != -1)
        {
            m_BeltItemSlots.RemoveAt(slotIndex);

            if (stack > 0)
            {
                m_BeltItemSlots.Insert(slotIndex, new ItemSlot(item, stack, index));
            }
        }
        else
        {
            m_BeltItemSlots.Add(new ItemSlot(item, stack, index));
        }

        UpdateShownItems();
    }

    private void UpdateArmorInSlotItem(Item item, int stack)
    {
        if (stack <= 0)
        {
            m_ArmorItemSlot = null;
        }
        else
        {
            m_ArmorItemSlot = new ItemSlot(item, 1, 0);
        }

        UpdateShownItems();
    }

    public void ShowInventory(bool show)
    {
        StopAllCoroutines();
        HideContextualMenu();

        //Show before animation
        if (show)
        {
            m_ToggleGroup.SetAllTogglesOff();
            m_CurrentFilter = ItemType.All;
            FilterInventoryByType(m_CurrentFilter);
            m_InventoryPanel.SetActive(show);
        }

        StartCoroutine(InventoryAnimation(show));
    }

    private IEnumerator InventoryAnimation(bool show)
    {
        float duration = 0f;

        if (!show) Time.timeScale = 1f;

        while (duration < m_InventoryAnimDuration)
        {
            m_InventoryPanel.transform.position = show ? Vector3.MoveTowards(m_InventoryPanel.transform.position, m_OpenedPosition, m_InventoryAnimSpeed) :
                Vector3.MoveTowards(m_InventoryPanel.transform.position, m_HiddenPosition, m_InventoryAnimSpeed);
            duration += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        if (show) Time.timeScale = 0f;

        //Hide after animation
        if (!show)
        {
            m_InventoryPanel.SetActive(show);
        }
    }

    private void UpdateShownItems()
    {
        ResetEquipmentIconColor();

        foreach (InventorySlot slot in m_InventorySlots)
        {
            slot.ItemSlot = null;
            slot.UpdateIconImage();
        }

        int index = 0;
        foreach (ItemSlot item in m_FilteredItemSlots)
        {
            InventorySlot slot = m_InventorySlots[index];
            slot.ItemSlot = item;
            slot.UpdateIconImage();
            index++;
        }

        foreach (EquipmentSlot slot in m_WeaponSlots)
        {
            slot.ItemSlot = null;
            slot.UpdateIconImage();
        }

        foreach (ItemSlot item in m_WeaponItemSlots)
        {
            EquipmentSlot slot = m_WeaponSlots[item.Index];
            slot.ItemSlot = item;
            slot.UpdateIconImage();
        }

        foreach (EquipmentSlot slot in m_BeltSlots)
        {
            slot.ItemSlot = null;
            slot.UpdateIconImage();
        }

        foreach (ItemSlot item in m_BeltItemSlots)
        {
            EquipmentSlot slot = m_BeltSlots[item.Index];
            slot.ItemSlot = item;
            slot.UpdateIconImage();
        }

        if (m_ArmorItemSlot != null)
        {
            m_ArmorSlot.ItemSlot = m_ArmorItemSlot;
            m_ArmorSlot.UpdateIconImage();
        }
        else
        {
            m_ArmorSlot.ItemSlot = null;
            m_ArmorSlot.UpdateIconImage();
        }
    }

    private void ShowContextualMenu(int index)
    {
        Vector3 offset = new Vector3(300f, -300f, 0f);
        //contextual menu on button index with command depending on item index
        if (m_FilteredItemSlots.Count > index)
        {
            ItemSlot itemSlot = m_FilteredItemSlots[index];
            if (itemSlot.Item.GetComponent<IUsable>() == null)
            {
                m_UseBtn.enabled = false;
            }
            else
            {
                m_UseBtn.enabled = true;
                m_UseBtn.onClick.RemoveAllListeners();
                m_UseBtn.onClick.AddListener(() => { HideContextualMenu(); });
            }

            m_DetailPanelItemName.text = itemSlot.Item.ItemName;
            m_DetailPanelDetails.text = itemSlot.Item.ItemDetail;

            if (itemSlot.Item.TryGetComponent(out Weapon weapon))
            {
                m_DetailPanelFirstAbility.gameObject.SetActive(true);
                m_DetailPanelFirstAbility.text = weapon.PrimaryAbility.AbilityName + " : \n" + weapon.PrimaryAbility.AbilityDescription;
                m_DetailPanelSecondAbility.gameObject.SetActive(true);
                m_DetailPanelSecondAbility.text = weapon.SecondaryAbility.AbilityName + " : \n" + weapon.SecondaryAbility.AbilityDescription;
            }
            else if (itemSlot.Item.TryGetComponent(out Potion potion))
            {
                m_DetailPanelFirstAbility.gameObject.SetActive(true);
                m_DetailPanelFirstAbility.text = potion.PotionAbility.AbilityName + " : \n" + potion.PotionAbility.AbilityDescription;
                m_DetailPanelSecondAbility.gameObject.SetActive(false);
            }

            m_ContextualMenu.transform.position = m_InventorySlots[index].transform.position + offset;
            m_ContextualMenu.GetComponent<RectTransform>().SetAsLastSibling();
            m_ContextualMenu.SetActive(true);
        }
    }

    private void HideContextualMenu()
    {
        m_ContextualMenu.SetActive(false);
    }

    private void FilterInventoryByType(ItemType type)
    {
        m_CurrentFilter = type;

        ChangeCurrentIconColor(type);

        if (type == ItemType.All)
            m_FilteredItemSlots = m_ItemSlots;
        else
            m_FilteredItemSlots = m_ItemSlots.FindAll(items => items.Item.ItemType == type);

        UpdateShownItems();
    }

    public void ChangeEquipmentIconColorByType(ItemType type)
    {
        m_ToggleGroup.SetAllTogglesOff();

        switch (type)
        {
            case ItemType.Armor:
                m_ArmorSlot.ChangeIconColor(true);
                break;
            case ItemType.Weapon:
                foreach (EquipmentSlot slot in m_WeaponSlots)
                {
                    slot.ChangeIconColor(true);
                }
                break;
            case ItemType.Belt:
                foreach (EquipmentSlot slot in m_BeltSlots)
                {
                    slot.ChangeIconColor(true);
                }
                break;
        }
    }

    public void ResetEquipmentIconColor()
    {
        m_ArmorSlot.ChangeIconColor(false);

        foreach (EquipmentSlot slot in m_WeaponSlots)
        {
            slot.ChangeIconColor(false);
        }

        foreach (EquipmentSlot slot in m_BeltSlots)
        {
            slot.ChangeIconColor(false);
        }
    }

    private void ChangeCurrentIconColor(ItemType type)
    {
        foreach (GameObject go in m_InventoryFilters)
        {
            if (go.TryGetComponent(out Image image))
            {
                if (go.name == type.ToString())
                {
                    image.color = m_ColorSelectedFilter;
                }
                else
                {
                    image.color = m_ColorDefaultFilter;
                }
            }
        }
    }

    private void ChangeToggleColor(GameObject go, int index, bool isOn)
    {
        ColorBlock cb = go.GetComponent<Toggle>().colors;
        if (isOn)
        {
            cb.selectedColor = m_ColorSelectedFilter;
            ShowContextualMenu(index);
        }
        else
        {
            cb.selectedColor = Color.white;
            HideContextualMenu();
        }
        go.GetComponent<Toggle>().colors = cb;
    }

    public RectTransform GetEquipmentPanel(ItemType type)
    {
        RectTransform rect = new RectTransform();

        switch (type)
        {
            case ItemType.Armor:
                rect = m_ArmorPanel.GetComponent<RectTransform>();
                break;
            case ItemType.Weapon:
                rect = m_WeaponPanel.GetComponent<RectTransform>();
                break;
            case ItemType.Belt:
                rect = m_BeltPanel.GetComponent<RectTransform>();
                break;
        }

        return rect;
    }
}