using UnityEngine;

public class ProjectileAbility : Ability, IProjectileAbility
{
    [SerializeField] private bool m_HasProjectileAtIdle;
    [SerializeField] private float m_ProjectileSpeed;
    [SerializeField] private GameObject m_Projectile;
    [SerializeField] private Transform m_ProjectileSpawner;
    [SerializeField] private AudioClip m_FireProjectileAudio;
    [SerializeField] private AudioClip m_CreateProjectileAudio;

    public AudioClip FireProjectileAudio => m_FireProjectileAudio;
    public AudioClip CreateProjectileAudio => m_CreateProjectileAudio;
    public bool HasProjectileAtIdle => m_HasProjectileAtIdle;
    public GameObject Projectile => m_Projectile;
    public float ProjectileSpeed => m_ProjectileSpeed;
    public Transform ProjectileSpawner => m_ProjectileSpawner;
    public Projectile CurrentProjectile => m_CurrentProjectile;

    private Projectile m_CurrentProjectile;

    public override void InitAbility(GameObject owner)
    {
        //Debug.Log("ProjectileAbility InitAbility");
        base.InitAbility(owner);
        if (m_HasProjectileAtIdle)
        {
            m_PlayerAnimator.OnCreateProjectile += CreateProjectile;
            m_PlayerAnimator.OnCreateProjectileAudio += CreateProjectilePlayAudio;
        }
        m_AbilityAnimationData.AbilityAnimatorParameterName = GetType().Name;
        m_AbilityAnimationData.AbilityAnimatorParameterType = AnimatorControllerParameterType.Trigger;
    }

    public override void AbilityStart()
    {
        //Debug.Log("ProjectileAbility AbilityStart");
        base.AbilityStart();
        m_PlayerAnimator.StartAnimation(m_AbilityAnimationData, true);
        m_PlayerAnimator.OnCreateProjectile += CreateProjectile;
        m_PlayerAnimator.OnFireProjectile += FireProjectile;
        m_PlayerAnimator.OnCreateProjectileAudio += CreateProjectilePlayAudio;
        m_PlayerAnimator.OnFireProjectileAudio += FireProjectilePlayAudio;
    }

    public void CreateProjectile()
    {
        //Debug.Log("ProjectileAbility CreateProjectile");
        m_PlayerAnimator.OnCreateProjectile -= CreateProjectile;
        GameObject go = Instantiate(m_Projectile, m_ProjectileSpawner.position, m_ProjectileSpawner.rotation);
        m_CurrentProjectile = go.GetComponent<Projectile>();
        m_CurrentProjectile.InitProjectile(m_Owner, m_AbilityAmount, m_ProjectileSpeed);
    }

    public void CreateProjectilePlayAudio()
    {
        m_PlayerAnimator.OnCreateProjectileAudio -= CreateProjectilePlayAudio;

        m_SFXManager.PlayAudioOnce(m_CreateProjectileAudio);
    }

    public void FireProjectile()
    {
        //Debug.Log("ProjectileAbility FireProjectile");
        m_PlayerAnimator.OnFireProjectile -= FireProjectile;

        if (m_CurrentProjectile == null) return;
        m_CurrentProjectile.FireProjectile();
    }

    public void FireProjectilePlayAudio()
    {
        m_PlayerAnimator.OnFireProjectileAudio -= FireProjectilePlayAudio;

        m_SFXManager.PlayAudioOnce(m_FireProjectileAudio);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        if (m_CurrentProjectile != null) Destroy(m_CurrentProjectile.gameObject);
        m_PlayerAnimator.OnCreateProjectile -= CreateProjectile;
        m_PlayerAnimator.OnFireProjectile -= FireProjectile;
        m_PlayerAnimator.OnCreateProjectileAudio -= CreateProjectilePlayAudio;
        m_PlayerAnimator.OnFireProjectileAudio -= FireProjectilePlayAudio;
    }
}
