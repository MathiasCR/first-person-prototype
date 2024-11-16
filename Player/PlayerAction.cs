using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerAction : MonoBehaviour
{
    public bool IsActing { get => m_IsActing; set => m_IsActing = value; }

    private PlayerManager m_PlayerManager;

    private bool m_IsActing = false;
    private Pickable m_activePickable;
    private Flammable m_activeFlammable;
    private Vector3 center = new Vector3(0.5f, 0.5f, 0);

    private void Awake()
    {
        m_PlayerManager = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        Ray ray = m_PlayerManager.PlayerCamera.ViewportPointToRay(center);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 2f, (1 << LayerMask.NameToLayer("Flammable")) | (1 << LayerMask.NameToLayer("Pickable"))))
        {
            // A pickable object is in front of the player
            if (hit.transform.TryGetComponent(out Pickable pickable))
            {
                m_activeFlammable = null;
                if (m_activePickable != null && m_activePickable != pickable)
                {
                    m_activePickable.ShowItemName(false);
                }

                m_activePickable = pickable;
                pickable.ShowItemName(true);
            }
            // A flammable object is in front of the player
            else if (hit.transform.TryGetComponent(out Flammable flammable))
            {
                //Debug.Log("flammable");
                m_activePickable = null;
                m_activeFlammable = flammable;
            }
        }
        else
        {
            if (m_activePickable != null) m_activePickable.ShowItemName(false);
            m_activePickable = null;
            m_activeFlammable = null;
        }
    }

    public void DoAction(bool isHolding, ActionType actionType)
    {
        if (m_PlayerManager.PlayerInventory.ItemInHands == null) return;

        //Action for a weapon
        if (m_PlayerManager.PlayerInventory.ItemInHands.TryGetComponent(out Weapon weapon))
        {
            //Debug.Log("Do action : " + !m_IsOccupied);
            Ability ability = GetAbilityByActionType(weapon, actionType);
            // Return si le joueur effectue déjà une action et que la nouvelle ne peut pas override
            if (ability == null
                || m_IsActing
                || isHolding
                || !ability.CanCastAbility()) return;

            m_IsActing = true;
            ability.AbilityStart();
        }

        //Action for a potion
        if (m_PlayerManager.PlayerInventory.ItemInHands.TryGetComponent(out Potion potion) && actionType == ActionType.Primary)
        {
            m_IsActing = true;
            potion.PotionAbility.AbilityStart();
        }

        //Action for the Torch
        if (m_PlayerManager.PlayerInventory.ItemInHands.TryGetComponent(out Torch torch) && actionType == ActionType.Primary)
        {
            if (m_activeFlammable != null && !m_activeFlammable.IsActive)
            {
                m_IsActing = true;
                torch.OnLightEnded += () => m_IsActing = false;
                torch.StartLight(m_activeFlammable);
            }
        }
    }

    public void ReleaseAction(ActionType actionType)
    {
        if (m_PlayerManager.PlayerInventory.ItemInHands != null && m_PlayerManager.PlayerInventory.ItemInHands.TryGetComponent(out Weapon weapon))
        {
            Ability ability = GetAbilityByActionType(weapon, actionType);

            if (ability.GetType().IsSubclassOf(typeof(ChargeableAbility)))
            {
                ability.GetComponent<ChargeableAbility>().AbilityRelease();
            }
            else if (ability.GetType() == typeof(BlockAbility))
            {
                ability.GetComponent<BlockAbility>().AbilityRelease();
            }

        }
    }

    public void Interaction()
    {
        if (m_activePickable != null)
        {
            m_activePickable.PickItem();
        }

        if (m_activeFlammable != null && m_activeFlammable.IsActive)
        {
            m_activeFlammable.Interact();
        }
    }

    public Ability GetAbilityByActionType(Weapon weapon, ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.Primary:
                return weapon.PrimaryAbility;
            case ActionType.Secondary:
                return weapon.SecondaryAbility;
            default: return null;
        }
    }
}

public enum ActionType
{
    Primary,
    Secondary,
    Interaction
}
