using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

[RequireComponent(typeof(VisualEffect)), RequireComponent(typeof(AudioSource))]
public class AnomalySoundEffect : VFXOutputEventAbstractHandler
{
    [SerializeField] private bool m_Override;
    [SerializeField] private AudioClip m_EffectAudio;

    public override bool canExecuteInEditor => true;

    private AudioSource m_AudioSource;

    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }

    public override void OnVFXOutputEvent(VFXEventAttribute eventAttribute)
    {
        if (m_Override) m_AudioSource.Stop();
        m_AudioSource.PlayOneShot(m_EffectAudio);
    }
}
