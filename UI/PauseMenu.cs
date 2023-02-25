using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer Mixer;
    [SerializeField] private UIOpacityFader[] Faders;
    [SerializeField] private Image PauseBlack;
    [SerializeField] private Image MainBlack;
    [SerializeField] private GameObject HUD;
    [SerializeField] private GameObject SettingsWindow;

    private Camera m_UICamera;
    private Camera m_MainCamera;
    private GameObject m_Elements;

    private bool m_IsTransitioning = false;

    public static PauseMenu Instance;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        Time.timeScale = 1.0f;
        Mixer.SetFloat("MasterVolume", 0.0f);
        m_MainCamera = FrequentlyAccessed.Instance.Camera;
        MainBlack.canvasRenderer.SetAlpha(1);

        m_UICamera = transform.GetChild(0).gameObject.GetComponent<Camera>();
        m_Elements = transform.GetChild(1).gameObject;
        m_Elements.SetActive(false);

        StartCoroutine(FadeBlack());
    }

    private IEnumerator FadeBlack()
    {
        yield return new WaitForSeconds(1.0f);
        MainBlack.CrossFadeAlpha(0, 1.5f, true);
    }

    private void Start()
    {
        SettingsWindow.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!m_Elements.activeSelf && !m_IsTransitioning)
                StartCoroutine(ShowRoutine());
            else if (!m_IsTransitioning)
                StartCoroutine(HideRoutine());
        }
    }

    private IEnumerator ShowRoutine()
    {
        m_IsTransitioning = true;
        Time.timeScale = 0.0f;

        // Show black
        MainBlack.gameObject.SetActive(true);
        MainBlack.canvasRenderer.SetAlpha(0);
        MainBlack.CrossFadeAlpha(1, 0.5f, true);
        yield return new WaitForSecondsRealtime(0.5f);

        // Disable main camera, enable UI camera
        m_MainCamera.enabled = false;
        HUD.SetActive(false);
        m_Elements.SetActive(true);
        m_UICamera.enabled = true;

        // Disable black
        MainBlack.gameObject.SetActive(false);
        PauseBlack.CrossFadeAlpha(0, 0.5f, true);
        yield return new WaitForSecondsRealtime(0.5f);

        // Show UI
        Fade();
        
        m_IsTransitioning = false;
        yield return null;
    }

    private IEnumerator HideRoutine()
    {
        m_IsTransitioning = true;

        // Show black
        PauseBlack.gameObject.SetActive(true);
        PauseBlack.CrossFadeAlpha(1, 0.5f, true);
        MainBlack.canvasRenderer.SetAlpha(1.0f);
        yield return new WaitForSecondsRealtime(0.5f);

        // Disable main camera, enable UI camera
        m_MainCamera.enabled = true;
        m_Elements.SetActive(false);
        HUD.SetActive(true);
        m_UICamera.enabled = false;

        // Disable black
        MainBlack.gameObject.SetActive(true);
        MainBlack.CrossFadeAlpha(0, 0.5f, true);
        yield return new WaitForSecondsRealtime(0.5f);

        // Show UI
        ResetFaders();

        MainBlack.gameObject.SetActive(false);
        m_IsTransitioning = false;
        Time.timeScale = 1.0f;
        yield return null;
    }

    private void Fade()
    {
        for (int i = 0; i < Faders.Length; i++)
            Faders[i].Fade();
    }
    
    private void ResetFaders()
    {
        for (int i = 0; i < Faders.Length; i++)
            Faders[i].Reset();
    }
    public void Quit()
    {
        Application.Quit();
    }

    public void ShowSettings() { StartCoroutine(SettingsRoutine()); }
    private IEnumerator SettingsRoutine()
    {
        // Fade black
        PauseBlack.gameObject.SetActive(true);
        PauseBlack.canvasRenderer.SetAlpha(0);
        PauseBlack.CrossFadeAlpha(1.0f, 0.5f, true);

        yield return new WaitForSecondsRealtime(0.5f);

        // Show settings, move them if necessary
        SettingsWindow.SetActive(true);
        PauseBlack.canvasRenderer.SetAlpha(1);
        PauseBlack.CrossFadeAlpha(0.0f, 0.5f, true);

        yield return new WaitForSecondsRealtime(0.5f);
        PauseBlack.gameObject.SetActive(false);
    }

    public void SettingsBack() { StartCoroutine(SettingsBackRoutine()); }
    private IEnumerator SettingsBackRoutine()
    {
        // Fade black
        PauseBlack.gameObject.SetActive(true);
        PauseBlack.canvasRenderer.SetAlpha(0);
        PauseBlack.CrossFadeAlpha(1.0f, 0.5f, true);

        yield return new WaitForSecondsRealtime(0.5f);

        // Show settings, move them if necessary
        SettingsWindow.SetActive(false);
        PauseBlack.canvasRenderer.SetAlpha(1);
        PauseBlack.CrossFadeAlpha(0.0f, 0.5f, true);

        yield return new WaitForSecondsRealtime(0.5f);
        PauseBlack.gameObject.SetActive(false);
    }

    public void Restart() { StartCoroutine(RestartRoutine()); }
    private IEnumerator RestartRoutine()
    {
        Mixer.GetFloat("MasterVolume", out float volume);
        // Fade black
        PauseBlack.gameObject.SetActive(true);
        PauseBlack.canvasRenderer.SetAlpha(0);
        PauseBlack.CrossFadeAlpha(1.0f, 2.0f, true);

        while (volume > -80.0f)
        {
            volume -= Time.unscaledDeltaTime * 15.0f;
            Mixer.SetFloat("MasterVolume", volume);
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(1);
    }

    public void Unpause()
    {
        StartCoroutine(HideRoutine());
    }

    public void RestartAfterDeath()
    {
        StartCoroutine(RestartAfterDeathRoutine());
    }
    private IEnumerator RestartAfterDeathRoutine()
    {
        yield return new WaitForSeconds(2.0f);

        MainBlack.gameObject.SetActive(true);
        MainBlack.canvasRenderer.SetAlpha(0);
        MainBlack.CrossFadeAlpha(1, 2.0f, false);

        float volume = 0.0f;
        while (volume > -60.0f)
        {
            volume -= Time.unscaledDeltaTime * 7.0f;
            Mixer.SetFloat("MasterVolume", volume);
            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(1);
        yield return null;
    }
}
