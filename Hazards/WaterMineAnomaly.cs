using UnityEngine;

public class WaterMineAnomaly : Anomaly
{
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_AnimDuration;

    private float m_Timer;

    protected override void Start()
    {
        base.Start();
        m_Timer = m_AnimDuration;
    }

    protected override void Update()
    {
        base.Update();

        if (m_VisualEffect == null) return;

        if (m_Timer > 0f)
        {
            float speed = m_Speed * (m_Timer / m_AnimDuration);
            m_VisualEffect.transform.position += Vector3.up * speed * Time.deltaTime;

            m_Timer -= Time.deltaTime;
        }
    }

    protected override void SpawnAnomaly()
    {
        base.SpawnAnomaly();
        m_Timer = m_AnimDuration;
    }
}
