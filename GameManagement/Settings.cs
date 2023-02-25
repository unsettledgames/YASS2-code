using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// TODO: Set UI
public class Settings : MonoBehaviour
{
    [Header("UI elements")]
    [SerializeField] private Toggle ToneMapping;
    [SerializeField] private Toggle InvertHorizontal;
    [SerializeField] private Toggle InvertVertical;

    [SerializeField] private Slider Bloom;
    [SerializeField] private Slider RedR;
    [SerializeField] private Slider RedG;
    [SerializeField] private Slider RedB;
    [SerializeField] private Slider GreenR;
    [SerializeField] private Slider GreenG;
    [SerializeField] private Slider GreenB;
    [SerializeField] private Slider BlueR;
    [SerializeField] private Slider BlueG;
    [SerializeField] private Slider BlueB;
    [SerializeField] private Slider Music;
    [SerializeField] private Slider Effects;

    [SerializeField] private Toggle EasyButton;
    [SerializeField] private Toggle MediumButton;
    [SerializeField] private Toggle DifficultButton;

    [SerializeField] private Image RadarTopImg;
    [SerializeField] private Image RadarMidImg;
    [SerializeField] private Image RadarBottomImg;

    [SerializeField] private Image RadarPreviewTop;
    [SerializeField] private Image RadarPreviewMid;
    [SerializeField] private Image RadarPreviewBottom;

    [SerializeField] private AudioMixer Mixer;

    [Header("SETTINGS")]
    public float Difficulty = 1.0f;
    public Color RadarBottomColor;
    public Color RadarMidColor;
    public Color RadarTopColor;

    public int EnableToneMapping = 1;
    public float BloomIntensity = 1.0f;
    // Color correction

    public float MusicVolume = 1.0f;
    public float SoundEffectsVolume = 1.0f;

    public int InvertHorizontalAxis = 1;
    public int InvertVerticalAxis = 1;

    public Vector3 RedMix = new Vector3(1, 0, 0);
    public Vector3 GreenMix = new Vector3(0, 1, 0);
    public Vector3 BlueMix = new Vector3(0, 0, 1);

    public static Settings Instance;

    private bool m_Inited = false;
    private Volume m_Volume;
    private Bloom m_Bloom;
    private Tonemapping m_Tonemapping;
    private ColorAdjustments m_ColorAdjustments;
    private ChannelMixer m_ChannelMixer;

    public void Awake()
    {
        Instance = this;
        m_Volume = GameObject.FindGameObjectWithTag("PostProcessing").GetComponent<Volume>();
        m_Volume.profile.TryGet<Bloom>(out m_Bloom);
        m_Volume.profile.TryGet<Tonemapping>(out m_Tonemapping);
        m_Volume.profile.TryGet<ColorAdjustments>(out m_ColorAdjustments);
        m_Volume.profile.TryGet<ChannelMixer>(out m_ChannelMixer);
    }

    public void Start()
    {
        if (!PlayerPrefs.HasKey("First"))
            SavePrefs();
        PlayerPrefs.SetInt("First", 1);
        LoadPrefs();

        m_Inited = true;
    }

    private void SetVector3(string key, Vector3 vec)
    {
        PlayerPrefs.SetFloat(key + ".x", vec.x);
        PlayerPrefs.SetFloat(key + ".y", vec.y);
        PlayerPrefs.SetFloat(key + ".z", vec.z);
    }

    private void SetColor(string key, Color vec)
    {
        PlayerPrefs.SetFloat(key + ".r", vec.r);
        PlayerPrefs.SetFloat(key + ".g", vec.g);
        PlayerPrefs.SetFloat(key + ".b", vec.b);
        PlayerPrefs.SetFloat(key + ".a", vec.a);
    }

    private Vector3 GetVector3(string key)
    {
        return new Vector3(PlayerPrefs.GetFloat(key + ".x"), PlayerPrefs.GetFloat(key + ".y"), PlayerPrefs.GetFloat(key + ".z"));
    }

    private Color GetColor(string key)
    {
        return new Color(PlayerPrefs.GetFloat(key + ".r"), PlayerPrefs.GetFloat(key + ".g"),
            PlayerPrefs.GetFloat(key + ".b"), PlayerPrefs.GetFloat(key + ".a"));
    }

