using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeOutImageOnEnable : MonoBehaviour
{
    public Image sprite;
    public float alpha;
    public float fadeRate = 8;
    public float holdTime;
    public float startAlpha = 1;
    public float endAlpha = 0;
    public bool autoDisable = true;
    public bool unscaledTime = false;
    public float startDelay = 0;

    public void OnEnable()
    {
        if (sprite == null) { sprite = gameObject.GetComponent<Image>(); }
        StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut()
    {
        if (startDelay != 0)
        {
            yield return new WaitForSecondsRealtime(startDelay);
        }

        alpha = startAlpha;
        SetColor(alpha);
        if (unscaledTime) { yield return new WaitForSecondsRealtime(holdTime); }
        else { yield return new WaitForSeconds(holdTime); }

        while (alpha > endAlpha)
        {
            yield return new WaitForEndOfFrame();
            if (unscaledTime) { alpha -= Time.unscaledDeltaTime * fadeRate; }
            else { alpha -= Time.deltaTime * fadeRate; }
            SetColor(alpha);
        }

        SetColor(endAlpha);
        if (autoDisable) { gameObject.SetActive(false); }
    }

    public void SetColor(float alphaPass)
    {
        sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alphaPass);
    }
}
