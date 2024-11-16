using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    protected Animator m_Animator;

    // Ability Animation Events
    public event Action OnAbilityEnd;
    public event Action OnAbilityCharge;
    public event Action OnPlayAbilitySound;
    public event Action OnAbilityNextChain;
    public event Action OnCheckAbilityHit;
    public event Action OnFireProjectile;
    public event Action OnCreateProjectile;
    public event Action OnAbilityChargeAudio;
    public event Action OnFireProjectileAudio;
    public event Action OnCreateProjectileAudio;

    // Equip Item Animation Events
    public event Action AnimationEquipEnded;
    public event Action AnimationEquipAddItem;
    public event Action AnimationUnEquipEnded;
    public event Action AnimationUnEquipRemoveItem;

    public Animator PAnimator => m_Animator;

    private Camera m_Camera;
    private Vector3 m_BasePosition;
    private Vector3 m_BlockPosition;
    private Vector3 m_LieOnBackPosition;
    private Vector3 m_CameraBasePosition;
    private bool m_Block;
    private float m_BlockDuration;

    private void Awake()
    {
        m_BasePosition = transform.localPosition;
        m_BlockPosition = transform.localPosition + new Vector3(0f, 0.01f, -0.5f);
        m_LieOnBackPosition = new Vector3(0f, -0.2f, 0f);
        m_Animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_Camera = Camera.main;
        m_CameraBasePosition = m_Camera.transform.localPosition;
    }

    private void Update()
    {
        if (m_Block)
        {
            m_BlockDuration += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(m_BasePosition, m_BlockPosition, m_BlockDuration);
        }
    }

    public void StartAnimation(AbilityAnimationData data, bool value)
    {
        switch (data.AbilityAnimatorParameterType)
        {
            case AnimatorControllerParameterType.Bool:
                SetBool(data.AbilityAnimatorParameterName, value);
                break;
            case AnimatorControllerParameterType.Trigger:
                SetTrigger(data.AbilityAnimatorParameterName);
                break;
        }
    }

    public void SetLayer(string layer, int weight)
    {
        int layerIndex = m_Animator.GetLayerIndex(layer);
        m_Animator.SetLayerWeight(layerIndex, weight);
    }

    public void SetBool(string key, bool value)
    {
        m_Animator.SetBool(key, value);
    }

    public void SetTrigger(string key)
    {
        m_Animator.SetTrigger(key);
    }

    //******* Ability Animation Events *******//
    public void AbilityEnd()
    {
        OnAbilityEnd?.Invoke();
    }

    public void AbilityCharge()
    {
        OnAbilityCharge?.Invoke();
    }

    public void AbilityNextChain()
    {
        OnAbilityNextChain?.Invoke();
    }

    public void CheckAbilityHit()
    {
        OnCheckAbilityHit?.Invoke();
    }

    public void CreateProjectile()
    {
        OnCreateProjectile?.Invoke();
    }

    public void FireProjectile()
    {
        OnFireProjectile?.Invoke();
    }

    public void PlaySound()
    {
        OnPlayAbilitySound?.Invoke();
    }

    public void PlayChargeAudio()
    {
        OnAbilityChargeAudio?.Invoke();
    }

    public void PlayCreateProjectileAudio()
    {
        OnCreateProjectileAudio?.Invoke();
    }

    public void PlayFireProjectileAudio()
    {
        OnFireProjectileAudio?.Invoke();
    }

    //******* Equip Item Animation Events *******//
    public void EquipAnimationEnded()
    {
        AnimationEquipEnded?.Invoke();
        Debug.Log("EquipAnimationEnded");
        GameManager.Instance.PlayerManager.PlayerAction.IsActing = false;
    }

    public void EquipAnimationAddItemReady()
    {
        AnimationEquipAddItem?.Invoke();
    }

    public void UnEquipAnimationEnded()
    {
        AnimationUnEquipEnded?.Invoke();
    }

    public void UnEquipAnimationRemoveItemReady()
    {
        AnimationUnEquipRemoveItem?.Invoke();
    }

    //******* Special Animations *******//
    public void BlockedHitAnimation()
    {
        m_BlockDuration = 0f;
        m_Block = true;
        StartCoroutine(Block());
    }

    private IEnumerator Block()
    {
        yield return new WaitForSeconds(0.05f);

        m_Block = false;
    }

    public void FallOnBackAndLookAt(Transform transform)
    {
        GameManager.Instance.PlayerManager.CanMove = false;
        GameManager.Instance.PlayerManager.CanLook = false;
        GameManager.Instance.PlayerManager.PlayerCharacterController.detectCollisions = false;
        m_Camera.transform.LookAt(transform.position + Vector3.up);
        StartCoroutine(FallCoroutine(true));
    }

    public void StandUpFromBackFall()
    {
        GameManager.Instance.PlayerManager.CanMove = true;
        GameManager.Instance.PlayerManager.PlayerCharacterController.detectCollisions = true;
        m_Camera.transform.LookAt(m_Camera.transform.position + Vector3.forward);
        StartCoroutine(FallCoroutine(false));
    }

    private IEnumerator FallCoroutine(bool fall)
    {
        float duration = 0f;

        while (duration < 1f)
        {
            m_Camera.transform.localPosition = Vector3.MoveTowards(m_Camera.transform.localPosition, fall ? m_LieOnBackPosition : m_CameraBasePosition, duration);

            duration += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        if (fall) GameManager.Instance.PlayerManager.CanLook = true;
    }
}