    public void SetDifficulty(float val) 
    { 
        Difficulty = val;
        if (val == 1.0f)
            MediumButton.isOn = true;
        else if (val > 1.0f)
            DifficultButton.isOn = true;
        else if (val < 1.0f)
            EasyButton.isOn = true;
        SavePrefs();
    }
    public void SetRadarBottom(Color val) 
    { 
        RadarBottomColor = val;
        RadarBottomImg.color = val;
        RadarPreviewBottom.color = val;
        SavePrefs();
    }
    public void SetRadarMid(Color val) 
    { 
        RadarMidColor = val;
        RadarMidImg.color = val;
        RadarPreviewMid.color = val;
        SavePrefs();
    }
    public void SetRadarTop(Color val) 
    { 
        RadarTopColor = val;
        RadarTopImg.color = val;
        RadarPreviewTop.color = val;
        SavePrefs();
    }
    public void SetTonemapping(bool val) 
    { 
        EnableToneMapping = (val ? 1 : 0);
        m_Volume.profile.TryGet<Tonemapping>(out Tonemapping tm);
        tm.active = val;
        m_Volume.profile.TryGet<ColorAdjustments>(out ColorAdjustments ca);
        ca.active = val;

        SavePrefs();
    }
    public void SetBloom(float val) 
    { 
        BloomIntensity = val;
        m_Volume.profile.TryGet<Bloom>(out Bloom b);
        var param = new VolumeParameter<float>();
        param.value = val;
        m_Bloom.intensity.SetValue(param);
        SavePrefs();
    }
    public void SetMusicVolume(float val) 
    {
        MusicVolume = val;
        Mixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, val));
        SavePrefs();
    }
    public void SetEffectsVolume(float val) 
    { 
        SoundEffectsVolume = val;
        Mixer.SetFloat("EffectsVolume", Mathf.Lerp(-80, 0, val));
        SavePrefs();
    }
    public void SetInvertHorizontal(bool val)
    { 
        InvertHorizontalAxis = (val ? -1 : 1);
        SavePrefs();
    }
    public void SetInvertVertical(bool val)
    { 
        InvertVerticalAxis = (val ? -1 : 1);
        SavePrefs();
    }

    public void SetRedR(float val) 
    {
        RedMix.x = val;
        m_ChannelMixer.redOutRedIn.value = val;
        SavePrefs();
    }

    public void SetRedG(float val) 
    {
        RedMix.y = val;
        m_ChannelMixer.redOutGreenIn.value = val;
        SavePrefs();
    }

    public void SetRedB(float val) 
    {
        RedMix.z = val;
        m_ChannelMixer.redOutBlueIn.value = val;
        SavePrefs();
    }


    public void SetGreenR(float val) 
    { 
        GreenMix.x = val;
        m_ChannelMixer.greenOutRedIn.value = val;
        SavePrefs();
    }
    public void SetGreenG(float val) 
    { 
        GreenMix.y = val;
        m_ChannelMixer.greenOutGreenIn.value = val;
        SavePrefs();
    }
    public void SetGreenB(float val) 
    { 
        GreenMix.z = val;
        m_ChannelMixer.greenOutBlueIn.value = val;
        SavePrefs();
    }

    public void SetBlueR(float val) 
    {
        BlueMix.x = val;
        m_ChannelMixer.blueOutRedIn.value = val;
        SavePrefs();
    }
    public void SetBlueG(float val) 
    {
        BlueMix.y = val;
        m_ChannelMixer.blueOutGreenIn.value = val;
        SavePrefs();
    }
    public void SetBlueB(float val) 
    {
        BlueMix.z = val;
        m_ChannelMixer.blueOutBlueIn.value = val;
        SavePrefs();
    }

    private void SavePrefs()
    {
        if (!m_Inited)
            return;

        // Store default player prefs
        PlayerPrefs.SetFloat("Difficulty", Difficulty);
        PlayerPrefs.SetFloat("MusicVolume", MusicVolume);
        PlayerPrefs.SetFloat("SoundEffectsVolume", SoundEffectsVolume);
        PlayerPrefs.SetInt("InvertHorizontal", InvertHorizontalAxis);
        PlayerPrefs.SetInt("InvertVertical", InvertVerticalAxis);
        PlayerPrefs.SetFloat("BloomIntensity", BloomIntensity);
        PlayerPrefs.SetInt("ToneMapping", EnableToneMapping);

        SetColor("RadarBottom", RadarBottomColor);
        SetColor("RadarMid", RadarMidColor);
        SetColor("RadarTop", RadarTopColor);

        SetVector3("RedMix", RedMix);
        SetVector3("GreenMix", GreenMix);
        SetVector3("BlueMix", BlueMix);

        PlayerPrefs.Save();
    }

    private void LoadPrefs()
    {
        // Load player prefs
        Difficulty = PlayerPrefs.GetFloat("Difficulty");

        // Store default player prefs
        MusicVolume = PlayerPrefs.GetFloat("MusicVolume");
        SoundEffectsVolume = PlayerPrefs.GetFloat("SoundEffectsVolume");

        InvertHorizontalAxis = PlayerPrefs.GetInt("InvertHorizontal");
        InvertVerticalAxis = PlayerPrefs.GetInt("InvertVertical");

        BloomIntensity = PlayerPrefs.GetFloat("BloomIntensity");
        m_Bloom.intensity.value = BloomIntensity;

        EnableToneMapping = PlayerPrefs.GetInt("ToneMapping");
        m_Tonemapping.active = EnableToneMapping == 1 ? true : false;
        m_ColorAdjustments.active = EnableToneMapping == 1 ? true : false;

        RadarBottomColor = GetColor("RadarBottom");
        RadarMidColor = GetColor("RadarMid");
        RadarTopColor = GetColor("RadarTop");

        RedMix = GetVector3("RedMix");
        m_ChannelMixer.redOutRedIn.value = RedMix.x;
        m_ChannelMixer.redOutGreenIn.value = RedMix.y;
        m_ChannelMixer.redOutBlueIn.value = RedMix.z;

        GreenMix = GetVector3("GreenMix");
        m_ChannelMixer.greenOutRedIn.value = GreenMix.x;
        m_ChannelMixer.greenOutGreenIn.value = GreenMix.y;
        m_ChannelMixer.greenOutBlueIn.value = GreenMix.z;

        BlueMix = GetVector3("BlueMix");
        m_ChannelMixer.blueOutRedIn.value = BlueMix.x;
        m_ChannelMixer.blueOutGreenIn.value = BlueMix.y;
        m_ChannelMixer.blueOutBlueIn.value = BlueMix.z;

        if (ToneMapping != null)
            UpdateUI();
    }

    private void UpdateUI()
    {
        if (Difficulty > 1.0f)
            DifficultButton.isOn = true;
        else if (Difficulty < 1.0f)
            EasyButton.isOn = true;
        else
            MediumButton.isOn = true;

        Music.value = MusicVolume;
        Mixer.SetFloat("MusicVolume", Mathf.Lerp(-80, 0, MusicVolume));
        Effects.value = SoundEffectsVolume;
        Mixer.SetFloat("EffectsVolume", Mathf.Lerp(-80, 0, SoundEffectsVolume));
        
        InvertHorizontal.isOn = InvertHorizontalAxis == -1 ? true : false;
        InvertVertical.isOn = InvertVerticalAxis == -1 ? true : false;

        Bloom.value = BloomIntensity;
        ToneMapping.isOn = EnableToneMapping == 1 ? true : false;

        RadarBottomImg.color = RadarBottomColor;
        RadarTopImg.color = RadarTopColor;
        RadarMidImg.color = RadarMidColor;

        RadarPreviewBottom.color = RadarBottomColor;
        RadarPreviewTop.color = RadarTopColor;
        RadarPreviewMid.color = RadarMidColor;

        RedR.value = RedMix.x;
        RedG.value = RedMix.y;
        RedB.value = RedMix.z;

        GreenR.value = GreenMix.x;
        GreenG.value = GreenMix.y;
        GreenB.value = GreenMix.z;

        BlueR.value = BlueMix.x;
        BlueG.value = BlueMix.y;
        BlueB.value = BlueMix.z;

    }

}
