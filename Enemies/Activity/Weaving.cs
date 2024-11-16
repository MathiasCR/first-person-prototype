using UnityEngine;

public class Weaving : Activity
{
    private Web m_Web;

    // Start is called before the first frame update
    void Start()
    {
    }

    public override void StartActivity()
    {
        Debug.Log("Weaving Start Activity");

        base.StartActivity();
        m_Web = m_ActivitySpot.GetComponentInChildren<Web>();
    }

    protected override void ActivityEnded()
    {
        Debug.Log("Weaving ActivityEnded");

        base.ActivityEnded();
        ChangeWebState(true);
        m_Web.OnTrigger += OnWebTrigger;
    }

    private void OnWebTrigger()
    {
        Debug.Log("OnWebTrigger");
        m_Web.OnTrigger -= OnWebTrigger;
        ChangeWebState(false);
        m_ActivitySpot.IsAvailable = true;
        m_ActivitySpot = null;
    }

    private void ChangeWebState(bool active)
    {
        if (m_Web != null)
        {
            m_Web.GetComponent<Collider>().enabled = active;
            m_Web.GetComponent<Renderer>().enabled = active;
        }
    }
}
