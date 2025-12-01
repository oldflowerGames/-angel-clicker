using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

public class PrestigeOption : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public PrestigeManager manager;
    public bool available = false;
    public bool selected = false;
    public int optionIndex;
    public Image img;
    public TextMeshProUGUI nameText;
    public Color unavailableColor = Color.black;
    public Color unselectedColor = Color.grey;

    public void SetData(PrestigeData data)
    {
        nameText.text = data.nameID;
        optionIndex = data.index;
        img.sprite = data.sprite;
    }

    public void SetSelected()
    {
        img.color = Color.white;
        available = true;
        selected = true;
    }

    public void SetUnavailable()
    {
        img.color = unavailableColor;
        available = false;
        selected = false;
    }

    public void SetAvailable()
    {
        img.color = unselectedColor;
        available = true;
        selected = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (available == false) { return; }
        if(manager.canSelect == false) { return; }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (selected) { return; }
            ActivateThis();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            DeactivateThis();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (available == false) { return; }
        //if (manager.canSelect == false) { return; }
        manager.PopulateTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        manager.DisableTooltip();
    }

    public void ActivateThis()
    {
        manager.AddOption(optionIndex, false);
        manager.GM.sfx.PlaySound(manager.GM.sfx.selectPrestige);
    }

    public void DeactivateThis()
    {
        manager.RemoveOption(optionIndex);
    }
}
