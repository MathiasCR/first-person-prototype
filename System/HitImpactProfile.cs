using UnityEngine;

[CreateAssetMenu(fileName = "New HitImpact Profile", menuName = "HitImpact/HitImpact Profile", order = 1)]
public class HitImpactProfile : ScriptableObject
{
    public string ProfileName;
    public ImpactSpec[] ImpactSpecifications;
}

[System.Serializable]
public class ImpactSpec
{
    public SurfaceType MaterialName;
    [Range(0, 1)]
    public float VolumeMultiplier = 1;
    public AudioClip ImpactClip;
}
