using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingBackground : MonoBehaviour
{
    public GameObject objToMove;
    public float moveRate;
    public float loopPoint;
    public float currentPos;
    float moveToAdd;
    public enum Axis
    {
        X,
        Y
    }
    public Axis axis;

    // Update is called once per frame
    void Update()
    {
        moveToAdd = moveRate * Time.deltaTime;
        if (loopPoint < 0) { moveToAdd *= -1; }

        currentPos += moveToAdd;

        if (loopPoint < 0)
        {
            if (currentPos <= loopPoint)
            {
                currentPos -= loopPoint;
            }
        }
        else
        {
            if (currentPos >= loopPoint)
            {
                currentPos -= loopPoint;
            }
        }
        if (axis == Axis.X)
        {
            objToMove.transform.localPosition = new Vector3(currentPos, 0, 0);
        }
        if (axis == Axis.Y)
        {
            objToMove.transform.localPosition = new Vector3(0, currentPos, 0);
        }
    }
}
