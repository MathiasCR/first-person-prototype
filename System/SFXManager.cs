using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SFXManager : MonoBehaviour
{
    private AudioSource m_AudioSource;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public void PlayAudioOnce(AudioClip audioClip)
    {
        m_AudioSource.PlayOneShot(audioClip);
    }

    public void Stop()
    {
        m_AudioSource.Stop();
    }
}
