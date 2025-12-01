using UnityEngine;

public class FadeSpriteOnEnable : MonoBehaviour
{
    public float rate = 1f;
    public float progress = 1f;
    public Color startColor = Color.white;
    public Color endColor = Color.clear;
    public SpriteRenderer spriteRenderer;
    public bool disable = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        spriteRenderer.color = startColor;
        progress = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(progress > 1f) { return; }
        progress += Time.deltaTime * rate;
        spriteRenderer.color = Color.Lerp(startColor, endColor, progress);
        if(progress > 1f && disable) 
        {
            gameObject.SetActive(false);
        }
    }
}
