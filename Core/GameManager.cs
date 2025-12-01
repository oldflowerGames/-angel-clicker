using UnityEngine;
using System;
using DamageNumbersPro;
using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public bool pauseInput = false;

    public long[] currencies = new long[5];
    public long[] totalCurrencies = new long[5];
    public long[] currencyGains = new long[5];
    public float[] currencyGainMultipliers = new float[5];

    public PrestigeManager prestige;
    public MusicOverseer music;

    public Sprite[] currencyIcons = new Sprite[13];
    public Sprite[] craftingIcons = new Sprite[13];

    public Builder[] builders = new Builder[100];
    public Builder[] wisps = new Builder[10];
    public Soul[] souls = new Soul[1000];

    public Building[] buildings = new Building[5];
    public Building[] collectibleBuildings = new Building[2];
    public CurrencyBuilding[] currencyBuildings  = new CurrencyBuilding[2];
    public Building selectedBuilding;
    public MartMenu martMenu;
    public StudyMenu studyMenu;
    public CanvasGroup mainUI, tutorialWindow, metronomeWindow;

    public bool constructedLight1 = false;
    public bool constructedLight2 = false;

    public CraftingStation[] craftingStations = new CraftingStation[2];

    public float gameSpeed = 1f;

    public UpgradeData[] upgrades = new UpgradeData[50];

    public PlayerInput playerInput;

    public AngelDatabase DB;
    public SFXManager sfx;
    public CameraController mainCam;
    public GameObject bonusCurrencyDisplay;
    public MetronomeClick metronomeDisplay;
    public FadeUtils fade;
    public BuildingBar buildingBar;
    public TooltipManager tooltip;
    public SettingsSave settingsSave;
    public DialogueOverseer dialogue;
    public IncrementNumber hellScore;
    public GameObject scorecard;
    public bool onMobile = false;
    public bool helpMode = false;

    public float builderSpeed = 1;
    public float builderLifetime = 10;
    public float builderLifetimeRegen = 2f;

    public float gathererSpeed = 1;
    public float gathererMultiplier = 1;

    public float manualGatherMultiplier = 1f;

    public float wispSpeed = 1f;
    public float wellGatherMultiplier = 1f;

    public int[] baseCurrencyGains = new int[5];
    public long[] flatCurrencyGatherBonuses = new long[5];

    public float craftingRate = 1f;

    public float soulGatherMultiplier = 1f;
    public float soulSpawnMultiplier = 1f;

    public float martRate = 1f;

    public float studyRate = 1f;

    public int groupPickupMax = 1;

    public float builderSpeedBase = 1;
    public float builderSpeedIncrement = 0.1f;
    public float builderLifetimeBase = 10;
    public float builderLifetimeIncrement = 2f;
    public float builderLifetimeRegenBase = 2f;

    public float gathererSpeedBase = 1;
    public float gathererSpeedIncrement = 0.1f;

    public float gathererMultiplierBase = 1;
    public float gathererMultiplierIncrement = 0.5f;

    public float manualGatherMultiplierBase = 1;
    public float manualGatherMultiplierIncrement = 0.75f;

    public float wispSpeedBase = 1f;
    public float wispSpeedIncrement = 0.2f;

    public int wellGatherBase = 1;
    public int wellGatherIncrement = 1;
    public int wellGatherIncrement2 = 3;

    public float wellGatherMultiplierBase = 1f;
    public float wellGatherMultiplierIncrement = 0.5f;
    public float wellGatherMultiplierIncrement2 = 1.5f;

    public float constructionRate = 1f;
    public float constructionRateBase = 1f;
    public float constructionRateIncrement = 0.5f;

    public float craftingRateBase = 1f;
    public float craftingRateIncrement = 0.5f;

    public long soulGatherBase = 1;
    public long soulGatherIncrement = 1;

    public float soulGatherMultiplierBase = 1f;
    public float soulGatherMultiplierIncrement = 1f;

    public float soulSpawnMultiplierBase = 1f;
    public float soulSpawnMultiplierIncrement = 0.25f;

    public float martRateBase = 1f;
    public float martRateIncrement = 0.1f;

    public float studyRateBase = 1f;
    public float studyRateIncrement = 0.1f;

    public int groupPickupMaxBase = 0;
    public int groupPickupIncrement = 1;

    public Vector3 builderSpawnPos = new Vector3(0, -4.7f, -1f);

    public DamageNumber gatherNumberPrefab;

    public GameObject victoryFlash, wave;
    public CanvasGroup victoryUI, exitGameFade;

    public SpriteRenderer nightsky;
    public List<SpriteRenderer> clouds = new List<SpriteRenderer>();
    public IdleFloater angel;
    public SettingsMenu settingsMenu;

    Soul tempSoul;

    public void Start()
    {
        dialogue.canvasGroup.gameObject.SetActive(false);
        hellScore.gameObject.SetActive(false);
        scorecard.SetActive(false);
        InitialSetup();
        settingsMenu.VolumeInit();
    }

    public void InitialSetup()
    {
        int startingBuilderCount = 3;
        if (prestige.unlockedStartingWorkers1) { startingBuilderCount = Global.unlockStartingWorkers; }
        for (int ii = 0; ii < startingBuilderCount; ii++)
        {
            SpawnBuilder(false);
        }

        InitialBuildingSetup();
        for (int ii = 0; ii < currencies.Length; ii++)
        {
            if (ii != 3)
            {
                currencies[ii] = 0;
            }
        }
        constructedLight1 = false;
        bonusCurrencyDisplay.SetActive(false);
        metronomeDisplay.gameObject.SetActive(false);
        mainCam.gameObject.transform.position = new Vector3(0, 0f, -10);
        mainCam.zoomLevel = 2;
        mainCam.SetZoom();

        if (prestige.unlockedStartingResources1)
        {
            for(int ii = 0; ii < 3; ii ++)
            {
                AddCurrencyAmount(ii, 10000);
            }
        }

        if(prestige.prestigeCount > 0)
        {
            buildings[6].transform.parent.gameObject.SetActive(true);
            buildings[6].SetNotification(true);
            prestige.previewButton.gameObject.SetActive(true);
        }
        else
        {
            prestige.previewButton.gameObject.SetActive(false);
        }

        if (prestige.unlockedGroupPickup)
        {
            for (int ii = 0; ii < Global.groupPickupPrestigeBonus; ii++)
            {
                PurchaseOption(34, buildings[1].buildMenu.buyOptions[2], true);
            }
        }

        studyMenu.ResetMenu();

        SetValues();

        buildingBar.PopulateBar();
        victoryUI.gameObject.SetActive(false);
        exitGameFade.gameObject.SetActive(false);

        angel.gameObject.transform.position = angel.teleportLocations[0].transform.position;
        buildings[0].SetBoxCollider(0);
    }

    public void ActivatePrestige()
    {
        prestige.canvasGroup.gameObject.SetActive(true);
        fade.StartFadeIn(prestige.canvasGroup, 1f, 0, 2f);
        prestige.canvasGroup.interactable = true;
        prestige.finishPrestigeButton.gameObject.SetActive(true);
        prestige.PopulateMenu();
        prestige.prestigeCount += 1;
        prestige.canSelect = true;
        prestige.previewButton.gameObject.SetActive(false);
        StartCoroutine(PrestigeResetDelay());
        sfx.PlaySound(sfx.activatePrestige);
        sfx.PlaySound(sfx.giantWave);
        wave.gameObject.SetActive(true);
        CloseAllBuildMenus();
        fade.StartFadeOut(mainUI);
        //music.FadeOut(0.2f);
    }

    public IEnumerator PrestigeResetDelay()
    {
        yield return new WaitForSeconds(3.5f);
        PrestigeResetExecute();
    }

    public void PrestigeResetExecute()
    {
        foreach(Builder b in builders) { b.gameObject.SetActive(false); b.assignedBuilding = null; }
        foreach(Builder b in wisps) { b.gameObject.SetActive(false); }
        foreach(Soul s in souls) { s.gameObject.SetActive(false); };
        foreach(CraftingStation c in craftingStations) { c.ResetStation(); }
        for (int ii = 0; ii < upgrades.Length; ii++)
        {
            if (upgrades[ii].ignoreReset == false)
            {
                upgrades[ii].level = upgrades[ii].startLevel;
            }
        }
        InitialBuildingSetup();
        for (int ii = 0; ii < collectibleBuildings.Length; ii++)
        {
            collectibleBuildings[ii].assignedBuilders.Clear();
            collectibleBuildings[ii].ResetCollectibles();
        }
        for(int ii = 0; ii < currencyBuildings.Length; ii++)
        {
            currencyBuildings[ii].gatherInterval = currencyBuildings[ii].baseGatherInterval;
        }
        constructedLight1 = constructedLight2 = false;
        mainCam.gameObject.transform.position = new Vector3(0, 0f, -10);
        mainCam.zoomLevel = 2;
        mainCam.SetZoom();
        wave.gameObject.SetActive(false);
        hellScore.ResetValues();
        CloseAllBuildMenus();
    }

    public void FinishPrestige()
    {
        InitialSetup();
        fade.StartFadeOut(prestige.canvasGroup);
        prestige.canSelect = false;
        prestige.canvasGroup.interactable = false;
        sfx.PlaySound(sfx.endPrestige);
        wave.gameObject.SetActive(false);
        fade.StartFadeIn(mainUI);
    }


    public void InitialBuildingSetup()
    {
        for (int ii = 0; ii < buildings.Length; ii++)
        {
            buildings[ii].transform.parent.gameObject.SetActive(false);
            buildings[ii].ResetBuilding();
        }
        buildings[0].transform.parent.gameObject.SetActive(true);
        buildings[3].gameObject.SetActive(false);//tower upgrades are a special case
        buildings[5].gameObject.SetActive(false);//crystal is a special case
        buildings[8].gameObject.SetActive(false);
        buildings[9].gameObject.SetActive(false);

        buildings[0].SetNotification(true);
        buildings[0].spriteRenderer.sprite = buildings[0].upgradeSprites[0];
        buildings[0].SetBoxCollider(0);

        if (prestige.unlockedStartingWell)
        {
            buildings[2].transform.parent.gameObject.SetActive(true);
            buildings[2].gameObject.SetActive(true);
            buildings[2].FinishBuilding();
            for (int ii = 0; ii < Global.wellUnlockStartingWisps; ii++)
            {
                PurchaseOption(6, buildings[2].buildMenu.buyOptions[0], true);
            }
            upgrades[5].level = upgrades[5].maxLevel;//prevents it from showing up on the menu
        }
    }

    public IEnumerator SetActiveDelay(GameObject objToEnable, float duration, bool enabling) 
    {
        yield return new WaitForSeconds(duration);
        objToEnable.SetActive(enabling);

    }

    public bool PurchaseOption(int index, BuyOption chosen, bool overridePurchase)
    {
        if (currencies[chosen.currencyIndex] < chosen.cost && overridePurchase == false) { return false; }

        //if (chosen.craftable == false) 
        //{
            upgrades[index].level = Mathf.Min(upgrades[index].level + 1, upgrades[index].maxLevel);
        //}

        if(overridePurchase == false)
        {
            for (int ii = 0; ii < chosen.costs.Length; ii++)
            {
                currencies[chosen.costs[ii].currencyIndex] -= chosen.costs[ii].cost;
            }
        }

        if(selectedBuilding != null)
        {
            selectedBuilding.buildMenu.Populate();
        }

        bool startConstruction = false;

        switch (index)//special cases for upgrades that aren't just numerical
        {
            case 0://spawn builder
                SpawnBuilder(true);

                break;

            case 3://build dorms
                buildings[1].StartConstruction();
                startConstruction = true;
                break;

            case 4://upgrade dorms
                buildings[1].upgradeLevel = upgrades[index].level - 1;
                buildings[1].spriteRenderer.sprite = buildings[1].upgradeSprites[Mathf.Min(buildings[1].upgradeLevel, buildings[1].upgradeSprites.Length - 1)];
                break;

            case 5://build well
                buildings[2].StartConstruction();
                startConstruction = true;
                break;

            case 6://summon wisp
                SpawnWisp();
                break;

            case 8:
                foreach(Builder b in builders)
                {
                    b.remainingLifetime += builderLifetimeIncrement;
                }
                break;

            case 11:
                buildings[3].StartConstruction();
                startConstruction = true;
                break;

            case 12:
                buildings[4].StartConstruction();
                startConstruction = true;
                break;

            case 30:
                buildings[8].StartConstruction();
                startConstruction = true;
                break;

            case 31:
                buildings[7].StartConstruction();
                startConstruction = true;
                break;
        }

        if (chosen.craftable)
        {
            StartCraftable(chosen.craftableIndex, 0);
        }
        else
        {
            CheckNotifications(index);
        }

        if (chosen.martOption)
        {
            martMenu.SpendPoint();
        }

        if (startConstruction)
        {
            CloseAllBuildMenus();
            tooltip.tooltip.gameObject.SetActive(false);
            //sound effect
        }

        SetValues();

        return true;
    }

    public void PurchaseMultiOption(int index, BuyOption chosen)
    {
        int multiCount = 10;
        int purchasedCount = 10;
        for (int ii = 0; ii < multiCount; ii++)
        {
            if (currencies[chosen.currencyIndex] > chosen.cost && upgrades[index].level < upgrades[index].maxLevel)//purchase up to 10, if you run out of currency or hit the max then cancel
            {
                PurchaseOption(index, chosen, false);
            }
            else
            {
                purchasedCount = ii;
                ii = multiCount;
            }
        }
        for(int ii = 0; ii < purchasedCount; ii++)
        {
            sfx.PlaySoundScheduled(sfx.upgrade, 1f, Global.frameTime * 4 * ii);
        }
    }

    public void CheckNotifications(int index)
    {
        int tempIndex;
        if (upgrades[index].level == 2)//first time an upgrade is purchased, check for new upgrades
        {
            for (int ii = 0; ii < upgrades.Length; ii++)
            {
                if (upgrades[ii].prerequisites.Count > 0)
                {
                    if (upgrades[ii].prerequisites.Contains(index))//if this upgrade requires the upgrade that was just obtained
                    {
                        bool newUnlock = true;
                        for(int jj = 0; jj < upgrades[ii].prerequisites.Count; jj++)//check all other prerequisites to make sure it was actually unlocked
                        {
                            tempIndex = upgrades[ii].prerequisites[jj];
                            if (upgrades[tempIndex].level < 2)
                            {
                                newUnlock = false;
                            }
                        }
                        if (newUnlock)//if all other prerequisites are met
                        {
                            for(int jj = 0; jj < buildings.Length; jj++)//find the building that just received a new option to activate its notification
                            {
                                if (buildings[jj].buildMenu != null)
                                {
                                    if (buildings[jj].buildMenu.gameObject.activeInHierarchy == false)
                                    {
                                        for (int kk = 0; kk < buildings[jj].buildMenu.buyOptions.Length; kk++)
                                        {
                                            if (buildings[jj].buildMenu.buyOptions[kk].optionIndex == upgrades[ii].upgradeIndex)
                                            {
                                                buildings[jj].SetNotification(true);
                                            }//lol nesting
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }


    public void BuildCrystal(int index)
    {
        switch (index)
        {
            case 0:
                buildings[5].gameObject.SetActive(true);
                CloseAllBuildMenus();
                ActivateSouls(250);
                //StartCoroutine(SetActiveDelay(buildings[5].notification, 12f, true));
                upgrades[19].level = 99;
                if(prestige.prestigeCount > 0)
                {
                    buildings[0].SetNotification(true);
                }
                sfx.PlaySound(sfx.craftedCrystal);
                constructedLight1 = true;
                SetValues();
                if (prestige.prestigeCount <= 0)
                {
                    dialogue.QueueDialogueDelay(1, 12f);
                }
                else
                {
                    dialogue.QueueDialogueDelay(2, 3f);
                }

                //light crystal camera sequence
                break;
            case 1:
                upgrades[32].level = 99;
                if (upgrades[30].level == 99)//if tower 2 has also been constructed, you win
                {
                    buildings[9].transform.parent.gameObject.SetActive(true);
                    PlayVictorySequence();
                    constructedLight2 = true;
                }
                sfx.PlaySound(sfx.craftedBigCrystal);
                break;
        }
    }

    public void PlayVictorySequence()
    {
        StartCoroutine(VictorySequence());
    }

    public IEnumerator VictorySequence()
    {
        bool activatedLightFlash = false;
        yield return new WaitForSeconds(0.1f);
        for(int ii = 0; ii < 100; ii++)
        {
            ActivateSouls(50);//this will max out the pool pretty quickly but that's fine
            yield return new WaitForSeconds(0.1f);
            if(ii > 75 && activatedLightFlash == false)
            {
                victoryFlash.gameObject.SetActive(true);
                sfx.PlaySound(sfx.endingWoosh);
                activatedLightFlash = true;
            }
        }
        victoryUI.gameObject.SetActive(true);
    }

    public void ContinueAfterVictory()
    {
        fade.StartFadeOut(victoryUI);
        victoryFlash.gameObject.SetActive(false);
    }

    public void ExitGame()
    {
        StartCoroutine(ExitGameExecute());
    }

    public IEnumerator ExitGameExecute()
    {
        fade.StartFadeIn(exitGameFade);
        yield return new WaitForSeconds(1.5f);
        Application.Quit();
    }

    public void SpawnBuilder(bool playSound)
    {
        Builder toActivate = FindInactiveBuilder();
        if (toActivate != null)
        {
            toActivate.gameObject.SetActive(true);
            toActivate.SetState(Builder.State.Idle);
            toActivate.gameObject.transform.position = builderSpawnPos + new Vector3(UnityEngine.Random.Range(-2, 2f), 0, UnityEngine.Random.Range(-0.01f, 0f));//slight Z random for sort order
            if (playSound)
            {
                toActivate.audioSource.clip = sfx.GetClip(sfx.builderHello);
                toActivate.PlaySound(0.95f, 1.05f, 0.2f);
            }
        }
    }

    public void SpawnWisp()
    {
        Builder toActivate = FindInactiveWisp();
        if (toActivate != null)
        {
            toActivate.gameObject.SetActive(true);
            toActivate.SetState(Builder.State.GatherWood);
            //toActivate.gameObject.transform.position = builderSpawnPos + new Vector3(UnityEngine.Random.Range(-2, 2f), 0, UnityEngine.Random.Range(-0.01f, 0.01f));//slight Z random for sort order
        }
    }

    public int AddCurrency(int index, bool manualGather)
    {
        int toAdd = 0;

        toAdd = Mathf.RoundToInt((float)currencyGains[index] * currencyGainMultipliers[index]);

        if (manualGather)
        {
            toAdd = Mathf.RoundToInt((float)currencyGains[index] * manualGatherMultiplier);
            if (prestige.unlockedGatheringUpgrade1)
            {
                toAdd += Global.gatherUpgrade1;
            }
            if (prestige.unlockedGatheringUpgrade2)
            {
                toAdd *= Global.gatherUpgrade2;
            }
            //if (prestige.unlockedGatheringUpgrade3)
            //{
            //    toAdd *= Global.gatherUpgrade3;
            //}
        }

        currencies[index] += toAdd;

        if(index == 3)
        {
            prestige.CheckPrestigeExp();
        }

        return toAdd;
    }

    public void AddCurrencyAmount(int index, int amount)
    {
        currencies[index] += amount;
    }

    public void StartCraftable(int index, int stationIndex)
    {
        craftingStations[stationIndex].QueueCraftable(index);
    }

    public void AddCraftable(int index)
    { 
        currencies[index] += 1;
        SetValues();
    }


    public void SetValues()
    {
        gathererMultiplier = GetGathererMultiplier(upgrades[1].level);

        manualGatherMultiplier = GetManualGatherMultiplier(upgrades[2].level);

        //gathererSpeed = gathererSpeedBase;
        //gathererSpeed += upgrades[2].level * gathererSpeedIncrement;

        wispSpeed = GetWispSpeed(upgrades[7].level);

        builderLifetime = GetBuilderLifetime(upgrades[8].level);
        //builderSpeed = GetBuilderSpeed(upgrades[9].level);//not in use at this time
        builderSpeed = 1;

        foreach (Builder b in builders)
        {
            b.lifetime = builderLifetime;
        }

        builderLifetimeRegen = builderLifetimeRegenBase;

        //flatCurrencyGatherBonuses[2] = GetWellGather(upgrades[18].level);

        wellGatherMultiplier = GetWellGatherMult(upgrades[10].level, upgrades[20].level);

        constructionRate = GetConstructionRate(upgrades[9].level, upgrades[25].level);

        craftingRate = GetCraftingRate(upgrades[21].level, upgrades[28].level);

        flatCurrencyGatherBonuses[3] = GetSoulGather(upgrades[23].level, upgrades[29].level);

        //soulGatherMultiplier = GetSoulGatherRate(upgrades[23].level);
        soulGatherMultiplier = 1;
        soulSpawnMultiplier = GetSoulSpawnRate(1);//nothing increases this at the moment, but will in the future

        martRate = GetMartRate(upgrades[26].level);

        martMenu.burstInterval = martMenu.GetBurstInterval(upgrades[22].level);

        groupPickupMax = GetGroupPickupMax(upgrades[34].level);

        flatCurrencyGatherBonuses[0] = GetFlatGatherBonus(currencies[5], currencies[8], upgrades[24].level);

        flatCurrencyGatherBonuses[1] = GetFlatGatherBonus(currencies[6], currencies[9], upgrades[24].level, upgrades[33].level);

        flatCurrencyGatherBonuses[2] = GetFlatGatherBonus(currencies[7], currencies[10], 0);

        if (constructedLight2)
        {
            flatCurrencyGatherBonuses[3] += Global.lightCrystalSoulBonus;
        }

        for(int ii = 0; ii < currencyGains.Length; ii++)
        {
            currencyGains[ii] = baseCurrencyGains[ii] + flatCurrencyGatherBonuses[ii];
        }
    }

    public float GetBuilderSpeed(int levelPass)
    {
        float returnSpeed = 0;
        returnSpeed = builderSpeedBase;
        returnSpeed += levelPass * builderSpeedIncrement;
        if (prestige.unlockedDoubleBuildRate)
        {
            returnSpeed *= 2;
        }
        return returnSpeed;
    }

    public float GetBuilderLifetime(int levelPass)
    {
        float returnLifetime = 0;
        returnLifetime = builderLifetimeBase;
        returnLifetime += levelPass * builderLifetimeIncrement;
        return returnLifetime;
    }

    public float GetGathererMultiplier(int levelPass)
    {
        float returnMult = 0;
        returnMult = gathererMultiplierBase;
        returnMult += levelPass * gathererMultiplierIncrement;
        if (prestige.unlockedTenRate)
        {
            returnMult *= 10;
        }
        else if (prestige.unlockedQuadRate)
        {
            returnMult *= 4;
        }
        else if (prestige.unlockedDoubleRate)
        {
            returnMult *= 2;
        }
        return returnMult;
    }

    public float GetManualGatherMultiplier(int levelPass)
    {
        float returnMult = 0;
        returnMult = manualGatherMultiplierBase;
        returnMult += levelPass * manualGatherMultiplierIncrement;
        if (prestige.unlockedTenRate)
        {
            returnMult *= 10;
        }
        else if (prestige.unlockedQuadRate)
        {
            returnMult *= 4;
        }
        else if (prestige.unlockedDoubleRate)
        {
            returnMult *= 2;
        }
        return returnMult;
    }

    public float GetWispSpeed(int levelPass)
    {
        float returnSpeed = 0;
        returnSpeed = wispSpeedBase + (wispSpeedIncrement * levelPass);
        return returnSpeed;
    }

    public int GetWellGather(int levelPass)
    {
        int returnGather = 0;
        returnGather = wellGatherBase + (wellGatherIncrement * levelPass);
        return returnGather;
    }

    public float GetWellGatherMult(int levelPass, int levelPass2)
    {
        float returnGather = 0;
        returnGather = wellGatherMultiplierBase + (wellGatherMultiplierIncrement * levelPass) + (wellGatherMultiplierIncrement2 * levelPass2);
        if (prestige.unlockedTenRate)
        {
            returnGather *= 10;
        }
        else if (prestige.unlockedQuadRate)
        {
            returnGather *= 4;
        }
        else if (prestige.unlockedDoubleRate)
        {
            returnGather *= 2;
        }
        return returnGather;
    }

    public float GetConstructionRate(int levelPass, int levelPass2)
    {
        float returnRate = 0;
        returnRate = constructionRateBase;
        returnRate += constructionRateIncrement * (levelPass + levelPass2);
        if (prestige.unlockedTenBuildRate)
        {
            returnRate *= 10;
        }
        else if (prestige.unlockedQuadBuildRate)
        {
            returnRate *= 4;
        }
        else if (prestige.unlockedDoubleBuildRate)
        {
            returnRate *= 2;
        }

        return returnRate;
    }

    public float GetCraftingRate(int levelPass, int levelPass2)
    {
        float returnRate = 0;
        returnRate = craftingRateBase;
        returnRate += craftingRateIncrement * levelPass;
        if(levelPass2 > 1)
        {
            returnRate *= 3;
        }
        return returnRate;
    }

    public long GetSoulGather(int levelPass, int levelPass2)
    {
        long returnGather = 0;
        returnGather = soulGatherBase + (soulGatherIncrement * levelPass);
        if(levelPass2 >= 2)
        {
            returnGather *= 7;
        }
        return returnGather;
    }

    public float GetSoulGatherRate(int levelPass)
    {
        float returnRate = 0;
        returnRate = soulGatherMultiplierBase;
        returnRate += soulGatherMultiplierIncrement * levelPass;
        return returnRate;
    }

    public float GetSoulSpawnRate(int levelPass)
    {
        float returnRate = 0;
        returnRate = soulSpawnMultiplierBase;
        returnRate += soulSpawnMultiplierIncrement * levelPass;
        return returnRate;
    }

    public float GetMartRate(int levelPass)
    {
        float returnRate = 0;
        returnRate = martRateBase;
        returnRate += martRateIncrement * levelPass;
        return returnRate;
    }

    public long GetFlatGatherBonus(long currency1, long currency2, int upgradeLevel, int upgradeLevel2 = 0)
    {
        long returnRate = 0;
        returnRate = Global.craftableGatheringBonus * currency1;
        returnRate += Global.megaCraftableGatheringBonus * currency2;
        if(upgradeLevel > 0)
        {
            returnRate += upgradeLevel * Global.studyCurrencyBonus;
        }
        if(upgradeLevel2 > 1)
        {
            returnRate += (upgradeLevel2 - 1) * Global.communingBonus;
        }

        if (constructedLight1)
        {
            returnRate += Global.lightCrystalGatheringBonus;
        }
        if (constructedLight2)
        {
            returnRate += Global.greaterLightCrystalGatheringBonus;
        }
        
        return returnRate;
    }

    public int GetGroupPickupMax(int levelPass)
    {
        int returnRate = 0;
        returnRate = groupPickupMaxBase;
        returnRate += groupPickupIncrement * levelPass;
        return returnRate;
    }

    public float GetXRate(int levelPass)
    {
        float returnRate = 0;


        return returnRate;
    }

    public int GetCost(int index, int baseCost)
    {
        int finalCost = 0;

        switch (upgrades[index].costType)
        {
            case UpgradeData.CostType.Additive:

                finalCost = baseCost * upgrades[index].level;

                break;

            case UpgradeData.CostType.Exponential:

                finalCost = Mathf.RoundToInt(Mathf.Pow(baseCost, upgrades[index].level));

                break;

            case UpgradeData.CostType.Doubling:

                finalCost = Mathf.RoundToInt(baseCost * Mathf.Pow(2, upgrades[index].level));

                break;

            case UpgradeData.CostType.Static:

                finalCost = baseCost;

                break;
        }

        return finalCost;
    }

    public void CloseAllBuildMenus()
    {
        foreach(Building b in buildings)
        {
            if(b.buildMenu != null)
            {
                b.buildMenu.transform.parent.gameObject.SetActive(false);
            }
        }
        StartCoroutine(SetActiveDelay(tooltip.tooltip.gameObject, Global.frameTime, false));
        selectedBuilding = null;
    }

    public void ActivateSouls(int count)
    {
        for(int ii = 0; ii < count; ii++)
        {
            ActivateSoul();
        }
    }

    public void ActivateSoul()
    {
        tempSoul = FindInactiveSoul();
        if(tempSoul != null)
        {
            tempSoul.gameObject.SetActive(true);
            tempSoul.ActivateSoul();
        }
    }

    public Builder FindIdleBuilder()
    {
        Builder found = null;

        for (int ii = 0; ii < builders.Length; ii++)
        {
            if (found == null)
            {
                if (builders[ii].gameObject.activeInHierarchy && builders[ii].state == Builder.State.Idle)
                {
                    found = builders[ii];
                }
            }
            else
            {
                ii = builders.Length;
            }
        }

        return found;
    }

    public Builder FindInactiveBuilder()
    {
        Builder found = null;
        
        for(int ii = 0; ii < builders.Length; ii++)
        {
            if(found == null)
            {
                if(builders[ii].gameObject.activeInHierarchy == false)
                {
                    found = builders[ii];
                }
            }
            else
            {
                ii = builders.Length;
            }
        }

        return found;
    }

    public Builder FindInactiveWisp()
    {
        Builder found = null;

        for (int ii = 0; ii < wisps.Length; ii++)
        {
            if (found == null)
            {
                if (wisps[ii].gameObject.activeInHierarchy == false)
                {
                    found = wisps[ii];
                }
            }
            else
            {
                ii = wisps.Length;
            }
        }

        return found;
    }

    public Soul FindInactiveSoul()
    {
        Soul found = null;

        for (int ii = 0; ii < souls.Length; ii++)
        {
            if (found == null)
            {
                if (souls[ii].gameObject.activeInHierarchy == false)
                {
                    found = souls[ii];
                }
            }
            else
            {
                ii = souls.Length;
            }
        }

        return found;
    }

    public void CreateGatherNumber(GameObject target, int amount)
    {
        //if(mainCam.zoomLevel > 2) { return; }//don't show numbers if too zoomed out, performance and clarity
        gatherNumberPrefab.Spawn(target.transform.position, "+" + ToKMB((decimal)amount));
    }

    public string ToKMB(decimal num)
    {
        if (num > 999999999 || num < -999999999)
        {
            return num.ToString("0,,,.###B", CultureInfo.InvariantCulture);
        }
        else
        if (num > 999999 || num < -999999)
        {
            return num.ToString("0,,.##M", CultureInfo.InvariantCulture);
        }
        else
        if (num > 999 || num < -999)
        {
            return num.ToString("0,.#K", CultureInfo.InvariantCulture);
        }
        else
        {
            return num.ToString(CultureInfo.InvariantCulture);
        }
    }

    public void EnterNightMode(float delay = 0f)
    {
        fade.StartFadeInSprite(nightsky, 0.1f);
        foreach (SpriteRenderer s in clouds)
        {
            fade.StartColorFadeSprite(s, 0.1f, Color.white, Color.grey, delay);
        }
    }

    public void ExitNightMode(float delay = 3f)
    {
        fade.StartFadeOutSprite(nightsky, 0.1f, false, delay);
        foreach(SpriteRenderer s in clouds)
        {
            fade.StartColorFadeSprite(s, 0.1f, Color.grey, Color.white, delay);
        }
    }

}


public static class Global
{
    public static int craftableGatheringBonus = 1;
    public static int megaCraftableGatheringBonus = 10;
    public static int lightCrystalGatheringBonus = 120;
    //public static int mega2CraftableGatheringBonus = 1000;
    public static int greaterLightCrystalGatheringBonus = 500;
    public static int lightCrystalSoulBonus = 1000;

    public static int wispGatherMultiplier = 3;

    public static float mapWidth = 30;
    public static float soulStartPoint = 8;

    public static int gatherUpgrade1 = 50;
    public static int gatherUpgrade2 = 50;
    public static int gatherUpgrade3 = 500;

    public static int wellUnlockStartingWisps = 5;
    public static int unlockStartingWorkers = 10;

    public static int maxBuilderCarry = 10;

    public static int studyCurrencyBonus = 10;
    public static int communingBonus = 10;

    public static float noMetronomeRate = 2f;

    public static int groupPickupPrestigeBonus = 4;

    public static float frameTime = 0.0166666666666667f;
}

