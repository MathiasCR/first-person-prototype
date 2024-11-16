using System.Collections.Generic;
using UnityEngine;

public class ModifierManager : MonoBehaviour
{
    private List<Modifier> m_CurrentModifiers = new List<Modifier>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        List<Modifier> modifiersEnded = new List<Modifier>();
        List<ModifierType> typesToRefresh = new List<ModifierType>();
        m_CurrentModifiers.ForEach(modifier =>
        {
            modifier.RemainingDuration -= Time.deltaTime;
            if (modifier.RemainingDuration <= 0)
            {
                modifiersEnded.Add(modifier);
                typesToRefresh.Add(modifier.Type);
            }
        });

        modifiersEnded.ForEach(modifier =>
        {
            m_CurrentModifiers.Remove(modifier);
        });

        typesToRefresh.ForEach(type =>
        {
            ApplyModifiersByType(type);
        });
    }

    public void AddModifier(Modifier modifier)
    {
        if (m_CurrentModifiers.Contains(modifier)) return;

        modifier.RemainingDuration = modifier.Duration;
        m_CurrentModifiers.Add(modifier);
        ApplyModifiersByType(modifier.Type);
    }

    private void ApplyModifiersByType(ModifierType modifierType)
    {
        switch (modifierType)
        {
            case ModifierType.Speed:
                ModifySpeed();
                break;
        }
    }

    private void ModifySpeed()
    {
        if (gameObject.TryGetComponent(out PlayerMovement playerMovement))
        {
            List<Modifier> speedModifiers = m_CurrentModifiers.FindAll(modifier => modifier.Type == ModifierType.Speed);
            List<Modifier> additiveModifiers = speedModifiers.FindAll(modifier => modifier.StatType == StatModifierType.Additive);
            List<Modifier> multiplicativeModifiers = speedModifiers.FindAll(modifier => modifier.StatType == StatModifierType.Multiplicative);

            playerMovement.CurrentRunSpeed = GetStatModified(PlayerMovement.RunSpeed, additiveModifiers, multiplicativeModifiers);
            playerMovement.CurrentWalkSpeed = GetStatModified(PlayerMovement.WalkSpeed, additiveModifiers, multiplicativeModifiers);
            playerMovement.CurrentCrouchSpeed = GetStatModified(PlayerMovement.CrouchSpeed, additiveModifiers, multiplicativeModifiers);
            playerMovement.CurrentRunCrouchSpeed = GetStatModified(PlayerMovement.RunCrouchSpeed, additiveModifiers, multiplicativeModifiers);
        }
    }

    private float GetStatModified(float baseStat, List<Modifier> additiveModifiers, List<Modifier> multiplicativeModifiers)
    {
        // 1. Appliquer les modificateurs additifs
        float additiveSum = 0;
        additiveModifiers.ForEach(mod => additiveSum += mod.Amount);
        float modifiedStat = baseStat + additiveSum;

        // 2. Appliquer les modificateurs multiplicatifs (cumulés)
        float multiplicativeFactor = 1;
        multiplicativeModifiers.ForEach(mod => multiplicativeFactor += mod.Amount);

        return modifiedStat * multiplicativeFactor;
    }
}
