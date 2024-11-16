using System;
using UnityEngine;

public abstract class Activity : MonoBehaviour
{
    [SerializeField] protected float m_ActivityTime;
    [SerializeField] protected ActivityType m_ActivityType;

    public event Action OnActivityEnd;

    public ActivityType Type => m_ActivityType;
    public ActivitySpot Spot { get => m_ActivitySpot; set => m_ActivitySpot = value; }

    protected bool m_IsActing = false;
    protected ActivitySpot m_ActivitySpot;
    protected float m_ActivityDuration = 0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_IsActing)
        {
            m_ActivityDuration += Time.deltaTime;
            if (m_ActivityDuration >= m_ActivityTime)
            {
                ActivityEnded();
            }
        }
    }

    public virtual void StartActivity()
    {
        Debug.Log("base Start Activity");
        m_ActivitySpot.IsAvailable = false;
        m_IsActing = true;
        m_ActivityDuration = 0f;
    }

    protected virtual void ActivityEnded()
    {
        Debug.Log("base ActivityEnded");
        m_IsActing = false;
        OnActivityEnd?.Invoke();
    }
}
