using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeSetter : MonoBehaviour
{
    [SerializeField] private bool IsMusic;
    private AudioSource m_Source;
    private float m_StartVolume;
    private void Start()
    {
        m_Source = GetComponent<AudioSource>();
        m_StartVolume = m_Source.volume;
    }
    // Start is called before the first frame update
    void Update()
    {
        m_Source.volume = m_StartVolume * (IsMusic ? Settings.Instance.MusicVolume : Settings.Instance.SoundEffectsVolume);
    }
}
