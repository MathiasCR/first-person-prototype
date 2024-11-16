using UnityEngine;

public class HeadBobSystem : MonoBehaviour
{
    [SerializeField] private float m_BobbingAmount;
    [SerializeField] private float m_BobbingFrequency;
    [SerializeField] private float m_BobbingSmooth;

    public float CrounchedFrequency = 0.5f;
    public float CrounchedRunFrequency = 0.7f;
    public float WalkFrequency = 1f;
    public float RunFrequency = 1.5f;

    private Vector3 m_StartPosition;

    private void Start()
    {
        m_StartPosition = transform.localPosition;
    }

    private void Update()
    {
        StopHeadBob();
    }

    public Vector3 StartHeadBob(float frequencyMultiplier)
    {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * m_BobbingFrequency * frequencyMultiplier) * m_BobbingAmount * 1.4f, m_BobbingSmooth * Time.deltaTime);
        pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * m_BobbingFrequency / 2f * frequencyMultiplier) * m_BobbingAmount * 1.6f, m_BobbingSmooth * Time.deltaTime);
        transform.localPosition += pos;

        return pos;
    }

    private void StopHeadBob()
    {
        if (transform.localPosition == m_StartPosition) return;
        transform.localPosition = Vector3.Lerp(transform.localPosition, m_StartPosition, 5f * Time.deltaTime);
    }
}
