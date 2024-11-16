using UnityEngine;

[CreateAssetMenu(fileName = "New Particle Profile", menuName = "Particle/Particle Profile", order = 1)]
public class ParticleProfile : ScriptableObject
{
    public string ProfileName;
    public ParticleSpec[] ParticleSpecifications;
}

[System.Serializable]
public class ParticleSpec
{
    public SurfaceType MaterialName;
    public ParticleSystem[] ParticleSystems;
}
