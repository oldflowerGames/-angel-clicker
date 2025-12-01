using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuyOption : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameManager GM;
    public int optionIndex;
    public int cost
    {
        get { return costs[0].cost; }
        set { costs[0].cost = value; }
    }
    public int currencyIndex
    {
        get { return costs[0].currencyIndex; }
        set { costs[0].currencyIndex = value; }
    }
    public Image optionIcon;

    public Image currencyIcon
    {
        get { return currencyIcons[0]; }
        set { currencyIcons[0] = value; }
    }
    public Image[] currencyIcons = new Image[3];

    public TextMeshProUGUI nameText;

    public TextMeshProUGUI costText
    {
        get { return costTexts[0]; }
        set { costTexts[0] = value; }
    }
    public TextMeshProUGUI[] costTexts = new TextMeshProUGUI[3];

    public Button purchaseButton;

    public bool craftable;
    public int craftableIndex;

    public bool martOption;
    public bool studyTotalLevelOption;

    public CostData[] costs = new CostData[3];
    public RectTransform rectTransform;
    public float tooltipOffset = 0.8f;

    public GameObject requiredSymbol;

    bool available = false;

    public void Start()
    {
        if(rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
    }

    public void PurchaseThis()
    {
        if (Keyboard.current.shiftKey.isPressed)
        {
            GM.PurchaseMultiOption(optionIndex, this);
        }
        else
        {
            GM.PurchaseOption(optionIndex, this, false);
            GM.sfx.PlaySound(GM.sfx.upgrade);
        }
        GM.tooltip.PopulateUpgradeTooltip(this);
    }

    public void LateUpdate()
    {
        if (GM == null) { return; }
        if(costs.Length == 1)
        {
            if (cost > GM.currencies[currencyIndex]) { purchaseButton.interactable = false; }
            else { purchaseButton.interactable = true; }
        }
        else
        {
            available = true;
            for(int ii = 0; ii < costs.Length; ii++)
            {
                if (costs[ii].cost > GM.currencies[costs[ii].currencyIndex]) { available = false; }
            }

            purchaseButton.interactable = available;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GM == null) { return; }

        GM.tooltip.tooltip.gameObject.SetActive(true);
        GM.tooltip.PopulateUpgradeTooltip(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GM == null) { return; }

        GM.tooltip.tooltip.gameObject.SetActive(false);
    }
}
