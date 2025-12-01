using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeData
{
    public string upgradeName;
    public int upgradeIndex;
    public int baseCost
    {
        get { return costs[0].cost; }
        set { costs[0].cost = value; }
    }
    public int currencyIndex
    {
        get { return costs[0].currencyIndex; }
        set { costs[0].currencyIndex = value; }
    }
    public int level = 1;
    public int startLevel = 1;
    public int maxLevel = 10;
    public enum CostType
    {
        Additive,
        Exponential,
        Doubling,
        Static
    }
    public CostType costType;
    public Sprite sprite;

    public CostData[] costs = new CostData[3];

    public List<int> prerequisites = new List<int>();
    public int prestigeRequirement = 0;
    public bool ignoreReset = false;
    public bool required = false;

    public string description;
    public string altDescription;
}
