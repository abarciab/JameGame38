using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] Slider masterVolume, musicVolume, ambientVolume, sfxVolume;
    [SerializeField] Sound clickSound;

    private void Start()
    {
        masterVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetMasterVolume(value));
        musicVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetMusicVolume(value));
        ambientVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetAmbientVolume(value));
        sfxVolume.onValueChanged.AddListener((float value) => AudioManager.i.SetSfxVolume(value));
        clickSound = Instantiate(clickSound);
    }

    private void OnEnable()
    {
        SetSliderValuesToAudioSettings();
    }

    void SetSliderValuesToAudioSettings()
    {
        var volumes = AudioManager.i.Volumes;
        masterVolume.value = volumes[0];
        musicVolume.value = volumes[1];
        ambientVolume.value = volumes[2];
        sfxVolume.value = volumes[3];
    }

    public void Resume()
    {
        GameManager.i.Resume();
        clickSound.Play();
    }

    public void ExitToMainMenu()
    {
        GameManager.i.LoadMenu();
        clickSound.Play();
    }
}
