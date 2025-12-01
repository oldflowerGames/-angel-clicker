using UnityEngine;
using UnityEngine.EventSystems;

public class CancelNet : MonoBehaviour, IPointerDownHandler
{
    public GameManager GM;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GM.CloseAllBuildMenus();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {

        }
    }
}
