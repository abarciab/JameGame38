using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneController : MonoBehaviour
{
    [SerializeField] Fade fade;
    [SerializeField] MusicPlayer music;
    [SerializeField] Sound clickSound;

    [SerializeField] RectTransform contentParent;
    [SerializeField] float startPos = -250, scrollSpeed;

    private void Start()
    {
        clickSound = Instantiate(clickSound);
        fade.Hide();

        var pos = contentParent.anchoredPosition;
        pos.y = startPos;
        contentParent.anchoredPosition = pos;
    }

    private void Update()
    {
        if (contentParent.anchoredPosition.y < contentParent.rect.height) {
            contentParent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            if (contentParent.anchoredPosition.y > contentParent.rect.height) contentParent.anchoredPosition = new Vector2(0, contentParent.rect.height);
        }
    }

    public void Click()
    {
        clickSound.Play();
    }

    public void GoToMenu()
    {
        StartCoroutine(TransitionToGame());
    }

    IEnumerator TransitionToGame()
    {
        music.FadeOutCurrent(fade.fadeTime);
        fade.Appear();
        yield return new WaitForSeconds(fade.fadeTime + 0.5f);
        Destroy(AudioManager.i.gameObject);
        SceneManager.LoadScene(0);
    }
}
