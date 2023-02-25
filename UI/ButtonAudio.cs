using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAudio : MonoBehaviour
{
    [SerializeField] private AudioClip HoveredClip;
    [SerializeField] private AudioClip ClickedClip;

    private AudioSource m_AudioSource;
    private Selectable m_Button;
    private void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_Button = GetComponent<Selectable>();
    }
    public void OnHover()
    {
        if (!m_Button.interactable)
            return;
        m_AudioSource.clip = HoveredClip;
        m_AudioSource.Play();
    }

    public void OnClick()
    {
        m_AudioSource.clip = ClickedClip;
        m_AudioSource.Play();
    }
}
