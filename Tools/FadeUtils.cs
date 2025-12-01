using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeUtils : MonoBehaviour
{
    public void StartFadeOut(CanvasGroup pass, float goal = 0, float rate = 1, float delay = 0, float startAlpha = 1)
    {
        StartCoroutine(FadeOut(pass, goal, rate, delay, startAlpha));
    }

    public void StartFadeOut(GameObject pass, float goal = 0, float rate = 1)
    {
        CanvasGroup canvas = pass.GetComponent<CanvasGroup>();
        if (canvas != null)
        {
            StartCoroutine(FadeOut(canvas, goal, rate));
        }
    }

    public IEnumerator FadeOut(CanvasGroup pass, float goal = 0, float rate = 1, float delay = 0, float startAlpha = 1)
    {
        float alph = startAlpha;
        pass.alpha = alph;

        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }

        while (alph > goal)
        {
            yield return new WaitForEndOfFrame();
            alph -= Time.deltaTime * rate;
            pass.alpha = alph;
        }
        if (goal <= 0)
        {
            pass.gameObject.SetActive(false);
        }
    }

    public void StartFadeIn(CanvasGroup pass, float rate = 1, float startAlpha = 0, float delay = 0)
    {
        StartCoroutine(FadeIn(pass, rate, startAlpha, delay));
    }

    public void StartFadeIn(GameObject pass, float rate = 1, float startAlpha = 0, float delay = 0)
    {
        CanvasGroup canvas = pass.GetComponent<CanvasGroup>();
        if (canvas != null)
        {
            StartCoroutine(FadeIn(canvas, rate, startAlpha, delay));
        }
    }

    public IEnumerator FadeIn(CanvasGroup pass, float rate, float startAlpha = 0, float delay = 0)
    {
        float alph = startAlpha;
        pass.gameObject.SetActive(true);
        pass.alpha = alph;
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        while (alph < 1)
        {
            yield return new WaitForEndOfFrame();
            alph += Time.deltaTime * rate;
            pass.alpha = alph;
        }
    }



    public void StartFadeInImage(Image i, float rate)
    {
        i.gameObject.SetActive(true);
        StartCoroutine(FadeInImage(i, rate));
    }

    public IEnumerator FadeInImage(Image img, float rate)
    {
        float alph = img.color.a;
        while (alph < 1)
        {
            yield return new WaitForEndOfFrame();
            alph += Time.deltaTime * rate;
            img.color = new Color(1, 1, 1, alph);
        }
    }


    public void StartFadeOutImage(Image i, float rate, bool disable)
    {
        StartCoroutine(FadeOutImage(i, rate, disable));
    }

    public IEnumerator FadeOutImage(Image img, float rate, bool disable)
    {
        float alph = img.color.a;
        while (alph > 0)
        {
            yield return new WaitForEndOfFrame();
            alph -= Time.deltaTime * rate;
            img.color = new Color(1, 1, 1, alph);
        }
        if (disable)
        {
            img.gameObject.SetActive(false);
        }
    }

    public void StartFadeInSprite(SpriteRenderer i, float rate)
    {
        i.gameObject.SetActive(true);
        StartCoroutine(FadeInSprite(i, rate));
    }

    public IEnumerator FadeInSprite(SpriteRenderer img, float rate)
    {
        float alph = img.color.a;
        while (alph < 1)
        {
            yield return new WaitForEndOfFrame();
            alph += Time.deltaTime * rate;
            img.color = new Color(1, 1, 1, alph);
        }
    }


    public void StartFadeOutSprite(SpriteRenderer i, float rate, bool disable, float delay)
    {
        StartCoroutine(FadeOutSprite(i, rate, disable, delay));
    }

    public IEnumerator FadeOutSprite(SpriteRenderer img, float rate, bool disable, float delay)
    {
        if(delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        float alph = img.color.a;
        while (alph > 0)
        {
            yield return new WaitForEndOfFrame();
            alph -= Time.deltaTime * rate;
            img.color = new Color(1, 1, 1, alph);
        }
        if (disable)
        {
            img.gameObject.SetActive(false);
        }
    }

    public void StartColorFadeSprite(SpriteRenderer i, float rate, Color startColor, Color endColor, float delay)
    {
        StartCoroutine(ColorFadeSprite(i, rate, startColor, endColor, delay));
    }

    public IEnumerator ColorFadeSprite(SpriteRenderer img, float rate, Color startColor, Color endColor, float delay)
    {
        img.color = startColor;
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }
        float progress = 0;
        while (progress < 1)
        {
            yield return new WaitForEndOfFrame();
            progress += Time.deltaTime * rate;
            img.color = Color.Lerp(startColor, endColor, progress);
        }
        img.color = endColor;
    }

}
