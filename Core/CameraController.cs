using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;

public class CameraController : MonoBehaviour
{
    public GameManager GM;
    public int zoomLevel = 1;
    public int maxZoom = 2;
    public float minX, minY, maxX, maxY;
    public float[] minXVals = new float[3];
    public float[] minYVals = new float[3];
    public float[] maxXVals = new float[3];
    public float[] maxYVals = new float[3];
    public bool zoomLock = false;
    public bool inputLock = false;
    public Camera cam;
    public float edgeScrollRate = 1f;
    public float edgeMargin = 0.05f;
    public PixelPerfectCamera pixelCam;
    Vector3 screenToWorld;
    Vector3 pointerPos;
    Vector3 edgeScrollVector;
    bool edgeScrollThisFrame;

    public void ZoomIn()
    {
        if (zoomLevel <= 0) { return; }
        if (zoomLock) { return; }
        zoomLevel = zoomLevel - 1;
        SetZoom();
    }

    public void ZoomOut()
    {
        if (zoomLevel >= maxZoom) { return; }
        if (zoomLock) { return; }
        zoomLevel = zoomLevel + 1;
        SetZoom();
    }

    public void SetZoom()
    {
        switch (zoomLevel)
        {
            case 0:
                //cam.orthographicSize = 3.5f;
                pixelCam.assetsPPU = 150;
                //adjust UI sizes?
                break;

            case 1:
                pixelCam.assetsPPU = 100;
                //cam.orthographicSize = 5f;
                break;

            case 2:
                pixelCam.assetsPPU = 50;
                //cam.orthographicSize = 6.4f;
                break;
        }

        //set new min/max vals
        minX = minXVals[zoomLevel];
        minY = minYVals[zoomLevel];
        maxX = maxXVals[zoomLevel];
        maxY = maxYVals[zoomLevel];

        //check bounds
        Vector3 newPos = cam.transform.position;
        if(cam.transform.position.x < minX) { newPos.x = minX; }
        if (cam.transform.position.x > maxX) { newPos.x = maxX; }
        if (cam.transform.position.y < minY) { newPos.y = minY; }
        if (cam.transform.position.y > maxY) { newPos.y = maxY; }
        cam.transform.position = newPos;

    }

    private Vector3 Origin;
    private Vector3 Difference;
    private bool Drag = false;
    Vector3 newPos = Vector3.zero;
    Vector3 clampPos = Vector3.zero;

    void LateUpdate()
    {
        //if(GM.playerInput.heldBuilders.Count > 0) { return; }
        if (inputLock) { return; }
        if(Pointer.current.press.isPressed)
        {
            pointerPos = Pointer.current.position.ReadValue();
            screenToWorld = cam.ScreenToWorldPoint(pointerPos);
            edgeScrollThisFrame = false;
            if (Drag == false)
            {
                Drag = true;
                Origin = cam.ScreenToWorldPoint(pointerPos);
            }
            if (GM.playerInput.heldBuilders.Count == 0) 
            {
                Difference = screenToWorld - cam.transform.position;
            }
            else
            {
                edgeScrollVector = Vector3.zero;
                if(pointerPos.x <= Screen.width * edgeMargin)
                {
                    edgeScrollVector.x = Mathf.Lerp(-1, 0, pointerPos.x / (Screen.width * edgeMargin));
                }
                if(pointerPos.x >= Screen.width * (1 - edgeMargin))
                {
                    edgeScrollVector.x = Mathf.Lerp(0, 1, (pointerPos.x - (Screen.width * (1 - edgeMargin))) / (Screen.width * edgeMargin));
                }
                if(pointerPos.y <= Screen.height * edgeMargin)
                {
                    edgeScrollVector.y = Mathf.Lerp(-1, 0, pointerPos.y / (Screen.height * edgeMargin));
                }
                if(pointerPos.y >= Screen.height * (1 - edgeMargin))
                {
                    edgeScrollVector.y = Mathf.Lerp(0, 1, (pointerPos.y - (Screen.height * (1 - edgeMargin))) / (Screen.height * edgeMargin));
                }
                if(edgeScrollVector != Vector3.zero)
                {
                    Drag = true;
                    Difference = edgeScrollVector * Time.deltaTime * edgeScrollRate;
                    edgeScrollThisFrame = true;
                }
                else
                {
                    Drag = false;
                }
            }
        }
        else
        {
            Drag = false;
        }
        if (Drag == true)
        {
            if (edgeScrollThisFrame)
            {
                newPos = cam.transform.position + Difference;
            }
            else
            {
                newPos = Origin - Difference;
            }

            clampPos.x = Mathf.Clamp(newPos.x, minX, maxX);
            clampPos.y = Mathf.Clamp(newPos.y, minY, maxY);
            clampPos.z = -10f;
            cam.transform.position = clampPos;

        }
        ////RESET CAMERA TO STARTING POSITION WITH RIGHT CLICK
        //if (Input.GetMouseButton(1))
        //{
        //    Camera.main.transform.position = ResetCamera;
        //}

        if (Mouse.current.scroll.ReadValue().y > 0)
        {
            ZoomIn();
        }
        if (Mouse.current.scroll.ReadValue().y < 0)
        {
            ZoomOut();
        }

    }
}

    //public void DetectMovement()
    //{
    //    Vector3 screenPosition = Pointer.current.position.ReadValue();

    //    bool isDown = Pointer.current.press.isPressed;
    //    bool wentDown = Pointer.current.press.wasPressedThisFrame;

    //    // The camera is where you are regarding the world from.
    //    ProcessTouch(cam, screenPosition: screenPosition, isDown: isDown, wentDown: wentDown);
    //}

    //void ProcessTouch(Camera viewCamera, Vector3 screenPosition, bool isDown, bool wentDown)
    //{
    //    var ray = cam.ScreenPointToRay(screenPosition);

    //    float distance = 0;
    //    Vector3 worldPoint = ray.GetPoint(distance);

    //    if (wentDown)
    //    {
    //        // on the frame the mouse went down, by definition we didn't move at all
    //        PreviousWorldPoint = worldPoint;
    //    }

    //    if (isDown)
    //    {
    //        if (Vector3.Distance(worldPoint, PreviousWorldPoint) > 0.01f)//reduce tiny accidental movements.
    //        {
    //            // we are down, how much did we move in world space?
    //            Vector3 worldDelta = worldPoint - PreviousWorldPoint;

    //            worldDelta = -worldDelta;//invert
    //            //worldDelta *= 2;

    //            // move the map that much
    //            if (pointToScroll)
    //            {

    //                //observe clamped values
    //                tempSlidePos = pointToScroll.position + worldDelta;
    //                //Vector3 camOffset = camControl.gameObject.transform.position;
    //                tempSlidePos.x = Mathf.Clamp(tempSlidePos.x, minX, maxX);
    //                tempSlidePos.y = Mathf.Clamp(tempSlidePos.y, minY, maxY);
    //                tempSlidePos.z = -10;
    //                pointToScroll.position = tempSlidePos;

    //                //MapToScroll.position += worldDelta;
    //            }
    //        }

    //    }

    //    PreviousWorldPoint = worldPoint;
    //}
