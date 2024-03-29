using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager i;
    private void Awake() { i = this; }

    [SerializeField] GameObject coordinatorPrefab;
    List<SoundCoordinator> soundCoordinators = new List<SoundCoordinator>();

    [Header("Mixers & Volume")]
    [SerializeField] AudioMixer mixer;
    [SerializeField] float masterVolume, musicVolume, ambientVolume, sfxVolume;

    public Vector4 Volumes { get { return new Vector4(masterVolume, musicVolume, ambientVolume, sfxVolume); } }

    public void FadeOutMaster(float time)
    {
        StartCoroutine(AnimateMasterFade(time));
    }

    IEnumerator AnimateMasterFade(float time)
    {
        float original = masterVolume;
        float timePassed = 0;
        while (timePassed < time) {
            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
            SetMasterVolume(Mathf.Lerp(original, 0, timePassed / time));
        }
    }

    private void Start()
    {
        LoadVolumeValuesFromSaveData();
        SetMixerVolumes();
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("ambientVolume", ambientVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }

    public void Pause()
    {
        foreach (var s in soundCoordinators) s.Pause();
    }

    public void Resume()
    {
        foreach (var s in soundCoordinators) s.Resume();
    }

    public void ResetVolumeSaveData()
    {

    }

    void LoadVolumeValuesFromSaveData()
    {
        masterVolume = PlayerPrefs.GetFloat("masterVolume", masterVolume);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", musicVolume);
        ambientVolume = PlayerPrefs.GetFloat("ambientVolume", ambientVolume);
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", sfxVolume);
    }

    public AudioMixerGroup GetMixer(SoundType type)
    {
        switch (type) {
            case SoundType.sfx:
                return mixer.FindMatchingGroups("Sfx")[0];
            case SoundType.music:
                return mixer.FindMatchingGroups("Music")[0];
            case SoundType.ambient:
                return mixer.FindMatchingGroups("Ambient")[0];
        }
        return null;
    }

    public void SetMasterVolume(float vol)
    {
        masterVolume = vol;
        SetMixerVolumes();
    }

    public void SetSfxVolume(float vol)
    {
        sfxVolume = vol;
        SetMixerVolumes();
    }

    public void SetMusicVolume(float vol)
    {
        musicVolume = vol;
        SetMixerVolumes();
    }

    void SetMixerVolumes()
    {
        float max = 20;

        mixer.SetFloat("masterVolume", Mathf.Log10(masterVolume) * max);
        mixer.SetFloat("sfxVolume", Mathf.Log10(sfxVolume) * max);
        mixer.SetFloat("musicVolume", Mathf.Log10(musicVolume) * max);

        SaveVolume();
    }

    public void SetAmbientVolume(float vol)
    {
        ambientVolume = vol;
        mixer.SetFloat("ambientVolume", Mathf.Log10(ambientVolume) * 20);
    }

    public void PlaySound(Sound sound, Transform caller, bool restart = true)
    {
        if (caller == null) caller = transform;
        var coordinator = GetExistingCoordinator(caller);
        coordinator.AddNewSound(sound, restart, caller != transform);
    }

    SoundCoordinator GetExistingCoordinator(Transform caller)
    {
        for (int i = 0; i < soundCoordinators.Count; i++) {
            var coord = soundCoordinators[i];
            if (coord == null || coord.transform.parent == null) soundCoordinators.RemoveAt(i);
        }

        foreach (var coord in soundCoordinators) {
            if (coord && coord.transform.parent == caller) return coord;
        }
        return AddNewCoordinator(caller);
    }

    SoundCoordinator AddNewCoordinator(Transform caller)
    {
        var coordObj = Instantiate(coordinatorPrefab, caller);
        var coord = coordObj.GetComponent<SoundCoordinator>();
        soundCoordinators.Add(coord);
        return coord;
    }

    private void OnDestroy()
    {
        if (i == this) i = null;
    }
}
