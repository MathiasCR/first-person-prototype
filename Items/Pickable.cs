using TMPro;
using UnityEngine;

public class Pickable : MonoBehaviour
{
    [SerializeField] private int m_Nbr = 1;
    [SerializeField] private Item m_Item;
    [SerializeField] private Canvas m_ItemCanvas;
    [SerializeField] private TextMeshProUGUI m_ItemNameText;

    private Camera m_UICamera;

    private void Start()
    {
        m_ItemNameText.text = m_Item.ItemName;
        m_ItemNameText.enabled = false;
        m_UICamera = GameManager.Instance.PlayerManager.UICamera;
        m_ItemCanvas.worldCamera = m_UICamera;
    }

    private void LateUpdate()
    {
        if (m_ItemNameText.enabled)
        {
            m_ItemNameText.transform.LookAt(m_ItemNameText.transform.position + m_UICamera.transform.rotation * Vector3.forward, m_UICamera.transform.rotation * Vector3.up);
        }
    }

    public void PickItem()
    {
        GameManager.Instance.PlayerManager.PlayerInventory.AddItemToInventory(m_Item, m_Nbr);
        Destroy(gameObject);
    }

    public void ShowItemName(bool show)
    {
        m_ItemNameText.enabled = show;
    }
}
