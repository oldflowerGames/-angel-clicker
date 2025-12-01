using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MartMenu : MonoBehaviour
{
    public GameManager GM;
    public BuildMenu buildMenu;

    public float bonusRatePerBuilder = 0.1f;

    public int level;
    public int pointsRemaining;
    public float currentExp;
    public float expToLevel;
    public float baseExpToLevel = 15;
    public float expIncrement = 5;

    public Slider expBar;
    public TextMeshProUGUI pointsRemainingText, pointsRemainingTopText, totalTevelText, nextSkillText, nameText;

    public GameObject[] animalLayers = new GameObject[4];
    public string[] animalNames = new string[4];
    public int[] layerThresholds = new int[4];
    public int activeLayer = 0;

    public GameObject burstObj;
    public float burstInterval = 10;
    public float burstTimer = 0;
    public float baseBurstInterval = 10;
    public float burstIntervalIncrement = 0.5f;

    // Update is called once per frame
    void Update()
    {
        currentExp += (GM.martRate * (1 + (buildMenu.building.assignedBuilders.Count * bonusRatePerBuilder))) * Time.deltaTime;//+10% rate per assigned builder

        if(currentExp > expToLevel)
        {
            GainLevel();
        }

        expBar.value = currentExp;
        expBar.maxValue = expToLevel;

        burstTimer += Time.deltaTime;
        if(burstTimer > burstInterval)
        {
            ActivateBurst();
            burstTimer = 0;
        }
    }

    public void GainLevel()
    {
        level += 1;
        currentExp -= expToLevel;
        expToLevel = GetExpToLevel();
        CheckLevels();
        buildMenu.PopulateOptions();
        SetAnimal();
    }

    public void ActivateBurst()
    {
        if (GM.upgrades[22].level <= 1 || GM.constructedLight1 == false) { return; }

        GM.ActivateSouls(250);
        burstObj.SetActive(true);
    }

    public float GetBurstInterval(int levelPass)
    {
        float returnInterval = 0;
        returnInterval = baseBurstInterval - (burstIntervalIncrement * levelPass);
        return returnInterval;
    }

    public void SetAnimal()
    {
        int layerCheck = 0;
        for(int ii = 0; ii < layerThresholds.Length; ii++)
        {
            if(level >= layerThresholds[ii])
            {
                layerCheck = ii;
            }
        }
        if(layerCheck != activeLayer || activeLayer == 0)
        {
            activeLayer = layerCheck;
            for (int ii = 0; ii < animalLayers.Length; ii++)
            {
                if (ii == activeLayer)
                {
                    animalLayers[ii].SetActive(true);
                }
                else
                {
                    animalLayers[ii].SetActive(false);
                }
            }
            nameText.text = animalNames[layerCheck];
        }
    }

    public void SpendPoint()
    {
        //upgrade was already incremented with the purchase option, so this should automatically set the pointsRemaining to the right value
        CheckLevels();
    }


    public void CheckLevels()
    {
        totalTevelText.text = "Level: " + level.ToString();
        pointsRemainingText.text = pointsRemainingTopText.text = GetPointsAvailable(true).ToString();
    }

    public int GetPointsAvailable(bool set)
    {
        int pointsAvailable = level;
        for(int ii = 20; ii < 23; ii++)
        {
            pointsAvailable -= (GM.upgrades[ii].level - 1);
        }
        for (int ii = 0; ii < GM.upgrades[23].level; ii++)
        {
            pointsAvailable -= ii;
        }
        if (set) 
        {
            pointsRemaining = pointsAvailable;
            GM.currencies[11] = pointsAvailable;
        }
        return pointsAvailable;
    }

    public float GetExpToLevel()
    {
        float tempToLevel = 0;

        tempToLevel = baseExpToLevel + (level * expIncrement);

        return tempToLevel;
    }
}
