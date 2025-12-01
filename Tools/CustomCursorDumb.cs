using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.UI.VirtualMouseInput;

public class CustomCursorDumb : MonoBehaviour
{
    public Image cursorImg;
    public GameObject cursorObj;
    public Texture2D blankCursor;
    public Texture2D defaultCursor, clickedCursor;
    public bool cursorEnabled = true;
    public enum CursorState
    {
        Default,
        EnemyHover,
        Walking,
        CastingSigil
    }
    public CursorState cursorState;

    public void Start()
    {
        if(cursorObj != null)
        {
            //ClearCursor();
            DefaultCursor();
        }
    }

    void Update()
    {
        //if (cursorEnabled)
        //{
        //    cursorObj.transform.position = Pointer.current.position.ReadValue();
        //}
        if (Mouse.current.leftButton.isPressed && cursorState == CursorState.Default)
        {
            Cursor.SetCursor(clickedCursor, Vector2.zero, UnityEngine.CursorMode.Auto);
            //cursorImg.sprite = clickedCursor;
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame && cursorState == CursorState.Default)
        {
            Cursor.SetCursor(defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto);
            //cursorImg.sprite = defaultCursor;
        }
    }

    public void ClearCursor()
    {
        Cursor.SetCursor(blankCursor, Vector3.zero, UnityEngine.CursorMode.ForceSoftware);
    }

    public void DefaultCursor()
    {
        //cursorImg.sprite = defaultCursor;
        Cursor.SetCursor(defaultCursor, Vector2.zero, UnityEngine.CursorMode.Auto);
        cursorState = CursorState.Default;
    }
}
