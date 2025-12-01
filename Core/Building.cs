using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static Builder;

public class Building : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    public GameManager GM;
    public BuildMenu buildMenu;
    public List<Builder> assignedBuilders = new List<Builder>();

    public enum BuildingID
    {
        Tower,
        Dorm,
        Well,
        TowerExpand1,
        Genesis,
        Crystal1,
        Mart,
        Research,
        TowerExpand2,
        Crystal2,
        Trees,
        Rock,
    }

    public BuildingID buildingID;

    public bool collectible = false;
    public int collectibleIndex;
    public List<Collectible> collectibles = new List<Collectible>();
    public float collectibleSpawnRange = 1f;
    public bool availableToCollect = false;
    public float collectibleCooldown = 0.27f;
    public float collectibleTimer = 0;
    public int collectibleTicksRequired = 0;
    public int collectibleTickTracker = 0;
    public bool autoCollecting = false;

    public SpriteRenderer spriteRenderer;

    public float constructionProgress;
    public float constructionRequirement;
    public bool constructed;
    public bool alwaysConstructed;
    public Sprite[] constructionSprites;
    public Slider constructionSlider;

    public int upgradeLevel;
    public Sprite[] upgradeSprites = new Sprite[5];
    public Sprite baseSprite;

    public List<GameObject> attachedObjs = new List<GameObject>();

    public GameObject notification;
    public bool ignoreBar;
    public BoxCollider2D boxCollider;

    public AudioSource audioSource;
    public SFXPool manualGatherPool, gathererPool, constructionPool;
    public AudioSource constructionLoop, passiveLoop;
    public float soundCooldown = 0.05f;
    public float baseSoundCooldown = 0.05f;
    public float soundCooldownRange = 0.01f;
    public float soundTimer = 0;
    public int queuedSounds = 0;

    Builder tempBuilder;
    Vector3 tempMenuPos;
    Vector3 mousePos;



    public void Start()
    {
        if(boxCollider == null)
        {
            boxCollider = gameObject.GetComponent<BoxCollider2D>();
        }
    }

    public void Update()
    {
        if (collectible == false){ return; }
        collectibleCooldown = GM.metronomeDisplay.interval * 0.5f;

        if(availableToCollect == false)
        {
            collectibleTimer += Time.deltaTime;
            if(collectibleTimer > collectibleCooldown)
            {
                availableToCollect = true;
                collectibleTimer = 0;
                if (autoCollecting)
                {
                    CollectibleTick();
                }
            }
        }

        if(queuedSounds > 0)
        {
            soundTimer += Time.deltaTime;
            if(soundTimer > soundCooldown)
            {
                soundTimer = 0;
                if(constructed == false)
                {
                    constructionPool.SelectSound(1);
                }
                else
                {
                    gathererPool.SelectSound(1);
                }
                queuedSounds -= 1;
                soundCooldown = baseSoundCooldown + Random.Range(-soundCooldownRange, soundCooldownRange);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            SelectThis();
        }
        else if (eventData.button == PointerEventData.InputButton.Right && GM.prestige.unlockedAutoAssign)
        {
            if (constructed)
            {
                if (collectible == false) { return; }//can't assign a builder to a constructed, not-collectible building
            }
            tempBuilder = GM.FindIdleBuilder();
            if(tempBuilder != null)
            {
                AssignBuilder(tempBuilder, true, 0);
            }
            //assign Agent
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            autoCollecting = false;
        }
    }

    public void CollectibleTick()
    {
        collectibleTickTracker += 1;
        availableToCollect = false;
        if (collectibleTickTracker >= collectibleTicksRequired)
        {
            collectibleTickTracker = 0;
            EnableCollectible();
        }

        GM.playerInput.PlayClickParticles(0);

        if (manualGatherPool != null)//play sound on every tick
        {
            manualGatherPool.SelectSound(1f);//each collectible has its own pool of sounds assigned
        }
    }
    
    public void AssignBuilders(List<Builder> builderList)
    {
        for(int ii = 0; ii < builderList.Count; ii++)
        {
            AssignBuilder(builderList[ii], false, ii);
        }
    }

    public void AssignBuilder(Builder b, bool fromHQ, int count)
    {
        assignedBuilders.Add(b);
        b.assignedBuilding = this;

        switch (buildingID)
        {
            case BuildingID.Trees:
                b.SetState(Builder.State.GatherWood);
                break;
            case BuildingID.Rock:
                b.SetState(Builder.State.GatherRock);
                break;
            case BuildingID.Dorm:
                b.SetState(Builder.State.Building);
                break;
            case BuildingID.Well:
                b.SetState(Builder.State.Building);
                break;
            case BuildingID.TowerExpand1:
                b.SetState(Builder.State.Building);
                break;
            case BuildingID.Genesis:
                b.SetState(Builder.State.Building);
                break;
            case BuildingID.Research:
                if (constructed)
                {
                    b.SetState(Builder.State.AssignedToStudy);
                    buildMenu.studyMenu.CheckWorkers();
                }
                else
                {
                    b.SetState(Builder.State.Building);
                }

                break;
            case BuildingID.Mart:
                b.SetState(Builder.State.TendingToMart);
                break;
            case BuildingID.TowerExpand2:
                b.SetState(Builder.State.Building);
                break;
        }

        if (fromHQ)
        {
            b.transform.position = gameObject.transform.position;
            b.transform.position += new Vector3(UnityEngine.Random.Range(-collectibleSpawnRange, collectibleSpawnRange), UnityEngine.Random.Range(-collectibleSpawnRange, collectibleSpawnRange), UnityEngine.Random.Range(-0.1f, -0.2f));
        }

        b.transform.position += Vector3.back * count * 0.000001f;//very slight Z offset to prevent sort issues

        if(notification != null)
        {
            if (notification.gameObject.activeInHierarchy)
            {
                SetNotification(false);
            }
        }
    }

    public void StartConstruction()
    {
        gameObject.transform.parent.gameObject.SetActive(true);
        gameObject.SetActive(true);
        spriteRenderer.sprite = constructionSprites[Mathf.Min(upgradeLevel, constructionSprites.Length - 1)];
        constructed = false;
        constructionSlider.transform.parent.gameObject.SetActive(true);
        constructionSlider.value = 0;
        constructionSlider.maxValue = constructionRequirement;
        SetNotification(true);
        constructionLoop.Play();
        switch (buildingID)
        {
            case BuildingID.TowerExpand1:
                GM.buildings[0].spriteRenderer.sprite = GM.buildings[3].baseSprite;
                break;
            case BuildingID.TowerExpand2:
                GM.buildings[0].spriteRenderer.sprite = GM.buildings[8].baseSprite;
                break;
            case BuildingID.Genesis:
                if(boxCollider != null)
                {
                    boxCollider.offset = new Vector2(0, 0.66f);
                    boxCollider.size = Vector3.one * 1.3f;
                }
                break;
        }
    }

    public void IncrementBuildProgress()
    {
        constructionProgress += GM.constructionRate;

        if(constructionProgress >= constructionRequirement)
        {
            FinishBuilding();
        }
        else
        {
            constructionSlider.transform.parent.gameObject.SetActive(true);
            constructionSlider.value = constructionProgress;
            constructionSlider.maxValue = constructionRequirement;
            float constructionPercent = constructionProgress / constructionRequirement;
            for(int ii = 1; ii < constructionSprites.Length + 1; ii++)
            {
                if(constructionPercent <= (float)ii / (float)constructionSprites.Length)
                {
                    spriteRenderer.sprite = constructionSprites[ii - 1];
                    ii = constructionSprites.Length;
                }
            }
            constructionPool.SelectSound(1);



            //if(constructionProgress <= constructionRequirement * 0.33f)
            //{
            //    spriteRenderer.sprite = constructionSprites[0];
            //}
            //else if(constructionProgress <= constructionRequirement * 0.66f)
            //{
            //    spriteRenderer.sprite = constructionSprites[1];
            //}
            //else
            //{
            //    spriteRenderer.sprite = constructionSprites[2];
            //}
        }
    }

    public void FinishBuilding()
    {
        constructed = true;
        spriteRenderer.sprite = upgradeSprites[Mathf.Min(upgradeLevel, upgradeSprites.Length - 1)];
        constructionSlider.transform.parent.gameObject.SetActive(false);
        constructionLoop.Stop();
        GM.sfx.PlaySound(GM.sfx.constructionComplete);
        for (int ii = 0; ii < assignedBuilders.Count; ii++)
        {
            assignedBuilders[ii].assignedBuilding = null;
            if (assignedBuilders[ii].state == Builder.State.Building)
            {
                assignedBuilders[ii].SetState(Builder.State.GentleFalling);
                //assignedBuilders[ii].SetToIdle(ii);
            }
        }

        switch (buildingID)
        {
            case BuildingID.Well:
                passiveLoop.Play();
                break;
            case BuildingID.TowerExpand1:
                GM.buildings[3].gameObject.SetActive(false);//disable the in-progress tower
                GM.buildings[0].spriteRenderer.sprite = GM.buildings[0].upgradeSprites[1];//swap the sprite of the original tower object
                GM.upgrades[11].level = 99;//set above level 2 to indicate construction completed, unlocking pre-requisites
                GM.buildings[0].SetNotification(true);
                GM.buildings[0].SetBoxCollider(1);
                GM.sfx.PlaySound(GM.sfx.towerComplete);
                //expand the original tower's hitbox
                break;
            case BuildingID.TowerExpand2:
                GM.buildings[8].gameObject.SetActive(false);//disable the in-progress tower
                GM.buildings[0].spriteRenderer.sprite = GM.buildings[0].upgradeSprites[2];//swap the sprite of the original tower object
                GM.upgrades[30].level = 99;//set above level 2 to indicate construction completed, unlocking pre-requisites
                if (GM.upgrades[32].level >= 99)//if the crystal has already been constructed, you win!
                {
                    GM.buildings[9].transform.parent.gameObject.SetActive(true);
                    GM.PlayVictorySequence();
                    GM.constructedLight2 = true;
                    GM.SetValues();
                }
                GM.sfx.PlaySound(GM.sfx.towerComplete);
                //expand the original tower's hitbox
                break;
            case BuildingID.Genesis:
                GM.bonusCurrencyDisplay.SetActive(true);
                if(GM.prestige.prestigeCount == 0)
                {
                    GM.metronomeWindow.gameObject.SetActive(true);
                }
                if (boxCollider != null)
                {
                    boxCollider.offset = new Vector2(0, 1.35f);
                    boxCollider.size = new Vector2(0.5f, 2.35f);
                }
                break;

        }

        ToggleAttachedObjs(true);

        GM.buildingBar.PopulateBar();

        SetNotification(true);

        assignedBuilders.Clear();
    }

    public void SetBoxCollider(int index)
    {
        switch (buildingID)
        {
            case BuildingID.Tower:
                switch (index)
                {
                    case 0:
                        boxCollider.offset = new Vector2(0, 1f);
                        boxCollider.size = new Vector2(2.2f, 2f);

                        break;
                    case 1:
                        boxCollider.offset = new Vector2(0, 1.56f);
                        boxCollider.size = new Vector2(1, 3.1f);
                        break;
                }
                break;
        }
    }

    public void ResetBuilding()
    {
        for(int ii = 0; ii < assignedBuilders.Count; ii++)
        {
            assignedBuilders[ii].assignedBuilding = null;
        }
        assignedBuilders.Clear();
        if (alwaysConstructed) { constructed = true; }
        else { constructed = false; }
        spriteRenderer.sprite = baseSprite;
        constructionProgress = 0;
        if(notification != null)
        {
            notification.gameObject.SetActive(false);
        }
    }

    public void ResetCollectibles()
    {
        foreach(Collectible c in collectibles)
        {
            c.gameObject.SetActive(false);
        }
    }

    public void SelectThis()
    {
        if (collectible)
        {
            if (availableToCollect)
            {
                CollectibleTick();
                autoCollecting = true;
            }
        }
        else
        {
            if(constructed == false) { return; }//can't select a building that hasn't been constructed yet
            GM.CloseAllBuildMenus();
            GM.selectedBuilding = this;
            buildMenu.gameObject.transform.parent.gameObject.SetActive(true);
            SetNotification(false);
            GM.sfx.PlaySound(GM.sfx.selectBuilding);
            //tempMenuPos = GM.mainCam.cam.WorldToScreenPoint(buildMenu.gameObject.transform.position);
            //if(tempMenuPos.x < Screen.width * 0.3f || tempMenuPos.x > Screen.width * 0.8f)//if too close to the edge of the screen, flip to the other side. asymmetrical because of the currency display
            //{
            //    buildMenu.gameObject.transform.localPosition = new Vector3(-menuOriginalXPos, buildMenu.gameObject.transform.localPosition.y, buildMenu.gameObject.transform.localPosition.z);
            //}
            //enable additional visuals
            //GM.sfx.PlaySound(GM.sfx.tactile);
        }
    }

    public void EnableCollectible()
    {
        bool spawned = false;
        for(int ii = 0; ii < collectibles.Count; ii++)
        {
            if (collectibles[ii].gameObject.activeInHierarchy == false)
            {
                collectibles[ii].gameObject.SetActive(true);
                collectibles[ii].transform.position = FindSpawnPos();
                ii = collectibles.Count;
                spawned = true;
            }
        }

        switch (buildingID)
        {
            case BuildingID.Rock:

                break;
            case BuildingID.Trees:

                break;
        }

        if(spawned) { return; }

        //if you reach the cap of the object pool, instantiate one instead
        Collectible c = Instantiate(GM.DB.collectiblePrefabs[collectibleIndex]).GetComponent<Collectible>();
        c.instantiated = true;
        c.GM = GM;
        c.transform.position = gameObject.transform.position;
        c.transform.position += new Vector3(UnityEngine.Random.Range(-collectibleSpawnRange, collectibleSpawnRange), UnityEngine.Random.Range(-collectibleSpawnRange, collectibleSpawnRange), UnityEngine.Random.Range(-0.1f, -0.2f));
        collectibles.Add(c);
        c.transform.SetParent(collectibles[0].transform.parent);
    }

    public void SetNotification(bool enabling)
    {
        if(notification == null) { return; }
        notification.gameObject.SetActive(enabling);

        for (int ii = 0; ii < GM.buildingBar.options.Length; ii++) 
        {
            if (GM.buildingBar.options[ii].buildingIndex == (int)buildingID)
            {
                GM.buildingBar.options[ii].notificationIcon.SetActive(enabling);
            }
        }
    }

    public Vector3 FindSpawnPos()
    {
        Vector3 returnPos = Vector3.zero;
        bool foundPos = false;
        mousePos = GM.mainCam.cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());

        while (foundPos == false)
        {
            returnPos = gameObject.transform.position +
                new Vector3(UnityEngine.Random.Range(-collectibleSpawnRange, collectibleSpawnRange), UnityEngine.Random.Range(-collectibleSpawnRange, collectibleSpawnRange), UnityEngine.Random.Range(-0.1f, -0.2f));

            mousePos.z = returnPos.z;

            if (Vector3.Distance(returnPos, mousePos) > 0.5f) 
            {
                foundPos = true;
            }
        }

        return returnPos;
    }

    public bool CanAssign()
    {
        bool canAssign = true;
        if(constructed && collectible == false) { canAssign = false; }
        if(buildingID == BuildingID.Mart) { canAssign = true; }
        if(buildingID == BuildingID.Research) { canAssign = true; }
        return canAssign;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GM.playerInput.hoverBuilding = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GM.playerInput.hoverBuilding = null;
    }

    public void PlaySound(float minPitch = 1, float maxPitch = 1)
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.Play();
    }

    public void OnEnable()
    {
        if (constructed)
        {
            ToggleAttachedObjs(true);
        }
    }

    public void OnDisable()
    {
        ToggleAttachedObjs(false);   
    }

    public void ToggleAttachedObjs(bool enabling)
    {
        if (attachedObjs.Count > 0)
        {
            foreach (GameObject g in attachedObjs)
            {
                if(g != null) { g.SetActive(enabling); }
            }
        }
    }

}
