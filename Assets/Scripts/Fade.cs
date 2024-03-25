using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    [SerializeField] Gradient fadeGradient;
    public float fadeTime;
    [SerializeField] Image img;

    public void Appear()
    {
        gameObject.SetActive(true);
        img.raycastTarget = true;
        StopAllCoroutines();
        StartCoroutine(AnimateFade(false));
    }

    public void Hide()
    {
        gameObject.SetActive(true);
        img.raycastTarget = false;
        StopAllCoroutines();
        StartCoroutine(AnimateFade(true));
    }

    IEnumerator AnimateFade(bool reverse)
    {
        float timePassed = 0;
        while (timePassed < fadeTime) {
            float progress = timePassed / fadeTime;
            if (reverse) progress = 1 - progress;

            img.color = fadeGradient.Evaluate(progress);

            timePassed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        img.color = fadeGradient.Evaluate(reverse ? 0 : 1);
        if (reverse) gameObject.SetActive(false);
    }
}
