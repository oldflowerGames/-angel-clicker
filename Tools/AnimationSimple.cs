using System.Collections.Generic;
using UnityEngine;

public class AnimationSimple : MonoBehaviour
{
    public List<Sprite> sprites = new List<Sprite>();
    public SpriteRenderer spriteRenderer;
    public float frameTime = 0.5f;
    public int frameTracker;
    public bool looping = true;
    float frameTimer;
    bool active = true;

    private void OnEnable()
    {
        spriteRenderer.sprite = sprites[0];
        frameTimer = 0;
        frameTracker = 0;
        active = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(active == false) { return; }
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameTime)
        {
            frameTimer -= frameTime;
            frameTracker += 1;
            if (frameTracker >= sprites.Count)
            {
                if (looping == true)
                {
                    frameTracker = 0;
                }
                else
                {
                    frameTracker = sprites.Count - 1;
                    active = false;
                }
            }
            spriteRenderer.sprite = sprites[frameTracker];
        }
    }
}
