using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier", menuName = "Modifier/Modifier Type", order = 1)]
public class Modifier : ScriptableObject
{
    public string Name;
    public float Amount;
    public float Duration;
    public float RemainingDuration;
    public ModifierType Type;
    public StatModifierType StatType;
    public AudioClip SoundEffect;
    public ParticleSystem ParticleEffect;
}

public enum StatModifierType
{
    Additive,
    Multiplicative
}

public enum ModifierType
{
    Speed,
    Damage,
    Health,
    HealthRegen,
    Stamina,
    StaminaRegen,
    Mana,
    ManaRegen,
}
