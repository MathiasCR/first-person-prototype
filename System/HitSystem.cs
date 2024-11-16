using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class HitSystem : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float m_Volume;

    [SerializeField] private HitImpactProfile m_CurrentHitProfile;
    [SerializeField] private ParticleProfile m_CurrentParticleProfile;

    private AudioSource m_AudioSource;

    /// <summary>
    /// The Type of Material the player is standing on, by name. (Rock, Grass, etc - defined in the supplied FootProfile.)
    /// </summary>
    [HideInInspector] public SurfaceType CurrentImpactMaterialName;

    void Start() => m_AudioSource = GetComponent<AudioSource>();

    public void TriggerHitSystem(Collision collision)
    {
        MaterialTag tag = collision.collider.GetComponent<MaterialTag>();
        if (tag != null)
        {
            Debug.Log("Tag ok");
            CurrentImpactMaterialName = tag.MaterialType;

            ImpactSpec impactSpec = m_CurrentHitProfile.ImpactSpecifications.FirstOrDefault(spec => spec.MaterialName == CurrentImpactMaterialName);
            if (impactSpec != null)
            {
                Debug.Log("Audio ok");
                m_AudioSource.PlayOneShot(impactSpec.ImpactClip, m_Volume * impactSpec.VolumeMultiplier);
            }

            ParticleSpec particleSpec = m_CurrentParticleProfile.ParticleSpecifications.FirstOrDefault(spec => spec.MaterialName == CurrentImpactMaterialName);
            if (particleSpec != null)
            {
                Debug.Log("Particle ok");
                ContactPoint contactPoint = collision.contacts[0];
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, contactPoint.normal);
                foreach (ParticleSystem particleSystem in particleSpec.ParticleSystems)
                {
                    ParticleSystem ps = Instantiate(particleSystem, contactPoint.point, rot);
                    ps.Play();
                }
            }
        }

    }
}
