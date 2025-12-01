using UnityEngine;
using UnityEngine.InputSystem;

public class FollowCursor : MonoBehaviour
{
    public Camera cam;
    public float zValue = 0;
    Vector3 newPos;
    // Update is called once per frame
    void Update()
    {
        newPos = cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        newPos.z = zValue;

        gameObject.transform.position = newPos;
    }
}
