using UnityEngine;

public class ActivitySpot : MonoBehaviour
{
    [SerializeField] private ActivityType m_SpotType;

    public ActivityType SpotType => m_SpotType;
    public bool IsAvailable = true;
}

public enum ActivityType
{
    Weaving,
    None
}
