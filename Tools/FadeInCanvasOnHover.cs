using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FadeInCanvasOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CanvasGroup canvas;
    public float alpha = 0;
    public float minAlpha = 0;
    public float startAlpha = 0;
    public float fadeInRate = 1f;
    public float fadeOutRate = 2f;
    public bool hovering = false;

    private void Reset()
    {
        canvas = gameObject.GetComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        alpha = startAlpha;
        hovering = false;
    }



    // Update is called once per frame
    void Update()
    {
        if (hovering)
        {
            alpha += Time.deltaTime * fadeInRate;
            alpha = Mathf.Min(alpha, 1);
            canvas.alpha = alpha;
        }
        else
        {
            alpha -= Time.deltaTime * fadeOutRate;
            alpha = Mathf.Max(alpha, minAlpha);
            canvas.alpha = alpha;
        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }
}
