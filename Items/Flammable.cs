using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Flammable : MonoBehaviour, IInteractable
{
    [SerializeField] private bool m_StartActive;
    [SerializeField] private float m_InteractionTime;
    [SerializeField] private ParticleSystem m_FirePS;
    [SerializeField] private GameObject m_FireGameObject;

    public bool StartActive => m_StartActive;

    public bool IsActive => m_IsActive;

    public bool IsInteractable => m_IsInteractable;

    public bool IsInteracting => m_IsInteracting;

    public float InteractionTime => m_InteractionTime;

    private bool m_IsActive;
    private bool m_IsInteracting;
    private bool m_IsInteractable;
    private AudioSource m_FireAudioSource;

    private void Awake()
    {
        m_FireAudioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (m_StartActive)
        {
            Activate();
        }
    }

    public void Interact()
    {
        StartCoroutine(Interacting());
    }

    public IEnumerator Interacting()
    {
        m_IsInteracting = true;
        float duration = 0f;

        while (duration < InteractionTime)
        {
            duration += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }

        Activate();
    }

    public void Activate()
    {
        m_IsActive = !m_IsActive;
        m_FireGameObject.SetActive(m_IsActive);

        if (m_IsActive)
        {
            m_FireAudioSource.Play();
        }
        else
        {
            m_FireAudioSource.Stop();
        }
    }

    public void StopInteracting()
    {
        StopCoroutine(Interacting());
    }
}
