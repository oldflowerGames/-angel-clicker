using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class StudyMenu : MonoBehaviour
{
    public GameManager GM;
    public BuildMenu buildMenu;

    public float bonusRatePerBuilder = 1f;
    public float baseRate = 0.1f;
    public float bonusRatePerLevel = 0.01f;

    public int totalLevel = 0;
    public float[] currentExps = new float[3];
    public float[] expsToLevel = new float[3];
    public float baseExpToLevel = 16;
    public float expIncrement = 8;
    public TextMeshProUGUI totalLevelText, buildersAvailableText;

    public int[] totalLevelThreshold = new int[3];
    public int[] totalLevelUpgradeIndexes = new int[3];
    public Image totalLevelVisual, totalLevelBase;
    public Image[] totalLevelIcons = new Image[3];
    public TextMeshProUGUI[] totalLevelTexts = new TextMeshProUGUI[3];

    public StudyOption[] studyOptions = new StudyOption[3];

    public SFXPool gainLevelSound;

    public void EnableMenu()
    {
        totalLevelText.text = "Total Level: " + totalLevel.ToString();
        CheckWorkers();
        CheckLevels();
        for (int ii = 0; ii < totalLevelTexts.Length; ii++)
        {
            totalLevelTexts[ii].text = totalLevelThreshold[ii].ToString();
        }
    }

    public void ResetMenu()
    {
        CheckWorkers();
        CheckLevels();
    }

    // Update is called once per frame
    void Update()
    {
        if (buildMenu.building.constructed == false) { return; }
        for(int ii = 0; ii < studyOptions.Length; ii++)
        {
            currentExps[ii] += GM.studyRate * (baseRate + (bonusRatePerLevel * totalLevel) + (studyOptions[ii].assignedBuilders.Count * bonusRatePerBuilder)) * Time.deltaTime;//each worker contributes to the research evenly (for now)
            studyOptions[ii].expBar.value = currentExps[ii];
            studyOptions[ii].expBar.maxValue = expsToLevel[ii];
            if (currentExps[ii] > expsToLevel[ii]) 
            { 
                GainLevel(ii);
            }
        }
    }

    public void GainLevel(int index)
    {
        currentExps[index] -= expsToLevel[index];
        GM.PurchaseOption(buildMenu.buyOptions[index].optionIndex, buildMenu.buyOptions[index], true);
        expsToLevel[index] = GetExpToLevel(index);
        studyOptions[index].expBar.maxValue = expsToLevel[index];
        studyOptions[index].levelText.text = GM.upgrades[buildMenu.buyOptions[index].optionIndex].level.ToString();
        GetTotalLevel(true);
        CheckLevels();
        buildMenu.PopulateOptions();
        gainLevelSound.SelectSound(1);
        for(int ii = 0; ii < totalLevelThreshold.Length; ii++)
        {
            //if (GM.upgrades[buildMenu.buyOptions[index].optionIndex].level == totalLevelThreshold[ii])
            if(totalLevel == totalLevelThreshold[ii])
            {
                GM.sfx.PlaySound(GM.sfx.studyTotalLevel);
            }
        }
    }

    public int GetTotalLevel(bool set)
    {
        int totalLevelTemp = 0;
        for(int ii = 0; ii < buildMenu.buyOptions.Length; ii++)
        {
            if (buildMenu.buyOptions[ii].studyTotalLevelOption == false)
            {
                totalLevelTemp += GM.upgrades[buildMenu.buyOptions[ii].optionIndex].level;
            }
        }

        if (set) 
        {
            totalLevel = totalLevelTemp;
        }

        return totalLevelTemp;
    }


    public void SpendPoint()
    {
        //upgrade was already incremented with the purchase option, so this should automatically set the pointsRemaining to the right value
        CheckLevels();
    }


    public void CheckLevels()
    {
        for(int ii = 0; ii < studyOptions.Length; ii++)
        {
            expsToLevel[ii] = GetExpToLevel(ii);
            studyOptions[ii].levelText.text = GM.upgrades[buildMenu.buyOptions[ii].optionIndex].level.ToString();
        }

        int currentThreshold = 0;
        int nextThreshold = 0;
        int nextThresholdIndex = 0;
        for(int ii = 0; ii < totalLevelUpgradeIndexes.Length; ii++)
        {
            if(totalLevel >= totalLevelThreshold[ii])
            {
                GM.upgrades[totalLevelUpgradeIndexes[ii]].level = 2;
                currentThreshold = totalLevelThreshold[ii];
            }
            else
            {
                nextThreshold = totalLevelThreshold[ii];
                nextThresholdIndex = ii;
                ii = totalLevelUpgradeIndexes.Length;
            }
        }

        totalLevelText.text = "Total Level: " + totalLevel.ToString();
        foreach (Image i in totalLevelIcons) { i.color = Color.black; }
        for(int ii = 0; ii < totalLevelIcons.Length; ii++)
        {
            if (totalLevel >= totalLevelThreshold[ii])
            {
                totalLevelIcons[ii].color = Color.white;
            }
        }
        totalLevelVisual.sprite = totalLevelBase.sprite = totalLevelIcons[nextThresholdIndex].sprite;
        //totalLevelVisual.sprite = totalLevelBase.sprite = GM.upgrades[totalLevelUpgradeIndexes[nextThreshold]].sprite;
        totalLevelVisual.fillAmount = ((float)totalLevel - currentThreshold) / ((float)nextThreshold - currentThreshold);
    }

    public void CheckWorkers()
    {
        for (int ii = 0; ii < studyOptions.Length; ii++)
        {
            studyOptions[ii].CheckButtons();
        }
        buildersAvailableText.text = GetWorkersAvailable().ToString();
    }

    public void RemoveBuilder(Builder builder)
    {
        for (int ii = 0; ii < studyOptions.Length; ii++)
        {
            if (studyOptions[ii].assignedBuilders.Contains(builder))
            {
                studyOptions[ii].assignedBuilders.Remove(builder);
            }
        }
        CheckWorkers();
    }

    public int GetWorkersAvailable()
    {
        int workersAvailable = buildMenu.building.assignedBuilders.Count;

        for (int ii = 0; ii < studyOptions.Length; ii++)
        {
            workersAvailable -= studyOptions[ii].assignedBuilders.Count;
        }

        return workersAvailable;
    }

    public Builder FindAvailableWorker()
    {
        bool available = true;
        for(int ii = 0; ii < buildMenu.building.assignedBuilders.Count; ii++)
        {
            available = true;
            for(int jj = 0; jj < studyOptions.Length; jj++)
            {
                if (studyOptions[jj].assignedBuilders.Contains(buildMenu.building.assignedBuilders[ii]))
                {
                    available = false;
                }
            }
            if (available)
            {
                return buildMenu.building.assignedBuilders[ii];
            }
        }

        return null;
    }

    public float GetExpToLevel(int index)
    {
        float tempToLevel = 0;

        tempToLevel = baseExpToLevel + (GM.upgrades[buildMenu.buyOptions[index].optionIndex].level * expIncrement);

        return tempToLevel;
    }
}
