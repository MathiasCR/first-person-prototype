using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_RunStaminaCost = 1.0f;
    [SerializeField] private float m_UpDownRange = 70.0f;
    [SerializeField] private float m_Sensitivity = 75.0f;
    [SerializeField] private float m_JumpHeight = 1f;
    [SerializeField] private float m_JumpStaminaCost = 15.0f;
    [SerializeField] private HeadBobSystem m_HeadBobSystem;

    public const float WalkSpeed = 4.0f;
    public const float CrouchSpeed = 2.0f;
    public const float RunCrouchSpeed = 3.0f;
    public const float RunSpeed = 8.0f;
    public const float FallDamageThreshold = 10f;

    public float CurrentRunSpeed { get => m_RunSpeed; set => m_RunSpeed = value; }
    public float CurrentWalkSpeed { get => m_WalkSpeed; set => m_WalkSpeed = value; }
    public float CurrentCrouchSpeed { get => m_CrouchSpeed; set => m_CrouchSpeed = value; }
    public float CurrentRunCrouchSpeed { get => m_RunCrouchSpeed; set => m_RunCrouchSpeed = value; }

    public bool IsMoving => m_IsMoving;
    public bool IsRunning => m_IsRunning;
    public bool IsCrouched => m_IsCrouched;

    private PlayerManager m_PlayerManager;

    private Vector2 m_MoveDir;
    private Vector2 m_RotateDir;
    private float m_StartFallHeight;
    private bool m_IsMoving = false;
    private bool m_IsRunning = false;
    private bool m_IsFalling = false;
    private bool m_IsCrouched = false;
    private Coroutine m_CrouchingCoroutine;

    private float m_RunSpeed = RunSpeed;
    private float m_WalkSpeed = WalkSpeed;
    private float m_CrouchSpeed = CrouchSpeed;
    private float m_RunCrouchSpeed = RunCrouchSpeed;

    private float m_RotationY = 0.0f;
    private float m_GroundedTimer;
    private float m_PlayerVelocityY;

    private float m_CameraPositionStandUp = 1.04f;
    private float m_CameraPositionCrouched = 0.5f;
    private float m_HitBoxHeightStandUp = 2.0f;
    private float m_HitBoxHeightCrounched = 1.0f;
    private float m_HitBoxCenterStandUp = 0.6f;
    private float m_HitBoxCenterCrounched = 0f;

    private void Awake()
    {
        m_PlayerManager = GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        bool isGrounded = m_PlayerManager.PlayerCharacterController.isGrounded;

        // ROTATE //
        if (m_PlayerManager.CanLook && m_RotateDir != Vector2.zero)
        {
            float rotY = m_RotateDir.y * m_Sensitivity * Time.deltaTime;
            float rotX = m_RotateDir.x * m_Sensitivity * Time.deltaTime;

            //RotY
            m_RotationY -= rotY;
            m_RotationY = Mathf.Clamp(m_RotationY, -m_UpDownRange, m_UpDownRange);
            m_PlayerManager.PlayerCamera.transform.localRotation = Quaternion.Euler(m_RotationY, 0, 0);

            //RotX
            transform.Rotate(Vector3.up * rotX);
        }

        // MOVE //
        Vector3 move = new Vector3(m_MoveDir.x, 0, m_MoveDir.y);

        if (!m_PlayerManager.CanMove) move = Vector3.zero;

        if (move != Vector3.zero)
        {
            m_IsMoving = true;

            if (isGrounded)
            {
                float headbobFrequency = m_IsCrouched ? m_IsRunning ? m_HeadBobSystem.CrounchedRunFrequency
                : m_HeadBobSystem.CrounchedFrequency
                : m_IsRunning ? m_HeadBobSystem.RunFrequency
                : m_HeadBobSystem.WalkFrequency;
                m_HeadBobSystem.StartHeadBob(headbobFrequency);
            }
        }
        else
        {
            m_IsMoving = false;
        }

        float speed = m_IsCrouched ? m_IsRunning ? m_RunCrouchSpeed : m_CrouchSpeed : m_IsRunning ? m_RunSpeed : m_WalkSpeed;
        move *= speed;
        move = transform.TransformDirection(move);

        // RUNNING //
        if (m_IsRunning && move != Vector3.zero)
        {
            m_PlayerManager.PlayerStamina.Remove(m_RunStaminaCost * Time.deltaTime, gameObject);
        }

        if (m_PlayerManager.PlayerStamina.Current == 0f && m_IsRunning) m_IsRunning = false;

        // JUMPING //
        if (isGrounded)
        {
            // cooldown interval to allow reliable jumping even whem coming down ramps
            m_GroundedTimer = 0.2f;

            if (m_IsFalling)
            {
                m_IsFalling = false;
                float fallDamage = CalculateFallDamage(m_StartFallHeight, transform.position.y, FallDamageThreshold);
                if (fallDamage > 0f) m_PlayerManager.PlayerHealth.Remove(fallDamage, gameObject);
            }
        }
        else if (!m_IsFalling)
        {
            m_IsFalling = true;
            m_StartFallHeight = transform.position.y;
        }

        if (m_GroundedTimer > 0)
        {
            m_GroundedTimer -= Time.deltaTime;
        }

        // slam into the ground
        if (isGrounded && m_PlayerVelocityY < 0)
        {
            // hit ground
            m_PlayerVelocityY = 0f;
        }

        // apply gravity always, to let us track down ramps properly
        m_PlayerVelocityY -= GameManager.GRAVITY * Time.deltaTime;

        // inject Y velocity before we use it
        move.y = m_PlayerVelocityY;

        m_PlayerManager.PlayerCharacterController.Move(move * Time.deltaTime);
    }

    public void SetMovement(Vector2 movement)
    {
        m_MoveDir = movement;
    }

    public void SetRotation(Vector2 rotation)
    {
        m_RotateDir = rotation;
    }

    public void Run(bool isRunning)
    {
        if (isRunning && m_PlayerManager.PlayerStamina.Current > 0)
        {
            m_IsRunning = true;
        }
        else
        {
            m_IsRunning = false;
        }
    }

    public void Jump()
    {
        // must have been grounded recently to allow jump
        if (m_GroundedTimer > 0 && m_PlayerManager.PlayerStamina.CanUse(m_JumpStaminaCost))
        {
            // no more until we recontact ground
            m_GroundedTimer = 0;

            // Physics dynamics formula for calculating jump up velocity based on height and gravity
            m_PlayerVelocityY += Mathf.Sqrt(m_JumpHeight * 2 * GameManager.GRAVITY);
            m_PlayerManager.PlayerStamina.Remove(m_JumpStaminaCost, gameObject);
        }
    }

    public void Crouch()
    {
        if (Physics.Raycast(transform.position, Vector3.up, 1.6f) || !m_PlayerManager.PlayerCharacterController.isGrounded)
        {
            return;
        }

        m_IsCrouched = !m_IsCrouched;
        m_PlayerManager.PlayerCharacterController.height = m_IsCrouched ? m_HitBoxHeightCrounched : m_HitBoxHeightStandUp;
        m_PlayerManager.PlayerCharacterController.center = new Vector3(0f, m_IsCrouched ? m_HitBoxCenterCrounched : m_HitBoxCenterStandUp, 0f);

        if (m_CrouchingCoroutine != null) StopCoroutine(m_CrouchingCoroutine);

        m_CrouchingCoroutine = StartCoroutine(Crouching());
    }

    private IEnumerator Crouching()
    {
        Vector3 newPosition = new Vector3(0, m_IsCrouched ? m_CameraPositionCrouched : m_CameraPositionStandUp, 0);

        while (m_PlayerManager.PlayerCamera.transform.localPosition != newPosition)
        {
            m_PlayerManager.PlayerCamera.transform.localPosition =
                    Vector3.Lerp(m_PlayerManager.PlayerCamera.transform.localPosition, newPosition, Time.deltaTime * 10);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private float CalculateFallDamage(float startHeight, float endHeight, float velocityThreshold)
    {
        float fallDistance = startHeight - endHeight;

        // Calcul de la vitesse d'impact
        float impactVelocity = Mathf.Sqrt(2 * GameManager.GRAVITY * fallDistance);

        // Si la vitesse est inférieure au seuil, pas de dégâts
        if (impactVelocity < velocityThreshold)
        {
            return 0f;
        }

        // Calcul des dégâts en fonction de la vitesse
        float damage = (impactVelocity - velocityThreshold) * 7f;
        return damage;
    }
}
