using UnityEngine;

public class UIFollow : MonoBehaviour
{
    public Camera cam;
    public CameraController controller;
    public float[] offsetMultipliers = new float[3];
    public GameObject toFollow;
    public Vector3 offset = Vector3.zero;
    public RectTransform rectTransform;
    Vector3 camPos;
    Vector3 lastPos;
    float camZoom;
    float lastZoom;
    Vector3 lastOffset;
    Vector3 newPos;
    Vector3 tempPos;
    Vector3 intPos;
    float amountOver;
    float effectiveSize;
    float screenRatio;

    public void OnEnable()
    {
        lastOffset = offset * 0.99f;//just to make it always check OnEnable
        //CheckPosition(true);
        if(rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    public void Update()
    {
        CheckPosition(false);
    }

    public void CheckPosition(bool overrideCheck)
    {
        camPos = cam.transform.position;    
        camZoom = cam.orthographicSize;
        if (camPos == lastPos && camZoom == lastZoom && offset == lastOffset && overrideCheck == false) { return; }

        //screen space offset
        //tempPos = cam.WorldToScreenPoint(toFollow.transform.position);
        //newPos = tempPos + offset;

        //world space offset
        tempPos = cam.WorldToScreenPoint(toFollow.transform.position + (offset * offsetMultipliers[controller.zoomLevel]));
        newPos = tempPos;

        screenRatio = ((float)Screen.width / (float)1550);
        effectiveSize = (rectTransform.sizeDelta.x / 2) * screenRatio;

        if (newPos.x > Screen.width - effectiveSize)
        {
            amountOver = Screen.width - effectiveSize - newPos.x;
            newPos.x += amountOver;
        }
        else if(newPos.x < effectiveSize)
        {
            newPos.x = effectiveSize;
        }

        screenRatio = ((float)Screen.height / (float)588);
        effectiveSize = (rectTransform.sizeDelta.y / 2) * screenRatio;
        if (newPos.y > Screen.height - effectiveSize)
        {
            amountOver = Screen.height - effectiveSize - newPos.y;
            newPos.y += amountOver;
        }
        else if (newPos.y < effectiveSize)
        {
            newPos.y = effectiveSize;
        }

        intPos = new Vector3(Mathf.RoundToInt(newPos.x), Mathf.RoundToInt(newPos.y), Mathf.RoundToInt(newPos.z));

        //gameObject.transform.position = newPos;
        gameObject.transform.position = intPos;

        lastPos = cam.transform.position;
        lastZoom = cam.orthographicSize;
        lastOffset = offset;
    }
}
