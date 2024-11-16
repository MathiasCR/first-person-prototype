using UnityEngine;

public class MaterialTag : MonoBehaviour
{
    public SurfaceType MaterialType;
}

public enum SurfaceType
{
    Grass,
    Gravel,
    Metal,
    Rug,
    Sand,
    Snow,
    Stone,
    Water,
    Wood,
    Flesh,
    Air
}
