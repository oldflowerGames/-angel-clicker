using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepSpriteSize : MonoBehaviour
{
    public float targetHeight = 1f;
    // Start is called before the first frame update
    void Start()
    {
        SetSize();
    }

    public void SetSize()
    {
        var bounds = GetComponent<SpriteRenderer>().sprite.bounds;
        var factor = targetHeight / bounds.size.y; 
        transform.localScale = new Vector3(factor, factor, factor);
    }
}
