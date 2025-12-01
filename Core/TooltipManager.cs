using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;

public class TooltipManager : MonoBehaviour
{
    public GameManager GM;
    public TextMeshProUGUI tooltipText;
    public RectTransform tooltip;
    public float textboxHeightOffset = 25;
    public float tooltipScreenOffset = 0.2f;
    public Vector3 tooltipOffset = Vector3.left * 3f;
    Vector3 worldToScreen;

    public void PopulateUpgradeTooltip(BuyOption option)
    {
        tooltip.gameObject.SetActive(true);
        //set tooltip position
        tooltipText.text = GetUpgradeText(option.optionIndex);
        tooltipText.ForceMeshUpdate();
        float prefHeight = tooltipText.preferredHeight + textboxHeightOffset;
        tooltip.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, prefHeight);

        //tooltip.transform.position = GM.mainCam.cam.WorldToScreenPoint(option.gameObject.transform.position) + (Vector3.left * (Screen.width * tooltipScreenOffset));
        tooltip.transform.position = option.transform.position + tooltipOffset;
        if (option.gameObject.transform.position.x >= Screen.width * 0.5f)
        {
            tooltip.transform.localPosition += Vector3.left * option.rectTransform.sizeDelta.x * option.tooltipOffset;
        }
        else if(option.gameObject.transform.position.x <= Screen.width * 0.5f)
        {
            tooltip.transform.localPosition += Vector3.right * option.rectTransform.sizeDelta.x * option.tooltipOffset;
        }
    }

    public string GetUpgradeText(int index)
    {
        string finalString = GM.upgrades[index].description;
        switch (index)
        {
            case 1:
                if (GM.upgrades[6].level > 1 || GM.prestige.prestigeLevel > 0)//wisps are included as well
                {
                    finalString = GM.upgrades[index].altDescription;
                }
                finalString = Regex.Replace(finalString, "&1", GM.gathererMultiplier.ToString());
                finalString = Regex.Replace(finalString, "&2", GM.GetGathererMultiplier(GM.upgrades[index].level + 1).ToString());
                break;

            case 2:
                finalString = Regex.Replace(finalString, "&1", GM.manualGatherMultiplier.ToString());
                finalString = Regex.Replace(finalString, "&2", GM.GetManualGatherMultiplier(GM.upgrades[index].level + 1).ToString());
                break;

            case 7:
                finalString = Regex.Replace(finalString, "&1", (GM.wispSpeed * 100).ToString());
                finalString = Regex.Replace(finalString, "&2", (GM.GetWispSpeed(GM.upgrades[index].level + 1) * 100).ToString());
                break;

            case 8:
                finalString = Regex.Replace(finalString, "&1", GM.builderLifetime.ToString());
                finalString = Regex.Replace(finalString, "&2", GM.GetBuilderLifetime(GM.upgrades[index].level + 1).ToString());
                break;

            case 9:
                finalString = Regex.Replace(finalString, "&1", (GM.constructionRate * 100).ToString());
                finalString = Regex.Replace(finalString, "&2", (GM.GetConstructionRate(GM.upgrades[index].level + 1, GM.upgrades[25].level) * 100).ToString());
                break;

            case 10:
            //finalString = Regex.Replace(finalString, "&1", GM.ToKMB((decimal)GM.currencyBuildings[0].GetWaterRate(GM.wellGatherMultiplier) * 60));
            //finalString = Regex.Replace(finalString, "&2", GM.ToKMB((decimal)(GM.currencyBuildings[0].GetWaterRate(GM.GetWellGatherMult(GM.upgrades[index].level + 1, GM.upgrades[20].level)) * 60)));

                finalString = Regex.Replace(finalString, "&1", GM.wellGatherMultiplier.ToString());
                finalString = Regex.Replace(finalString, "&2", GM.GetWellGatherMult(GM.upgrades[10].level + 1, GM.upgrades[20].level).ToString());
                break;

            case 13:
                finalString = Regex.Replace(finalString, "&1", (GM.baseCurrencyGains[0] + GM.flatCurrencyGatherBonuses[0]).ToString());
                finalString = Regex.Replace(finalString, "&2", (GM.baseCurrencyGains[0] + GM.GetFlatGatherBonus(GM.currencies[5] + 1, GM.currencies[8], GM.upgrades[24].level)).ToString());
                break;

            case 14:
                finalString = Regex.Replace(finalString, "&1", (GM.baseCurrencyGains[1] + GM.flatCurrencyGatherBonuses[1]).ToString());
                finalString = Regex.Replace(finalString, "&2", (GM.baseCurrencyGains[1] + GM.GetFlatGatherBonus(GM.currencies[6] + 1, GM.currencies[9], GM.upgrades[24].level, GM.upgrades[33].level)).ToString());
                break;

            case 15:
                finalString = Regex.Replace(finalString, "&1", (GM.baseCurrencyGains[2] + GM.flatCurrencyGatherBonuses[2]).ToString());
                finalString = Regex.Replace(finalString, "&2", (GM.baseCurrencyGains[2] + GM.GetFlatGatherBonus(GM.currencies[7] + 1, GM.currencies[10], GM.upgrades[25].level)).ToString());
                break;

            case 16:
                finalString = Regex.Replace(finalString, "&1", (GM.baseCurrencyGains[0] + GM.flatCurrencyGatherBonuses[0]).ToString());
                finalString = Regex.Replace(finalString, "&2", (GM.baseCurrencyGains[0] + GM.GetFlatGatherBonus(GM.currencies[5], GM.currencies[8] + 1, GM.upgrades[24].level)).ToString());
                break;

            case 17:
                finalString = Regex.Replace(finalString, "&1", (GM.baseCurrencyGains[1] + GM.flatCurrencyGatherBonuses[1]).ToString());
                finalString = Regex.Replace(finalString, "&2", (GM.baseCurrencyGains[1] + GM.GetFlatGatherBonus(GM.currencies[6], GM.currencies[9] + 1, GM.upgrades[24].level, GM.upgrades[33].level)).ToString());
                break;

            case 18:
                finalString = Regex.Replace(finalString, "&1", (GM.baseCurrencyGains[2] + GM.flatCurrencyGatherBonuses[2]).ToString());
                finalString = Regex.Replace(finalString, "&2", (GM.baseCurrencyGains[2] + GM.GetFlatGatherBonus(GM.currencies[7], GM.currencies[10] + 1, GM.upgrades[25].level)).ToString());
                break;

            case 19:
                finalString = Regex.Replace(finalString, "&1", Global.lightCrystalGatheringBonus.ToString());
                break;

            case 20:
                finalString = Regex.Replace(finalString, "&1", GM.wellGatherMultiplier.ToString());
                finalString = Regex.Replace(finalString, "&2", GM.GetWellGatherMult(GM.upgrades[10].level, GM.upgrades[index].level + 1).ToString());
                break;

            case 21:
                finalString = Regex.Replace(finalString, "&1", GM.craftingRate.ToString());
                finalString = Regex.Replace(finalString, "&2", GM.GetCraftingRate(GM.upgrades[index].level + 1, GM.upgrades[28].level).ToString());
                break;

            case 22:
                finalString = Regex.Replace(finalString, "&1", GM.martMenu.burstInterval.ToString());
                finalString = Regex.Replace(finalString, "&2", GM.martMenu.GetBurstInterval(GM.upgrades[index].level + 1).ToString());
                break;

            case 23:
                finalString = Regex.Replace(finalString, "&1", GM.GetSoulGather(GM.upgrades[index].level, GM.upgrades[29].level).ToString());
                finalString = Regex.Replace(finalString, "&2", GM.GetSoulGather(GM.upgrades[index].level + 1, GM.upgrades[29].level).ToString());
                break;

            case 24:
                finalString = Regex.Replace(finalString, "&1", (GM.baseCurrencyGains[0] + GM.flatCurrencyGatherBonuses[0]).ToString());
                finalString = Regex.Replace(finalString, "&2", (GM.baseCurrencyGains[0] + GM.GetFlatGatherBonus(GM.currencies[5], GM.currencies[8], GM.upgrades[24].level + 1)).ToString());
                finalString = Regex.Replace(finalString, "&3", (GM.baseCurrencyGains[1] + GM.flatCurrencyGatherBonuses[1]).ToString());
                finalString = Regex.Replace(finalString, "&4", (GM.baseCurrencyGains[1] + GM.GetFlatGatherBonus(GM.currencies[6], GM.currencies[9], GM.upgrades[24].level + 1)).ToString());
                break;

            case 25:
                finalString = Regex.Replace(finalString, "&1", GM.constructionRate.ToString());
                finalString = Regex.Replace(finalString, "&2", GM.GetConstructionRate(GM.upgrades[9].level, GM.upgrades[25].level + 1).ToString());
                break;

            case 26:
                finalString = Regex.Replace(finalString, "&1", GM.martRate.ToString());
                finalString = Regex.Replace(finalString, "&2", GM.GetMartRate(GM.upgrades[index].level + 1).ToString());
                break;

            case 33:
                finalString = Regex.Replace(finalString, "&1", (GM.baseCurrencyGains[1] + GM.flatCurrencyGatherBonuses[1]).ToString());
                finalString = Regex.Replace(finalString, "&2", (GM.baseCurrencyGains[1] + GM.GetFlatGatherBonus(GM.currencies[6], GM.currencies[9], GM.upgrades[24].level, GM.upgrades[33].level + 1)).ToString());
                break;

            case 34:
                finalString = Regex.Replace(finalString, "&1", GM.groupPickupMax.ToString());
                finalString = Regex.Replace(finalString, "&2", GM.GetGroupPickupMax(GM.upgrades[index].level + 1).ToString());
                break;
        }
        return finalString;
    }
}
