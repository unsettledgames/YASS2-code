using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioClipPlayer : MonoBehaviour
{
    [SerializeField] private AnimationCurve RolloffCurve;
    [SerializeField] private AudioClip Clip;
    [SerializeField] private float Volume;
    [SerializeField] private float Spatiality;
    [SerializeField] private Vector2 PitchVariation;
    [SerializeField] private AudioMixerGroup MixerGroup;

    // Start is called before the first frame update
    void Start()
    {
        GameObject newObj = Instantiate(new GameObject(), transform.position, Quaternion.Euler(Vector3.zero));
        AudioSource source = newObj.AddComponent<AudioSource>();
        TimeDestroyer timeDestroyer = newObj.AddComponent<TimeDestroyer>();

        source.outputAudioMixerGroup = MixerGroup;
        source.clip = Clip;
        source.volume = Volume;
        source.pitch = 1.0f + Random.Range(PitchVariation.x, PitchVariation.y);
        source.spatialBlend = Spatiality;

        if (RolloffCurve != null && RolloffCurve.length > 0)
        {
            RolloffCurve.keys[0].value = Volume;
            source.rolloffMode = AudioRolloffMode.Custom;
            source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, RolloffCurve);
        }
        
        source.Play();

        timeDestroyer.SetLifeTime(Clip.length);
        StartCoroutine(timeDestroyer.StartDestroying());
    }
}