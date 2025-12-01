using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PrestigeManager : MonoBehaviour
{
    public GameManager GM;
    public int prestigeLevel = 0;
    public int prestigeCount = 0;
    public int[] prestigeExpToLevel = new int[3];

    public bool canSelect = true;

    public List<int> activeOptions = new List<int>();
    
    public PrestigeOption[] options = new PrestigeOption[20];

    public bool unlockedDoubleRate, unlockedDoubleBuildRate;
    public bool unlockedGroupPickup, unlockedGatheringUpgrade1, unlockedStartingWell, unlockedStartingWorkers1;

    public bool unlockedQuadRate, unlockedQuadBuildRate, unlockedStartingResources1;
    public bool unlockedAutoAssign, unlockedGatheringUpgrade2, unlockedUnlimitedLifespan, unlockedFasterCrafting1;

    public bool unlockedTenRate, unlockedTenBuildRate, unlockedStartingResources2;
    //public bool unlockedEyeHQ, unlockedGatheringUpgrade3, unlockedFasterCrafting2, unlockedStartingWorkers2;

    public CanvasGroup canvasGroup;
    public TextMeshProUGUI availablePointsText, pointsSpentText, threshold2Text, threshold3Text;
    public GameObject tooltip;
    public RectTransform tooltipBackground;
    public TextMeshProUGUI tooltipDescription;
    public Button previewButton, finishPrestigeButton;
    public Vector3 tooltipOffset;
    public float tooltipSizeOffset;

    public int row2Threshold = 2;
    public int row3Threshold = 5;
    public int row2Start = 6;
    public int row3Start = 13;

    PrestigeOption tempOption;
    Vector3 tooltipWorldToScreen;

    //private void OnEnable()
    //{
    //    PopulateMenu();//debug
    //}

    public void CheckPrestigeExp()
    {
        if (GM.currencies[3] >= prestigeExpToLevel[prestigeLevel])
        {
            GainPrestigeLevel();
        }
    }

    public void GainPrestigeLevel()
    {
        prestigeLevel += 1;
        //sound effect
        //visual effect on the crystal
    }

    public void PopulateMenu()
    {
        for(int ii = 0; ii < Mathf.Min(options.Length, GM.DB.prestigeDatas.Length); ii++)
        {
            options[ii].SetData(GM.DB.prestigeDatas[ii]);
        }

        AddOption(0, true);//always have these
        AddOption(1, true);

        threshold2Text.text = row2Threshold.ToString();
        threshold3Text.text = row3Threshold.ToString();

        UpdateMenu();
    }
    
    public int GetAvailablePoints()
    {
        int maxPoints = prestigeLevel;
        //maxPoints -= activeOptions.Count;
        maxPoints -= GetSpentPoints();//don't take away points for the auto unlocks

        return maxPoints;
    }

    public int GetSpentPoints()
    {
        int spentPoints = activeOptions.Count;

        for (int ii = 0; ii < activeOptions.Count; ii++)
        {
            if (GM.DB.prestigeDatas[activeOptions[ii]].autoUnlock)//don't take away points for the auto unlocks
            {
                spentPoints -= 1;
            }
        }

        return spentPoints;
    }

    public void AddOption(int index, bool bypassPoints)
    {
        if (activeOptions.Contains(index)) { return; }
        if(GetAvailablePoints() <= 0 && bypassPoints == false) { return; }

        activeOptions.Add(index);
        UpdateMenu();
    }

    public void RemoveOption(int index)
    {
        if (activeOptions.Contains(index) == false) { return; }

        activeOptions.Remove(index);
        UpdateMenu();
    }

    public void UpdateMenu()
    {
        for (int ii = 0; ii < Mathf.Min(options.Length, GM.DB.prestigeDatas.Length); ii++)
        {
            options[ii].SetAvailable();
            options[ii].SetData(GM.DB.prestigeDatas[ii]);
        }
        for (int ii = 0; ii < activeOptions.Count; ii++)
        {
            switch(activeOptions[ii])
            {
                case 0:
                    unlockedDoubleRate = true;
                    break;

                case 1:
                    unlockedDoubleBuildRate = true;
                    break;

                case 2:
                    unlockedGroupPickup = true;
                    break;

                case 3:
                    unlockedGatheringUpgrade1 = true;
                    break;

                case 4:
                    unlockedStartingWell = true;
                    break;

                case 5:
                    unlockedStartingWorkers1 = true;
                    break;

                case 6:
                    unlockedQuadRate = true;
                    break;

                case 7:
                    unlockedQuadBuildRate = true;
                    break;

                case 8:
                    unlockedStartingResources1 = true;
                    break;

                case 9:
                    unlockedAutoAssign = true;
                    break;

                case 10:
                    unlockedGatheringUpgrade2 = true;
                    break;

                case 11:
                    unlockedUnlimitedLifespan = true;
                    break;

                case 12:
                    unlockedFasterCrafting1 = true;
                    break;

                //case 13:
                //    unlockedTenRate = true;
                //    break;

                //case 14:
                //    unlockedTenBuildRate = true;
                //    break;

                //case 15:
                //    unlockedStartingResources2 = true;
                //    break;

                //case 16:
                //    unlockedEyeHQ = true;
                //    break;

                //case 17:
                //    unlockedGatheringUpgrade3 = true;
                //    break;

                //case 18:
                //    unlockedFasterCrafting2 = true;
                //    break;

                //case 19:
                //    unlockedStartingWorkers2 = true;
                //    break;

            }

            tempOption = FindOption(activeOptions[ii]);
            if(tempOption != null)
            {
                tempOption.SetSelected();
            }
        }

        if (GetSpentPoints() < row2Threshold)
        {
            for (int ii = row2Start; ii < row3Start; ii++)
            {
                options[ii].SetUnavailable();
                if (activeOptions.Contains(options[ii].optionIndex))
                {
                    activeOptions.Remove(options[ii].optionIndex);
                }
            }
        }
        else
        {
            AddOption(6, true);
            AddOption(7, true);
            AddOption(8, true);
        }

        //if (GetSpentPoints() < row3Threshold)
        //{
        //    for (int ii = row3Start; ii < GM.DB.prestigeDatas.Length; ii++)
        //    {
        //        options[ii].SetUnavailable();
        //        if (activeOptions.Contains(options[ii].optionIndex))
        //        {
        //            activeOptions.Remove(options[ii].optionIndex);
        //        }
        //    }
        //}
        //else
        //{
        //    AddOption(13, true);
        //    AddOption(14, true);
        //    AddOption(15, true);
        //}

        for (int ii = 0; ii < options.Length; ii++)
        {
            if (options[ii].available == false)
            {
                options[ii].nameText.text = "???";
            }
        }

        availablePointsText.text = GetAvailablePoints().ToString();
        pointsSpentText.text = GetSpentPoints().ToString();
    }

    public void PopulateTooltip(PrestigeOption pass)
    {
        tooltip.gameObject.SetActive(true);
        tooltip.gameObject.transform.position = pass.transform.position + tooltipOffset;
        tooltipDescription.text = GM.DB.prestigeDatas[pass.optionIndex].description;
        tooltipDescription.ForceMeshUpdate();
        float prefHeight = tooltipDescription.preferredHeight + tooltipSizeOffset;
        tooltipBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, prefHeight);

        //might need world to screen conversion
        tooltipWorldToScreen = GM.mainCam.cam.WorldToScreenPoint(tooltip.gameObject.transform.position);
        if(tooltipWorldToScreen.y > Screen.height * 0.8f)//flip to other side
        {
            tooltip.gameObject.transform.position = pass.transform.position - tooltipOffset;
        }
        //check edges
    }


    public void DisableTooltip()
    {
        tooltip.gameObject.SetActive(false);
    }


    public PrestigeOption FindOption(int index)
    {
        for(int ii = 0; ii < options.Length; ii++) 
        {
            if (options[ii].optionIndex == index)
            {
                return options[ii];
            }
        }
        return null;
    }

    public void TogglePrestigePreview()
    {
        if (canvasGroup.gameObject.activeInHierarchy)
        {
            GM.fade.StartFadeOut(canvasGroup);
            canvasGroup.interactable = false;
        }
        else
        {
            GM.fade.StartFadeIn(canvasGroup);
            canvasGroup.interactable = true;
            GM.CloseAllBuildMenus();
            finishPrestigeButton.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class PrestigeData
{
    public string nameID;
    public int index;
    public string description;
    public Sprite sprite;
    public bool autoUnlock;
}