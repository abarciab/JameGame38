using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public enum SoundType { sfx, music, ambient}

[CreateAssetMenu(fileName = "New Sound", menuName = "Sound")]
public class Sound : ScriptableObject
{
    [System.Serializable]
    public class Clip {
        [HideInInspector] public string name;
        public AudioClip clip;
        public bool looping;
        public bool CustomPitchAndVolume;
        [ConditionalField(nameof(CustomPitchAndVolume)), Range(0, 1), DefaultValue(1.0f)]
        public float volume = 1;
        [ConditionalField(nameof(CustomPitchAndVolume)), Range(0, 2), DefaultValue(1.0f)]
        public float pitch = 1;

        public Clip()
        {
            volume = 1;
            pitch = 1;
        }

        public Clip(Clip toCopy, float volume = -1, float pitch = -1)
        {
            clip = toCopy.clip;
            this.volume = volume == -1 ? toCopy.volume : volume;
            this.pitch = pitch == -1 ? toCopy.pitch : pitch;
            looping = toCopy.looping;
        }
    }

    [SerializeField] List<Clip> clips = new List<Clip>();
    public SoundType type;
    [SerializeField] bool SynchronizePitchAndVolume = true;
    [ConditionalField(nameof(SynchronizePitchAndVolume)), Range(0, 2), DefaultValue(1.0f)]
    [SerializeField] float pitch = 1;
    [ConditionalField(nameof(SynchronizePitchAndVolume)), Range(0, 2), DefaultValue(1.0f)]
    [SerializeField] float volume = 1;
    float actualVolume;
    public bool unpauseable;

    [SerializeField] bool randomizePitch;
    [SerializeField, ConditionalField(nameof(randomizePitch)), Range(0, 1), Tooltip("randomizes by this much above and below selected pitch")] float pitchRandomizeAmount;

    [HideInInspector] public AudioSource audioSource;
    [HideInInspector]public bool instantialized;
    Vector3 sourcePos;
    bool setPos;

    public float percentVolume { get { return audioSource ? audioSource.volume / actualVolume : 0; } }

    private void OnValidate()
    {
        foreach (var clip in clips) if (clip.clip) clip.name = clip.clip.name;
    }

    public void UpdateLocalPosition(Vector3 pos)
    {
        sourcePos = pos;
        setPos = true;
    }

    public void PercentVolume(float percent, float smoothness = 1)
    {
        if (!audioSource) return;
        //Debug.Log("precent volume: " + vol);
        audioSource.volume = Mathf.Lerp(audioSource.volume, actualVolume * percent, smoothness);
    }

    public void Stop()
    {
        if (!instantialized || !audioSource) return;

        audioSource.Stop();
    }

    void Awake()
    {
        if (Application.isPlaying) instantialized = true;
    }

    public void Delete()
    {
        Destroy(audioSource);
    }

    public void PlaySilent(Transform caller = null, bool restart = true)
    {
        if (!instantialized) {
            Debug.LogError("PlaySilent() was called on an uninstatizlized Sound");
            return;
        }

        if (!audioSource) FirstTimePlay(caller, restart);
        Play(true, true);
    }

    public void SetUp(Transform caller = null, bool restart = true)
    {
        if (!instantialized) {
            Debug.LogError("SetUp() was called on an uninstatizlized Sound");
            return;
        }
        if (clips.Count == 0) return;

        if (audioSource == null) FirstTimePlay(caller, restart);
    }

    public void PlayLine(Transform speaker, int index)
    {
        if (!instantialized) {
            Debug.LogError("PlaySilent() was called on an uninstatizlized Sound");
            return;
        }

        if (!audioSource) SetUp(speaker);

        audioSource.Stop();
        Play(true, index:index);
    }

    public float GetClipLength()
    {
        if (clips.Count == 0 || clips[0].clip == null) return 0;
        return clips[0].clip.length;
    }

    public void Play(Transform caller = null, bool restart = true)
    {
        if (!instantialized) {
            Debug.LogError("Play() was called on an uninstatizlized Sound");
            return;
        }
        if (clips.Count == 0) return;
       
        if (audioSource == null) FirstTimePlay(caller, restart);
        else Play(restart);
       
    }
    void Play(bool restart, bool silent = false, int index = 0)
    {
        var clip = GetClip();
        if (audioSource.isPlaying && !restart) return;

        if (setPos) audioSource.transform.localPosition = sourcePos;
        setPos = false;

        float _pitch = clip.CustomPitchAndVolume ? clip.pitch : pitch;
        if (randomizePitch) _pitch += Random.Range(-pitchRandomizeAmount, pitchRandomizeAmount);

        actualVolume = clip.CustomPitchAndVolume ? clip.volume : volume;
        audioSource.volume = silent ? 0 : actualVolume;
        audioSource.pitch = _pitch;
        audioSource.loop = clip.looping;
        audioSource.clip = clip.clip;
        audioSource.Play();
    }

    Clip GetClip()
    {
        return clips[Random.Range(0, clips.Count)];
    }

    void FirstTimePlay(Transform caller, bool restart)
    {
        var Aman = AudioManager.i;
        if (!Aman) return;
        Aman.PlaySound(this, caller, restart);
    }

    void ConfigureSource(Clip clip, AudioSource source)
    {
        source.clip = clip.clip;
        source.volume = clip.volume;
        source.pitch = clip.pitch;
        source.loop = clip.looping;
    }
}
