using UnityEngine;

public interface IResource
{
    float Max { get; }
    float Current { get; }
    float RegenRate { get; }
    float RegenDelay { get; }
    bool ResetOnStart { get; }
    void Add(float amount, GameObject origin);
    void Remove(float amount, GameObject origin);
    void Regen();
    bool CanUse(float cost);
}

public enum ResourceType
{
    Health,
    Mana,
    Stamina
}