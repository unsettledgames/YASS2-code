using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleMenuBackground : MonoBehaviour
{
    [SerializeField] private Sprite[] Backgrounds;
    [SerializeField] private Image Black;
    [SerializeField] private float FadeDuration;
    [SerializeField] private float ZoomSpeed;
    [SerializeField] private float TimePerBackground;

    private Image m_Image;
    private RectTransform m_ImageTransform;
    private Vector3 m_StartImageScale;
    private int m_CurrIndex = -1;

    // Start is called before the first frame update
    void Start()
    {
        m_Image = GetComponent<Image>();
        m_ImageTransform = m_Image.GetComponent<RectTransform>();
        m_StartImageScale = m_ImageTransform.localScale;

        StartCoroutine(BackgroundRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator BackgroundRoutine()
    {
        while (true)
        {
            // Reset background
            m_ImageTransform.localScale = m_StartImageScale;
            m_CurrIndex = (m_CurrIndex + 1) % Backgrounds.Length;

            // Fade black to transparent
            m_Image.sprite = Backgrounds[m_CurrIndex];
            Black.CrossFadeAlpha(0.0f, FadeDuration, true);
            yield return new WaitForSeconds(FadeDuration);

            // Zoom background
            float endTime = Time.time + TimePerBackground;
            while (endTime >= Time.time)
            {
                float scale = Time.deltaTime * ZoomSpeed;
                m_ImageTransform.localScale += new Vector3(scale, scale, scale);
                yield return new WaitForEndOfFrame();
            }

            // Fade to black
            Black.CrossFadeAlpha(1.0f, FadeDuration, true);
            yield return new WaitForSeconds(FadeDuration);
        }
    }
}
