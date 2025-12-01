using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class BuildMenu : MonoBehaviour
{
    public GameManager GM;
    public Building building;
    public BuyOption[] buyOptions = new BuyOption[3];
    public RectTransform background; //will need to scale this with the # of options (or don't...? just calculate the max possible upgrades per window and set that size)
    public bool defaultMenu = true;
    public bool workMenu = false;
    public CraftingStation craftingStation;
    public MartMenu martMenu;
    public StudyMenu studyMenu;
    public float boxSizeOffset = 60;

    public void OnEnable()
    {
        Populate();
    }

    public void Populate()
    {
        if (workMenu)
        {
            PopulateWork();
        }
        else
        {
            PopulateOptions();
        }

        if(craftingStation != null)
        {
            craftingStation.CheckQueueSprites();
        }
        if(studyMenu != null)
        {
            studyMenu.EnableMenu();
        }
    }

    public void PopulateWork()
    {

    }

    public void PopulateOptions()
    {
        int index = 0;
        int tempCost = 0;
        bool enabling = true;
        for(int ii = 0; ii < buyOptions.Length; ii++)
        {
            enabling = true;
            index = buyOptions[ii].optionIndex;
            Array.Resize(ref buyOptions[ii].costs, GM.upgrades[index].costs.Length);
            if (GM.upgrades[index].costs.Length == 1)//simplest version for things that only require 1 resource
            {
                buyOptions[ii].cost = GM.GetCost(index, GM.upgrades[index].baseCost);
                //buyOptions[ii].costText.text = buyOptions[ii].cost.ToString();
                tempCost = buyOptions[ii].cost;
                buyOptions[ii].costText.text = GM.ToKMB((decimal)tempCost);
                buyOptions[ii].currencyIndex = GM.upgrades[index].currencyIndex;
                buyOptions[ii].currencyIcon.sprite = GM.currencyIcons[GM.upgrades[index].currencyIndex];
                buyOptions[ii].currencyIcon.SetNativeSize();
            }
            else//multi-resource options
            {
                for(int jj = 0; jj < GM.upgrades[index].costs.Length; jj++)
                {
                    buyOptions[ii].costs[jj].currencyIndex = GM.upgrades[index].costs[jj].currencyIndex;
                    buyOptions[ii].costs[jj].cost = GM.GetCost(index, GM.upgrades[index].costs[jj].cost);
                    buyOptions[ii].costTexts[jj].text = buyOptions[ii].costs[jj].cost.ToString();
                    buyOptions[ii].currencyIcons[jj].sprite = GM.currencyIcons[GM.upgrades[index].costs[jj].currencyIndex];
                    buyOptions[ii].currencyIcons[jj].SetNativeSize();
                }
            }

            buyOptions[ii].nameText.text = GM.upgrades[index].upgradeName;
            buyOptions[ii].optionIcon.sprite = GM.upgrades[index].sprite;
            buyOptions[ii].optionIcon.SetNativeSize();

            if (buyOptions[ii].requiredSymbol != null)
            {
                bool showRequired = GM.upgrades[index].required;
                if (GM.upgrades[index].level > 1) { showRequired = false; }
                buyOptions[ii].requiredSymbol.SetActive(showRequired);
            }

            if (GM.upgrades[index].prerequisites.Count > 0)
            {
                enabling = CheckPrequisites(index);
            }
            if (GM.upgrades[index].level >= GM.upgrades[index].maxLevel) { enabling = false; }
            buyOptions[ii].gameObject.SetActive(enabling);
        }
        if (defaultMenu)//don't do this for the oddballs
        {
            int activeCount = 0;
            for (int ii = 0; ii < buyOptions.Length; ii++)
            {
                if (buyOptions[ii].gameObject.activeInHierarchy)
                {
                    activeCount++;
                }
            }
            background.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (activeCount * 55) + boxSizeOffset);
        }
    }

    public bool CheckPrequisites(int indexPass)
    {
        bool enabling = true;
        int tempIndex = 0;
        for (int jj = 0; jj < GM.upgrades[indexPass].prerequisites.Count; jj++)
        {
            tempIndex = GM.upgrades[indexPass].prerequisites[jj];
            if (GM.upgrades[tempIndex].level < 2)
            {
                enabling = false;
            }
        }
        if (GM.upgrades[indexPass].prestigeRequirement > 0)
        {
            if(GM.prestige.prestigeCount < GM.upgrades[indexPass].prestigeRequirement)
            {
                enabling = false;
            }
        }
        return enabling;
    }
}
