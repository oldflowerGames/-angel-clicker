using UnityEngine;
using TMPro;

public class CurrencyBuilding : MonoBehaviour
{
    public GameManager GM;
    public Building attachedBuilding;
    public enum BuildingID
    {
        Well,
        Crystal
    }

    public BuildingID buildingID;
    public float gatherInterval = 1f;
    public float gatherTimer = 0;
    public float baseGatherInterval = 1f;
    public float lowestGatherInterval = 0.1f;
    public float gatherIntervalIncrement = 0.001f;
    public TextMeshPro rateDisplay;
    bool gatherThisFrame;
    int gatherAdd;
    float lastRate, lastMultiplier, lastCurrency;

    void Update()
    {
        if (attachedBuilding == null) { if (rateDisplay != null) { rateDisplay.gameObject.SetActive(false); return; } }
        if (attachedBuilding.constructed == false) { if (rateDisplay != null) { rateDisplay.gameObject.SetActive(false); return; } }

        gatherAdd = 0;
        gatherTimer += Time.deltaTime;

        if (gatherTimer > gatherInterval)
        {
            gatherThisFrame = true;
            gatherTimer = 0;
        }
        else { gatherThisFrame = false; }

        if (gatherThisFrame)
        {
            if (buildingID == BuildingID.Well)
            {
                gatherAdd = Mathf.RoundToInt(GM.currencyGains[2] * GM.wellGatherMultiplier);
                GM.currencies[2] += gatherAdd;
                //GM.CreateGatherNumber(gameObject, gatherAdd);
            }
            if (buildingID == BuildingID.Crystal)
            {
                gatherAdd = Mathf.RoundToInt(GM.soulSpawnMultiplier);
                GM.ActivateSouls(gatherAdd);
                gatherInterval = Mathf.Max(lowestGatherInterval, gatherInterval - gatherIntervalIncrement);//very gradually the soul rate increases automatically
                //GM.currencies[3] += gatherAdd;
                //GM.CreateGatherNumber(gameObject, gatherAdd);
            }
        }

        CheckRateText();

    }

    public void CheckRateText()
    {
        if (buildingID == BuildingID.Well)
        {
            if (GM.wellGatherMultiplier != lastMultiplier || GM.currencyGains[2] != lastCurrency)//don't constantly set this, only when the value changes
            {
                SetRateText();
                lastMultiplier = GM.wellGatherMultiplier;
                lastCurrency = GM.currencies[2];
            }
        }
        if (buildingID == BuildingID.Crystal)
        {
            if (GM.soulGatherMultiplier != lastMultiplier || GM.currencyGains[3] != lastCurrency)
            {
                SetRateText();
                lastMultiplier = GM.soulGatherMultiplier;
                lastCurrency = GM.currencyGains[3];
            }
        }
    }

    public void SetRateText()
    {
        float gatherPerSecond = 0;
        if (buildingID == BuildingID.Well)
        {
            gatherPerSecond = GetWaterRate(GM.wellGatherMultiplier);
        }
        if (buildingID == BuildingID.Crystal)
        {
            gatherPerSecond = GetSoulRate(GM.soulGatherMultiplier);
        }
        if(rateDisplay != null)
        {
            int rate = Mathf.RoundToInt(gatherPerSecond * 60);
            rateDisplay.text = GM.ToKMB((decimal)rate) + "/min";
            rateDisplay.gameObject.SetActive(true);
        }
    }

    public float GetWaterRate(float multiplierPass)
    {
        float rate = ((GM.currencyGains[2] * multiplierPass) / gatherInterval);
        return rate;
    }

    public float GetSoulRate(float multiplierPass)
    {
        float rate = ((GM.currencyGains[3] * multiplierPass) / gatherInterval);
        return rate;
    }
}
