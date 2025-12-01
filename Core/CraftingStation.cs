using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CraftingStation : MonoBehaviour
{
    public GameManager GM;
    public Building attachedBuilding;
    public List<int> craftingQueue = new List<int>();
    public Slider slider, auxSlider;
    public Image[] queueImages = new Image[20];
    public float craftingProgress;
    public float craftingRequirement;
    public int activeCraftable;
    public GameObject notification;
    public Button removeFromQueueButton;
    public float bonusRate;
    public float bonusRateIncrement = 0.1f;
    public float bonusRateDecel = 0.1f;
    public float maxBonusRate = 3f;
    public TextMeshProUGUI bonusRateText, queueCountText;
    public SFXPool smallCraftSFX, bigCraftSFX;
    float craftingAdd = 0;
    // Update is called once per frame
    void Update()
    {
        if(attachedBuilding == null) { return; }
        if(attachedBuilding.constructed == false) { return; }

        if(activeCraftable <= -1 && craftingQueue.Count > 0)
        {
            StartCrafting(craftingQueue[0]);
        }

        if (bonusRate > 0)
        {
            bonusRate = Mathf.Max(0, bonusRate - bonusRateDecel * Time.deltaTime);
            bonusRateText.text = "x" + (1 + bonusRate).ToString("0.0");
        }
        else
        {
            bonusRateText.text = "";
        }

        if (activeCraftable <= -1) 
        {
            if (CanAfford() && attachedBuilding.buildMenu.gameObject.activeInHierarchy == false)
            {
                notification.SetActive(true);
            }
            return;
        };

        craftingAdd = Time.deltaTime * (GM.craftingRate + bonusRate);
        if (GM.metronomeDisplay.toggle.isOn == false) { craftingAdd *= Global.noMetronomeRate; }

        craftingProgress += craftingAdd;

        if(craftingProgress >= craftingRequirement)
        {
            craftingProgress = 0;
            CompleteCraft(activeCraftable);
            activeCraftable = -1;
            CheckQueueSprites();
            //will automatically start again on the next frame
        }

        slider.value = auxSlider.value = craftingProgress;
        slider.maxValue = auxSlider.maxValue = craftingRequirement;
    }

    public bool CanAfford()
    {
        bool canAfford = false;
        if (GM.currencies[0] > GM.upgrades[13].baseCost) { canAfford = true; }
        if (GM.currencies[1] > GM.upgrades[14].baseCost) { canAfford = true; }
        if (GM.currencies[2] > GM.upgrades[15].baseCost) { canAfford = true; }

        return canAfford;
    }

    public void QueueCraftable(int index)
    {
        craftingQueue.Add(index);
        CheckQueueSprites();
    }

    public void RemoveFromQueue()
    {
        if (Keyboard.current.shiftKey.isPressed)
        {
            int removeCount = Mathf.Min(10, craftingQueue.Count + 1);
            for (int ii = 0; ii < removeCount; ii++)
            {
                RemoveFromQueueExecute();
            }
        }
        else
        {
            RemoveFromQueueExecute();
        }
    }

    public void RemoveFromQueueExecute()
    {
        int removed = 0;
        if(activeCraftable == -1) { return; }
        if (craftingQueue.Count == 0) 
        {
            removed = activeCraftable;
            activeCraftable = -1;
            craftingProgress = 0;
            slider.value = auxSlider.value = 0;
        }
        else
        {
            removed = craftingQueue[craftingQueue.Count - 1];
            craftingQueue.RemoveAt(craftingQueue.Count - 1);
        }

        CheckQueueSprites();

        //refund
        int convertedIndex = ConvertCraftable(removed);
        for(int ii = 0; ii < GM.upgrades[convertedIndex].costs.Length; ii++) 
        {
            GM.AddCurrencyAmount(GM.upgrades[convertedIndex].costs[ii].currencyIndex, GM.upgrades[convertedIndex].costs[ii].cost);
        }
        GM.upgrades[convertedIndex].level -= 1;
        attachedBuilding.buildMenu.Populate();
    }

    public void StartCrafting(int index)
    {
        activeCraftable = index;
        craftingProgress = 0;
        craftingQueue.RemoveAt(0);

        switch (index)
        {
            case 5://ash
                craftingRequirement = 6;
                break;
            case 6://stone
                craftingRequirement = 6;
                break;
            case 7://tear
                craftingRequirement = 6;
                break;
            case 8://mega wood
                craftingRequirement = 20;
                break;
            case 9://megastone
                craftingRequirement = 20;
                break;
            case 10://megawater
                craftingRequirement = 20;
                break;
            case 11://light crystal 1
                craftingRequirement = 100;
                break;
            case 12://light crystal 2
                craftingRequirement = 1000;
                break;
        }

        CheckQueueSprites();
    }

    public int ConvertCraftable(int indexPass)
    {
        int returnVal = 0;
        switch(indexPass)
        {
            case 5:
                returnVal = 13;
                break;
            case 6:
                returnVal = 14;
                break;
            case 7:
                returnVal = 15;
                break;
            case 8:
                returnVal = 16;
                break;
            case 9:
                returnVal = 17;
                break;
            case 10:
                returnVal = 18;
                break;
            case 11:
                returnVal = 19;
                break;
            case 12:
                returnVal = 32;
                break;
        }
        return returnVal;
    }

    public void CompleteCraft(int index)
    {
        switch (index)
        {
            case 5://ash
                GM.AddCraftable(5);
                break;
            case 6://stone
                GM.AddCraftable(6);
                break;
            case 7://tear
                GM.AddCraftable(7);
                break;
            case 8://mega wood
                GM.AddCraftable(8);
                break;
            case 9://megastone
                GM.AddCraftable(9);
                break;
            case 10://megawater
                GM.AddCraftable(10);
                break;
            case 11://light crystal 1
                GM.BuildCrystal(0);
                break;
            case 12://light crystal 2
                GM.BuildCrystal(1);
                break;
        }

        if(index == 5 || index == 6 || index == 7)
        {
            smallCraftSFX.SelectSound(1);
        }
        if(index == 8 || index == 9 || index == 10)
        {
            bigCraftSFX.SelectSound(1);
        }
    }

    public void CheckQueueSprites()
    {
        foreach(Image i in queueImages) { i.gameObject.SetActive(false); }
        if(activeCraftable != -1)
        {
            queueImages[0].sprite = GM.craftingIcons[activeCraftable];
            queueImages[0].gameObject.SetActive(true);
            removeFromQueueButton.gameObject.SetActive(true);
        }
        else
        {
            removeFromQueueButton.gameObject.SetActive(false);
        }
        for(int ii = 0; ii < Mathf.Min(queueImages.Length - 1, craftingQueue.Count); ii++)
        {
            queueImages[ii + 1].gameObject.SetActive(true);
            queueImages[ii + 1].sprite = GM.craftingIcons[craftingQueue.ElementAt(ii)];
        }

        slider.value = auxSlider.value = craftingProgress;
        slider.maxValue = auxSlider.maxValue = craftingRequirement;
        if(craftingQueue.Count > 0)
        {
            queueCountText.text = craftingQueue.Count.ToString();
        }
        else
        {
            queueCountText.text = "";
        }

    }



    public void ResetStation()
    { 
        activeCraftable = -1;
        craftingQueue.Clear();
        slider.value = auxSlider.value = 0;
        craftingProgress = 0;
        foreach (Image i in queueImages) { i.gameObject.SetActive(false); }
    }

}
