using UnityEngine;

[RequireComponent(typeof(Health), typeof(Stamina), typeof(Mana))]
public class PlayerManager : MonoBehaviour, IResourceModifier
{
    [SerializeField] private Camera m_Camera;
    [SerializeField] private Camera m_UICamera;
    [SerializeField] private PlayerHUD m_PlayerHUD;
    [SerializeField] private InventoryUI m_PlayerInventoryUI;
    [SerializeField] private PlayerAnimator m_PlayerAnimator;
    [SerializeField] private CharacterController m_CharacterController;

    [SerializeField] private Transform m_LeftHand;
    [SerializeField] private Transform m_WeaponPlaceHolder;

    public bool CanAct { get => m_CanAct; }
    public bool CanLook { get => m_CanLook; set => m_CanLook = value; }
    public bool CanMove { get => m_CanMove; set => m_CanMove = value; }
    public bool PlayedDead { get => m_PlayedDead; }
    public bool InventoryOpened { get => m_InventoryOpened; }

    public Camera UICamera => m_UICamera;
    public Camera PlayerCamera => m_Camera;
    public Mana PlayerMana => m_PlayerMana;
    public Transform LeftHand => m_LeftHand;
    public PlayerHUD PlayerHUD => m_PlayerHUD;
    public GameObject ModifierGO => gameObject;
    public Health PlayerHealth => m_PlayerHealth;
    public Stamina PlayerStamina => m_PlayerStamina;
    public PlayerAction PlayerAction => m_PlayerAction;
    public InventoryUI InventoryUI => m_PlayerInventoryUI;
    public PlayerAnimator PlayerAnimator => m_PlayerAnimator;
    public PlayerInventory PlayerInventory => m_PlayerInventory;
    public PlayerMovement PlayerMovement => m_PlayerMovement;
    public Transform WeaponPlaceHolder => m_WeaponPlaceHolder;
    public CharacterController PlayerCharacterController => m_CharacterController;

    private bool m_CanAct;
    private bool m_CanLook;
    private bool m_CanMove;
    private bool m_PlayedDead;
    private bool m_InventoryOpened;

    private Mana m_PlayerMana;
    private Health m_PlayerHealth;
    private Stamina m_PlayerStamina;
    private PlayerAction m_PlayerAction;
    private PlayerMovement m_PlayerMovement;
    private PlayerInventory m_PlayerInventory;

    private void Awake()
    {
        m_PlayerMana = GetComponent<Mana>();
        m_PlayerHealth = GetComponent<Health>();
        m_PlayerStamina = GetComponent<Stamina>();
        m_PlayerAction = GetComponent<PlayerAction>();
        m_PlayerMovement = GetComponent<PlayerMovement>();
        m_PlayerInventory = GetComponent<PlayerInventory>();

        m_CanAct = false;
        m_CanMove = false;
        m_CanLook = false;
        m_PlayedDead = false;
        m_InventoryOpened = false;

        m_PlayerInventory.OnInHandsRemoved += m_PlayerHUD.ResetItemSlot;
        m_PlayerInventory.OnInHandsUpdated += SetEquippedUI;
        m_PlayerInventory.OnBeltItemUpdated += SetEquipmentUI;
        m_PlayerInventory.OnWeaponItemUpdated += SetEquipmentUI;
    }

    private void Start()
    {
        m_PlayerHUD.SetMana(m_PlayerMana.Max, m_PlayerMana.Current);
        m_PlayerHUD.SetHealth(m_PlayerHealth.Max, m_PlayerHealth.Current);
        m_PlayerHUD.SetStamina(m_PlayerStamina.Max, m_PlayerStamina.Current);
    }

    private void Update()
    {
        if (m_InventoryOpened)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void SetEquipmentUI(Item item, int stack, int index)
    {
        m_PlayerHUD.UpdateItemInSlot(item.ItemIcon, item.ItemType, index, stack > 0);
    }

    public void SetEquippedUI(Item item, int index)
    {
        m_PlayerHUD.EquipItemInSlot(item.ItemType, index);
    }

    public void ChangeInventoryState()
    {
        m_InventoryOpened = !m_InventoryOpened;
        m_PlayerInventoryUI.ShowInventory(m_InventoryOpened);
    }

    public void ChangeControls(bool enable)
    {
        m_CanAct = enable;
        m_CanMove = enable;
        m_CanLook = enable;
    }

    public void PlayerDead()
    {
        m_PlayedDead = true;
        m_PlayerMana.StopRegen = true;
        m_PlayerHealth.StopRegen = true;
        m_PlayerStamina.StopRegen = true;

        ChangeControls(false);

        PlayerAnimator.SetTrigger("Die");
        PlayerHUD.SetDeathScreen(true);

        if (m_Camera.TryGetComponent(out Collider cameraCollider) && m_Camera.TryGetComponent(out Rigidbody cameraRigidbody))
        {
            cameraCollider.enabled = true;
            cameraRigidbody.useGravity = true;
        }

        if (m_PlayerInventory.ItemInHands == null) return;

        if (m_PlayerInventory.ItemInHands.TryGetComponent(out Collider itemCollider) && m_PlayerInventory.ItemInHands.TryGetComponent(out Rigidbody itemRigidbody))
        {
            itemCollider.enabled = true;
            itemRigidbody.useGravity = true;
        }

        if (m_PlayerInventory.ItemInHands.TryGetComponent(out Weapon weapon) && weapon.GetProjectile() != null
            && weapon.GetProjectile().TryGetComponent(out Collider weaponCollider)
            && weapon.GetProjectile().TryGetComponent(out Rigidbody weaponRigidbody))
        {
            weaponCollider.enabled = true;
            weaponRigidbody.useGravity = true;
        }

    }
}
