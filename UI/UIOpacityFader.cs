using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIOpacityFader : MonoBehaviour
{
    [SerializeField] private float TargetAlpha = 1.0f;
    [SerializeField] private float FadeDuration;
    [SerializeField] private float FadeDelay;
    [SerializeField] private bool FadeOnStart;

    private TextMeshProUGUI[] m_Texts;
    private Image[] m_Images;
    // Start is called before the first frame update
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
            button.interactable = false;

        m_Texts = GetComponentsInChildren<TextMeshProUGUI>();
        m_Images = GetComponentsInChildren<Image>();

        Reset();

        if (FadeOnStart)
            Fade();
    }

    public void Fade()
    {
        StartCoroutine(FadeRoutine());
    }

    public void Reset()
    {
        for (int i = 0; i < m_Texts.Length; i++)
        {
            m_Texts[i].color = new Color(255, 255, 255, 1);
            m_Texts[i].canvasRenderer.SetAlpha(0);
        }
        for (int i = 0; i < m_Images.Length; i++)
        {
            m_Images[i].color = new Color(255, 255, 255, 1);
            m_Images[i].canvasRenderer.SetAlpha(0);
        }
    }

    private IEnumerator FadeRoutine()
    {
        yield return new WaitForSecondsRealtime(FadeDelay);

        for (int i = 0; i < m_Texts.Length; i++)
            m_Texts[i].CrossFadeAlpha(1, FadeDuration, true);
        for (int i = 0; i < m_Images.Length; i++)
            m_Images[i].CrossFadeAlpha(1, FadeDuration, true);

        yield return new WaitForSecondsRealtime(FadeDuration);

        Button button = GetComponent<Button>();
        if (button != null)
            button.interactable = true;

        yield return null;
    }

    public void SetTargetAlpha(float val)
    {
        TargetAlpha = val;
    }
}
