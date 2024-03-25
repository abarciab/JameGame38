using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Sound clickSound;
    [SerializeField] Fade fade;
    [SerializeField] MusicPlayer music;

    private void Start()
    {
        clickSound = Instantiate(clickSound);
        fade.Hide();
    }

    public void Click()
    {
        clickSound.Play();
    }

    public void StartGame()
    {
        StartCoroutine(TransitionToGame());
    }

    IEnumerator TransitionToGame()
    {
        music.FadeOutCurrent(fade.fadeTime);
        fade.Appear();
        yield return new WaitForSeconds(fade.fadeTime + 0.5f);
        Destroy(AudioManager.i.gameObject);
        SceneManager.LoadScene(1);
    }

}
