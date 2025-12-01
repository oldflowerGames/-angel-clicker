using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeInOnEnable : MonoBehaviour
{
    public CanvasGroup canvas;
    public float fadeRate = 2;
    public float startDelay = 0;
    public float goalAlpha = 1f;
    public bool startFromCurrent = false;

    public void OnEnable()
    {
        canvas = gameObject.GetComponent<CanvasGroup>();
        StartCoroutine(FadeIn());
    }

    public void Reset()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (canvas == null)
            {
                canvas = GetComponent<CanvasGroup>();
            }
        }
    }
    IEnumerator FadeIn()
    {
        if (startFromCurrent == false)
        {
            canvas.alpha = 0;
        }

        if (startDelay != 0)
        {
            yield return new WaitForSecondsRealtime(startDelay);
        }
        while (canvas.alpha < goalAlpha)
        {
            yield return new WaitForEndOfFrame();
            canvas.alpha += Time.unscaledDeltaTime * fadeRate;
        }
        canvas.alpha = goalAlpha;
    }
}
