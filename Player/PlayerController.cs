using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerManager), typeof(PlayerAction), typeof(PlayerMovement))]
public class PlayerController : MonoBehaviour
{
    public bool IsHoldingPrimary => m_IsHoldingPrimary;
    public bool IsHoldingSecondary => m_IsHoldingSecondary;
    public bool IsHoldingInteraction => m_IsHoldingInteraction;

    private bool m_IsHoldingPrimary = false;
    private bool m_IsHoldingSecondary = false;
    private bool m_IsHoldingInteraction = false;

    private PlayerManager m_PlayerManager;
    private PlayerAction m_PlayerAction;
    private PlayerMovement m_PlayerMovement;

    private void Awake()
    {
        m_PlayerManager = GetComponent<PlayerManager>();
        m_PlayerAction = GetComponent<PlayerAction>();
        m_PlayerMovement = GetComponent<PlayerMovement>();
    }

    public void OnMove(InputValue value)
    {
        if (!m_PlayerManager.CanAct || !m_PlayerManager.CanMove) return;

        Vector2 moveDir = value.Get<Vector2>();
        m_PlayerMovement.SetMovement(moveDir);
    }

    public void OnLook(InputValue value)
    {
        if (m_PlayerManager.InventoryOpened || !m_PlayerManager.CanAct || !m_PlayerManager.CanLook) return;

        Vector2 rotateDir = value.Get<Vector2>();
        m_PlayerMovement.SetRotation(rotateDir);
    }

    public void OnSprint(InputValue value)
    {
        if (!m_PlayerManager.CanAct || !m_PlayerManager.CanMove) return;

        if (value.isPressed)
        {
            m_PlayerMovement.Run(true);
        }
        else
        {
            m_PlayerMovement.Run(false);
        }
    }

    public void OnJump()
    {
        if (!m_PlayerManager.CanAct || !m_PlayerManager.CanMove) return;

        m_PlayerMovement.Jump();
    }

    public void OnCrouch()
    {
        if (!m_PlayerManager.CanAct || !m_PlayerManager.CanMove) return;

        m_PlayerMovement.Crouch();
    }

    public void OnPrimary(InputValue value)
    {
        if (m_PlayerManager.InventoryOpened || !m_PlayerManager.CanAct) return;

        if (value.isPressed)
        {
            m_PlayerAction.DoAction(m_IsHoldingPrimary, ActionType.Primary);
            m_IsHoldingPrimary = true;
        }
        else
        {
            m_IsHoldingPrimary = false;
            m_PlayerAction.ReleaseAction(ActionType.Primary);
        }
    }

    public void OnSecondary(InputValue value)
    {
        if (m_PlayerManager.InventoryOpened || !m_PlayerManager.CanAct) return;

        if (value.isPressed)
        {
            m_PlayerAction.DoAction(m_IsHoldingSecondary, ActionType.Secondary);
            m_IsHoldingSecondary = true;
        }
        else
        {
            m_IsHoldingSecondary = false;
            m_PlayerAction.ReleaseAction(ActionType.Secondary);
        }
    }

    public void OnInteraction(InputValue value)
    {
        if (m_PlayerManager.InventoryOpened || m_PlayerAction.IsActing || !m_PlayerManager.CanAct) return;

        if (value.isPressed)
        {
            m_IsHoldingInteraction = true;
            m_PlayerAction.Interaction();
        }
        else
        {
            m_IsHoldingInteraction = false;
        }
    }

    public void OnFirstWeapon()
    {
        if (m_PlayerManager.InventoryOpened || m_PlayerAction.IsActing || !m_PlayerManager.CanAct) return;

        Debug.Log("FirstWeapon Equip");
        GameManager.Instance.PlayerManager.PlayerAction.IsActing = true;
        m_PlayerManager.PlayerInventory.TakeItemInHands(ItemType.Weapon, 0);
    }

    public void OnSecondWeapon()
    {
        if (m_PlayerManager.InventoryOpened || m_PlayerAction.IsActing || !m_PlayerManager.CanAct) return;

        Debug.Log("SecondWeapon Equip");
        GameManager.Instance.PlayerManager.PlayerAction.IsActing = true;
        m_PlayerManager.PlayerInventory.TakeItemInHands(ItemType.Weapon, 1);
    }

    public void OnFirstBeltItem()
    {
        if (m_PlayerManager.InventoryOpened || m_PlayerAction.IsActing || !m_PlayerManager.CanAct) return;

        Debug.Log("FirstBelt Equip");
        GameManager.Instance.PlayerManager.PlayerAction.IsActing = true;
        m_PlayerManager.PlayerInventory.TakeItemInHands(ItemType.Belt, 0);
    }

    public void OnSecondBeltItem()
    {
        if (m_PlayerManager.InventoryOpened || m_PlayerAction.IsActing || !m_PlayerManager.CanAct) return;

        Debug.Log("SecondBelt Equip");
        GameManager.Instance.PlayerManager.PlayerAction.IsActing = true;
        m_PlayerManager.PlayerInventory.TakeItemInHands(ItemType.Belt, 1);
    }

    public void OnThirdBeltItem()
    {
        if (m_PlayerManager.InventoryOpened || m_PlayerAction.IsActing || !m_PlayerManager.CanAct) return;

        Debug.Log("ThirdBelt Equip");
        GameManager.Instance.PlayerManager.PlayerAction.IsActing = true;
        m_PlayerManager.PlayerInventory.TakeItemInHands(ItemType.Belt, 2);
    }

    public void OnFourthBeltItem()
    {
        if (m_PlayerManager.InventoryOpened || m_PlayerAction.IsActing || !m_PlayerManager.CanAct) return;

        Debug.Log("FourthBelt Equip");
        GameManager.Instance.PlayerManager.PlayerAction.IsActing = true;
        m_PlayerManager.PlayerInventory.TakeItemInHands(ItemType.Belt, 3);
    }

    public void OnFifthBeltItem()
    {
        if (m_PlayerManager.InventoryOpened || m_PlayerAction.IsActing || !m_PlayerManager.CanAct) return;

        Debug.Log("FifthBelt Equip");
        GameManager.Instance.PlayerManager.PlayerAction.IsActing = true;
        m_PlayerManager.PlayerInventory.TakeItemInHands(ItemType.Belt, 4);
    }

    public void OnInventory()
    {
        if (!m_PlayerManager.CanAct) return;

        m_PlayerManager.ChangeInventoryState();
    }
}
