using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Image Black;
    [SerializeField] private Button[] Buttons;
    [SerializeField] private AudioSource Music;
    [SerializeField] private float MusicFadeSpeed;
    [SerializeField] private GameObject CreditsWindow;
    [SerializeField] private GameObject SettingsWindow;

    private string m_CurrScene = "Title";

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        CreditsWindow.SetActive(false);
        SettingsWindow.SetActive(false);
        Black.CrossFadeAlpha(0, 0, true);
    }

    public void LoadCredits()
    {
        StartCoroutine(LoadScene("Credits"));
    }

    public void LoadMain()
    {
        StartCoroutine(LoadScene("Main"));
    }

    public void LoadSettings()
    {
        StartCoroutine(LoadScene("Settings"));
    }

    public void ShowTitle()
    {
        StartCoroutine(ShowTitleRoutine());
    }

    public void Quit()
    {
        Application.Quit();
    }

    private IEnumerator ShowTitleRoutine()
    {
        Black.CrossFadeAlpha(1, 0.75f, true);
        yield return new WaitForSeconds(0.75f);

        for (int i = 0; i < Buttons.Length; i++)
            Buttons[i].interactable = true;
        CreditsWindow.SetActive(false);
        SettingsWindow.SetActive(false);

        Black.CrossFadeAlpha(0, 0.75f, true);
        yield return new WaitForSeconds(0.75f);
    }

    private IEnumerator LoadScene(string name)
    {
        for (int i = 0; i < Buttons.Length; i++)
            Buttons[i].interactable = false;
        
        if (name.Equals("Main"))
        {
            Black.CrossFadeAlpha(1, 3, true);
            StartCoroutine(FadeMusic(0));
            
            yield return new WaitForSeconds(3);

            m_CurrScene = name;
            SceneManager.LoadScene(1);
        }
        else
        {
            Black.CrossFadeAlpha(1, 0.75f, true);
            yield return new WaitForSeconds(0.75f);

            if (name.Equals("Credits"))
                CreditsWindow.SetActive(true);
            else if (name.Equals("Settings"))
                SettingsWindow.SetActive(true);

            Black.CrossFadeAlpha(0, 0.75f, true);
            yield return new WaitForSeconds(0.75f);
        }
    }

    private IEnumerator FadeMusic(float targetVolume)
    {
        float volume = Music.volume;

        while (Mathf.Abs(volume - targetVolume) > 0.01f)
        {
            volume -= Time.deltaTime * MusicFadeSpeed;
            Music.volume = volume;
            yield return new WaitForEndOfFrame();
        }
    }
}