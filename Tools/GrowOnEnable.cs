using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowOnEnable : MonoBehaviour
{
    public float startScale, currentScale;
    public float increaseRate;
    public float maxSize;
    public bool shrink = false;
    public void OnEnable()
    {
        transform.localScale = Vector3.one * startScale;
        currentScale = startScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (shrink == false)
        {
            if (transform.localScale.x < maxSize)
            {
                currentScale += (Time.deltaTime * increaseRate);
                transform.localScale = new Vector3(currentScale, currentScale);
            }
        }
        else
        {
            if (transform.localScale.x > maxSize)
            {
                currentScale += (Time.deltaTime * increaseRate);
                transform.localScale = new Vector3(currentScale, currentScale);
            }
        }
    }
}
