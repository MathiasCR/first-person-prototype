using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private Image m_DeathScreen;
    [SerializeField] private Image m_ScreenDamage;
    [SerializeField] private Slider m_HealthSlider;
    [SerializeField] private Slider m_ManaSlider;
    [SerializeField] private Slider m_StaminaSlider;
    [SerializeField] private List<GameObject> m_BeltSlots;
    [SerializeField] private List<GameObject> m_WeaponSlots;

    private float _damageFeedbackDuration = 0.2f;

    public void SetHealth(float maxHealth, float currentHealth)
    {
        m_HealthSlider.maxValue = maxHealth;
        m_HealthSlider.value = currentHealth;
    }

    public void SetMana(float maxMana, float currentMana)
    {
        m_ManaSlider.maxValue = maxMana;
        m_ManaSlider.value = currentMana;
    }

    public void SetStamina(float maxStamina, float currentStamina)
    {
        m_StaminaSlider.maxValue = maxStamina;
        m_StaminaSlider.value = currentStamina;
    }

    public void UpdateHealth(float currentHealth)
    {
        m_HealthSlider.value = currentHealth;
    }

    public void UpdateMana(float currentMana)
    {
        m_ManaSlider.value = currentMana;
    }

    public void UpdateStamina(float currentStamina)
    {
        m_StaminaSlider.value = currentStamina;
    }

    public void DamageFeedback()
    {
        m_ScreenDamage.gameObject.SetActive(true);

        StartCoroutine(DamageFeedbackCooldown());
    }

    public void SetDeathScreen(bool active)
    {
        m_DeathScreen.gameObject.SetActive(active);
    }

    public void UpdateItemInSlot(Sprite icon, ItemType type, int index, bool add)
    {
        Debug.Log("update : " + type + " - " + index + " - " + add);
        Image img = null;
        switch (type)
        {
            case ItemType.Weapon:
                img = m_WeaponSlots[index].GetComponent<Image>();
                break;
            case ItemType.Belt:
                img = m_BeltSlots[index].GetComponent<Image>();
                break;
        }

        if (img == null) return;

        img.sprite = add ? icon : null;
        Color color = img.color;
        color.a = add ? 1f : 0f;
        img.color = color;
    }

    public void ResetItemSlot()
    {
        foreach (GameObject go in m_WeaponSlots)
        {
            go.GetComponent<RectTransform>().localScale = Vector3.one;
        }

        foreach (GameObject go in m_BeltSlots)
        {
            go.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    public void EquipItemInSlot(ItemType type, int index)
    {
        ResetItemSlot();

        RectTransform rectTransform = null;
        switch (type)
        {
            case ItemType.Weapon:
                rectTransform = m_WeaponSlots[index].GetComponent<RectTransform>();
                break;
            case ItemType.Belt:
                rectTransform = m_BeltSlots[index].GetComponent<RectTransform>();
                break;
        }

        if (rectTransform == null) return;

        rectTransform.localScale = new Vector3(1.5f, 1.5f, 1f);
    }

    private IEnumerator DamageFeedbackCooldown()
    {
        float delay = 0f;

        while (delay < _damageFeedbackDuration)
        {
            delay += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        m_ScreenDamage.gameObject.SetActive(false);
    }
}
