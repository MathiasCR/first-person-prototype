using UnityEngine;
using UnityEngine.VFX;

public abstract class Anomaly : MonoBehaviour
{
    [SerializeField] protected float m_SpawnRate;
    [SerializeField] protected VisualEffect m_AnomalyEffect;

    protected VisualEffect m_VisualEffect;
    protected float m_Duration = 0.0f;
    protected float m_NewSpawnRate = 0.0f;
    protected float m_LastSpawnTime = 0.0f;
    protected Renderer m_ParentRenderer;

    protected virtual void Start()
    {
        m_NewSpawnRate = m_SpawnRate;
        m_ParentRenderer = GetComponentInParent<Renderer>();
    }

    // Start is called before the first frame update
    protected virtual void Update()
    {
        if (m_ParentRenderer == null || !m_ParentRenderer.isVisible) return;

        if (m_LastSpawnTime > m_NewSpawnRate && m_LastSpawnTime > m_Duration)
        {
            SpawnAnomaly();
        }
        else
        {
            m_LastSpawnTime += Time.deltaTime;
        }

        if (m_VisualEffect == null) return;

        if (m_Duration > 0.0f)
        {
            m_Duration -= Time.deltaTime;
        }
        else
        {
            Destroy(m_VisualEffect.gameObject);
        }
    }

    protected virtual void SpawnAnomaly()
    {
        m_LastSpawnTime = 0.0f;
        m_VisualEffect = Instantiate(m_AnomalyEffect, transform.position, Quaternion.identity);
        m_Duration = m_VisualEffect.GetFloat("AnomalyDuration");
        m_NewSpawnRate = Random.Range(m_SpawnRate / 2, m_SpawnRate);
    }
}
