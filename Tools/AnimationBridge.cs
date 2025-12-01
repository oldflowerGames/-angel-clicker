using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AnimationBridge : MonoBehaviour
{
    public bool watchXPos, watchYPos, watchXScale, watchYScale, watchZRot;
    public float xPos, yPos, zRot;
    public float xScale = 1;
    public float yScale = 1;
    public float xPosMult = 1;
    public float yPosMult = 1;
    public float zRotMult = 1;
    public Transform transfo;
    public bool debugWatch;

    void LateUpdate()
    {
        if (watchXPos && watchYPos)
        {
            transfo.localPosition = new Vector2(xPos * xPosMult, yPos * yPosMult);
        }
        else
        {
            if (watchXPos)
            {
                transfo.localPosition = new Vector2(xPos * xPosMult, transfo.localPosition.y);
                if (debugWatch) { Debug.Log("X Pos: " + xPos); }
            }
            if (watchYPos)
            {
                transfo.localPosition = new Vector2(transfo.localPosition.x, yPos * yPosMult);
                if (debugWatch) { Debug.Log(yPosMult); }
            }
        }
        if (watchXScale && watchYScale)
        {
            transfo.localScale = new Vector2(xScale, yScale);
        }
        else
        {
            if (watchXScale)
            {
                transfo.localScale = new Vector2(xScale, transfo.localScale.y);
            }
            if (watchYScale)
            {
                transfo.localScale = new Vector2(transfo.localScale.x, yScale);
            }
        }

        if (watchZRot)
        {
            transfo.localRotation = Quaternion.Euler(transfo.localRotation.x, transfo.localRotation.y, zRot * zRotMult);
        }
    }

    public void SetYPosMult(float val)
    {
        yPosMult = val;
        //Debug.Log("yPosMult set to: " + yPosMult.ToString());
    }

    public void SetXPosMult(float val)
    {
        xPosMult = val;
    }

    public void ResetMultipliers()
    {
        SetYPosMult(1);
        xPosMult = 1;
    }
}
