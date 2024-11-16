using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootStepSystem : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float m_Volume;

    [SerializeField] private float m_FloorDistance;
    [SerializeField] private float m_DistancePerStep;

    [SerializeField] private float m_RunSpeed;
    [SerializeField] private float m_WalkSpeed;
    [SerializeField] private float m_CrouchSpeed;

    [SerializeField] private FootStepProfile m_CurrentFootProfile;

    /// <summary>
    /// Min & Max volume Variance
    /// </summary>
    public Vector2 RandomizedVolumeRange = new Vector2(.95f, 1.05f);
    /// <summary>
    /// Min and Max Pitch Variance
    /// </summary>
    public Vector2 RandomizedPitchRange = new Vector2(.95f, 1.05f);

    private AudioSource m_AudioSource;

    /// <summary>
    /// The Type of Material the player is standing on, by name. (Rock, Grass, etc - defined in the supplied FootProfile.)
    /// </summary>
    [HideInInspector] public SurfaceType CurrentFootMaterialName;
    //[HideInInspector] public TerrainCollider CurrentTerrain;
    //[HideInInspector] public float[,,] CurrentTerrainAlphas;

    float m_WalkThreshhold => Mathf.Lerp(m_CrouchSpeed, m_WalkSpeed, .5f);
    float m_RunThreshhold => Mathf.Lerp(m_WalkSpeed, m_RunSpeed, .5f);

    void Start() => m_AudioSource = GetComponent<AudioSource>();

    private Vector3[] m_PreviousPositions = new Vector3[6];

    private void InsertPosition(Vector3 newPosition)
    {
        for (int i = 5; i >= 1; i--)
            m_PreviousPositions[i] = m_PreviousPositions[i - 1];
        m_PreviousPositions[0] = newPosition;
    }

    private float m_PreStepTravelledDistance;

    private void Update()
    {
        // add the distance travelled
        float startY = m_PreviousPositions[0].y;
        m_PreviousPositions[0].y = m_PreviousPositions[1].y;
        m_PreStepTravelledDistance += Vector3.Distance(m_PreviousPositions[0], m_PreviousPositions[1]);
        m_PreviousPositions[0].y = startY;
        InsertPosition(transform.position);

        Vector3 _currentVelocity = (m_PreviousPositions[0] - m_PreviousPositions[1]) / Time.deltaTime;

        MaterialSpec curSpec = null;

        // Jumping
        if (_currentVelocity.y > 0.01f && CurrentFootMaterialName != SurfaceType.Air)
        {
            SurfaceType preName = CurrentFootMaterialName;
            refreshMaterial();
            curSpec ??= m_CurrentFootProfile.MaterialSpecifications.FirstOrDefault(spec => spec.MaterialName == preName);
            if (CurrentFootMaterialName == SurfaceType.Air)
                if (curSpec != null) playRandomOrdered(curSpec.Jumps);

        }
        // Landing
        if (_currentVelocity.y < -0.01f && CurrentFootMaterialName == SurfaceType.Air)
        {
            refreshMaterial();
            curSpec ??= m_CurrentFootProfile.MaterialSpecifications.FirstOrDefault(spec => spec.MaterialName == CurrentFootMaterialName);
            if (CurrentFootMaterialName != SurfaceType.Air)
                if (curSpec != null) playRandomOrdered(curSpec.Lands);
        }

        // Footsteps
        if (m_PreStepTravelledDistance > m_DistancePerStep)
        {
            m_PreStepTravelledDistance = 0;
            refreshMaterial();

            curSpec ??= m_CurrentFootProfile.MaterialSpecifications.FirstOrDefault(spec => spec.MaterialName == CurrentFootMaterialName);
            if (curSpec != null)
            {
                float randomize = Random.Range(RandomizedVolumeRange.x, RandomizedVolumeRange.y);
                m_AudioSource.volume = m_Volume * randomize * curSpec.VolumeMultiplier;
                if (_currentVelocity.magnitude < m_WalkThreshhold)
                {
                    m_AudioSource.volume *= .80f;
                    playRandomOrdered(curSpec.SoftSteps);
                }
                else if (_currentVelocity.magnitude < m_RunThreshhold)
                {
                    playRandomOrdered(curSpec.MediumSteps);
                }
                else
                {
                    playRandomOrdered(curSpec.HardSteps);
                }
            }
        }

        // Skid
        if (_currentVelocity.magnitude < m_WalkThreshhold && Vector3.Distance(m_PreviousPositions[4], m_PreviousPositions[5]) / Time.deltaTime > m_RunThreshhold)
        {
            curSpec ??= m_CurrentFootProfile.MaterialSpecifications.FirstOrDefault(spec => spec.MaterialName == CurrentFootMaterialName);
            if (curSpec != null)
                playRandomOrdered(curSpec.Scuffs);
        }
    }

    private uint m_StepCount = 0;
    private void playRandomOrdered(AudioClip[] clips, float scaleVol = 1)
    {
        if (clips == null || clips.Length == 0)
            return;
        m_AudioSource.pitch = Random.Range(RandomizedPitchRange.x, RandomizedPitchRange.y);
        m_AudioSource.PlayOneShot(clips[m_StepCount++ % clips.Length], scaleVol);
        if (m_StepCount % clips.Length == 0)
        {
            AudioClip[] shuffled = clips.Randomize().ToArray();
            if (clips[clips.Length - 1] == shuffled[0]) // Prevent sound playing twice edge case
            {
                shuffled[0] = shuffled[shuffled.Length - 1];
                shuffled[shuffled.Length - 1] = clips[clips.Length - 1];
            }
            for (int i = 0; i < clips.Length; i++)
                clips[i] = shuffled[i];
        }
    }

    private void refreshMaterial()
    {
        if (!Physics.Raycast(new Ray() { origin = transform.position, direction = Vector3.down }, out RaycastHit hitInfo) || hitInfo.distance > m_FloorDistance)
        {
            CurrentFootMaterialName = SurfaceType.Air;
            return;
        }
        else
        {
            MaterialTag tag = hitInfo.collider.GetComponent<MaterialTag>();
            if (tag != null)
                CurrentFootMaterialName = tag.MaterialType;

        }
    }
}

public static class FootStepSystemExtensions
{
    private static System.Random rnd = new System.Random();
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
        => source.OrderBy((item) => rnd.Next());
}
