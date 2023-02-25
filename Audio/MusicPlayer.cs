using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip MusicStart;
    [SerializeField] private AudioClip MusicLoop;
    [SerializeField] private AudioMixerGroup MixerGroup;
    [SerializeField] private float Volume;

    private AudioSource m_StartComponent;
    private AudioSource m_LoopComponent;

    // Start is called before the first frame update
    void Start()
    {
        m_StartComponent = gameObject.AddComponent<AudioSource>();
        m_LoopComponent = gameObject.AddComponent<AudioSource>();

        m_StartComponent.spatialBlend = 0.0f;
        m_LoopComponent.spatialBlend = 0.0f;

        m_StartComponent.volume = Volume * Settings.Instance.MusicVolume;
        m_LoopComponent.volume = Volume * Settings.Instance.MusicVolume;

        m_StartComponent.clip = MusicStart;
        m_LoopComponent.clip = MusicLoop;
        m_LoopComponent.loop = true;

        m_StartComponent.outputAudioMixerGroup = MixerGroup;
        m_LoopComponent.outputAudioMixerGroup = MixerGroup;

        m_StartComponent.Play();
        StartCoroutine(PlayLoop());
    }

    private IEnumerator PlayLoop()
    {
        yield return new WaitForSeconds(MusicStart.length - Time.deltaTime);
        m_LoopComponent.Play();
    }
}
