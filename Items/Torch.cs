using System;

public class Torch : Equippable
{
    public event Action OnLightEnded;

    private Flammable m_activeFlammable;
    private PlayerAnimator m_PlayerAnimator;

    public void Start()
    {
        m_activeFlammable = null;
        m_PlayerAnimator = m_Owner.GetComponentInChildren<PlayerAnimator>();
    }

    public override string GetSubtype()
    {
        return BeltType.Torch.ToString();
    }

    public void StartLight(Flammable flammable)
    {
        m_activeFlammable = flammable;
        m_PlayerAnimator.SetTrigger(GetType().Name);
        m_PlayerAnimator.OnAbilityEnd += LightObject;
    }

    public void LightObject()
    {
        m_PlayerAnimator.OnAbilityEnd -= LightObject;
        m_activeFlammable.Interact();
        OnLightEnded?.Invoke();
    }
}
