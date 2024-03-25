using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AmbientSound
{
    [HideInInspector] public string name;
    public Sound sound;
    public Vector2 waitTimeRange = new Vector2(4, 8);
    public float distanceFromCamera = 2;
    [HideInInspector] public float cooldown;
    [HideInInspector] public Transform transform;

    public void Play()
    {
        transform.localPosition = Random.insideUnitSphere * distanceFromCamera;
        sound.Play(transform);
        cooldown = Random.Range(waitTimeRange.x, waitTimeRange.y);
    }
}

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] Sound mainMusic, altMusic, pauseMusic, ambientLoop;
    [SerializeField] Vector2 silenceWaitRange = new Vector2(1, 10);
    [SerializeField] bool fadeIn = true;
    public bool playAltMusic;
    bool playingMainMusic, fadingOut;
    float mainMusicTimeLeft;
    

    [Header("Ambience")]
    [SerializeField] List<AmbientSound> ambientSounds = new List<AmbientSound>();

    private void OnValidate()
    {
        foreach (var a in ambientSounds) if (a.sound != null) a.name = a.sound.name;
    }

    private void Start()
    {
        foreach (var a in ambientSounds) {
            a.sound = Instantiate(a.sound);
            a.transform = new GameObject(a.name + " source (AmbientSound)").transform;
            a.transform.parent = Camera.main.transform;
            a.cooldown = Random.Range(a.waitTimeRange.x, a.waitTimeRange.y);
        }

        mainMusic = Instantiate(mainMusic);
        if (ambientLoop) ambientLoop = Instantiate(ambientLoop);
        if (pauseMusic) pauseMusic = Instantiate(pauseMusic);
        if (altMusic) altMusic = Instantiate(altMusic);

        if (pauseMusic) pauseMusic.PlaySilent();
        if (altMusic) altMusic.PlaySilent();
        if (ambientLoop) {
            if (fadeIn) ambientLoop.PlaySilent();
            else ambientLoop.Play();
        }

        StartNext(fadeIn);
    }

    public void FadeOutCurrent(float time)
    {
        mainMusicTimeLeft = Mathf.Infinity;
        StartCoroutine(FadeOut(time));
    }

    IEnumerator FadeOut(float time)
    {
        float mainStart = mainMusic.percentVolume;
        float ambientStart = ambientLoop ? ambientLoop.percentVolume : 0;
        float altStart = altMusic ? altMusic.percentVolume : 0;
        float pauseStart = pauseMusic ? pauseMusic.percentVolume : 0;

        fadingOut = true;
        float timePassed = 0;
        while (timePassed < time) {
            float progress = timePassed / time;

            mainMusic.PercentVolume(Mathf.Lerp(mainStart, 0, progress));
            if (ambientLoop) ambientLoop.PercentVolume(Mathf.Lerp(ambientStart, 0, progress));
            if (altMusic) altMusic.PercentVolume(Mathf.Lerp(altStart, 0, progress));
            if (pauseMusic) pauseMusic.PercentVolume(Mathf.Lerp(pauseStart, 0, progress));

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    void StartNext(bool silent = false)
    {
        if (silent) mainMusic.PlaySilent();
        else mainMusic.Play();
        mainMusicTimeLeft = mainMusic.GetClipLength();
        playingMainMusic = true;
    }

    private void Update()
    {
        if (fadingOut) return;

        foreach (var a in ambientSounds) {
            a.cooldown -= Time.deltaTime;
            if (a.cooldown <= 0) a.Play();
        }

        float pausedMod = Time.timeScale == 0 ? 0 : 1;
        if (playAltMusic && altMusic) PlayAltMusic(pausedMod);
        else PlayNormalMusic(pausedMod);

        if (ambientLoop) ambientLoop.PercentVolume(pausedMod, 0.01f);
        if (pauseMusic) pauseMusic.PercentVolume(1 - pausedMod, 0.01f);
    }

    void PlayAltMusic(float pausedMod)
    {
        mainMusic.PercentVolume(0, 0.1f);
        altMusic.PercentVolume(1 * pausedMod, 0.1f);
    }

    void PlayNormalMusic(float pausedMod)
    {
        if (playingMainMusic) mainMusic.PercentVolume(1 * pausedMod, 0.05f);
        if (altMusic) altMusic.PercentVolume(0, 0.1f);

        mainMusicTimeLeft -= Time.deltaTime;
        if (mainMusicTimeLeft > 0) return;

        if (!playingMainMusic) {
            StartNext();
            return;
        }
        playingMainMusic = false;
        mainMusicTimeLeft = Random.Range(silenceWaitRange.x, silenceWaitRange.y);
    }
}
