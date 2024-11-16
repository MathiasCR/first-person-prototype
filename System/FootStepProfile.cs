using UnityEngine;

[CreateAssetMenu(fileName = "New Foot Step Profile", menuName = "Foot Step/Foot Step Profile", order = 1)]
public class FootStepProfile : ScriptableObject
{
    public string ProfileName;
    public MaterialSpec[] MaterialSpecifications;
}

[System.Serializable]
public class MaterialSpec
{
    public SurfaceType MaterialName;
    [Range(0, 1)]
    public float VolumeMultiplier = 1;
    public AudioClip[] SoftSteps;
    public AudioClip[] MediumSteps;
    public AudioClip[] HardSteps;
    public AudioClip[] Scuffs;
    public AudioClip[] Jumps;
    public AudioClip[] Lands;
}