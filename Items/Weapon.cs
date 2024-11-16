using UnityEngine;

public class Weapon : Equippable
{
    [SerializeField] protected Ability m_PrimaryAbility;
    [SerializeField] protected Ability m_SecondaryAbility;
    [SerializeField] protected WeaponType m_WeaponType;

    public WeaponType WeaponType { get => m_WeaponType; }
    public Ability PrimaryAbility { get => m_PrimaryAbility; }
    public Ability SecondaryAbility { get => m_SecondaryAbility; }

    private void Start()
    {
        m_PrimaryAbility.InitAbility(m_Owner);
        m_SecondaryAbility.InitAbility(m_Owner);
    }

    public override string GetSubtype()
    {
        return m_WeaponType.ToString();
    }

    public Projectile GetProjectile()
    {
        if (m_PrimaryAbility is IProjectileAbility && ((IProjectileAbility)m_PrimaryAbility).CurrentProjectile != null)
        {
            return ((IProjectileAbility)m_PrimaryAbility).CurrentProjectile;
        }
        else if (m_SecondaryAbility is IProjectileAbility && ((IProjectileAbility)m_SecondaryAbility).CurrentProjectile != null)
        {
            return ((IProjectileAbility)m_SecondaryAbility).CurrentProjectile;
        }

        return null;
    }
}

public enum WeaponType
{
    Sword_2H,
    OneHandedSword,
    Wand,
    Bow
}