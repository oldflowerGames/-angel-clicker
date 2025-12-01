using NUnit.Framework;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class Builder : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    public GameManager GM;
    public float lifetime = 10;
    public float remainingLifetime = 10;
    public List<Sprite> spriteList = new List<Sprite>();
    public SpriteRenderer spriteRenderer;
    public enum State
    {
        Idle,
        WalkingToBuild,
        Climbing,
        Building,
        Falling,
        GentleFalling,
        Unconscious,
        GatherWood,
        GatherRock,
        GatherWater,
        Carrying,
        TendingToMart,
        AssignedToStudy,
        Studying
    }
    public State state;

    public enum BuilderType
    {
        Minion,
        Wisp
    }
    public BuilderType builderType;

    public float gatherInterval = 1f;
    public float gatherTimer = 0;

    public float buildInterval = 0.5f;
    public float buildTimer = 0;

    public Building assignedBuilding;

    public float fallingVelocity;
    public float fallingAcceleration;
    public bool fallingRight = false;
    public GameObject explosion;
    Vector3 fallingVector;
    public float horizontalRate = 1;
    public float rotationRate = 100;
    float fallingY;

    public BoxCollider2D boxCollider;
    public BoxCollider2D fallingCollider;

    public float frameTime = 0.5f;
    public float frameTimer = 0;
    public int frameTracker = 0;

    public bool interactable = true;

    public SpriteRenderer[] lifetimeBar = new SpriteRenderer[4];
    public AudioSource audioSource;

    bool gatherThisFrame, buildThisFrame, depleteLifetime;
    int gatherAdd;
    float lifetimePercent;

    public Vector3 carryOffset;
    public float landingOffset = 0.2f;

    // Update is called once per frame
    void Update()
    {
        gatherAdd = 0;
        if (builderType == BuilderType.Minion)
        {
            gatherTimer += Time.deltaTime * GM.gathererSpeed;
        }
        else if(builderType == BuilderType.Wisp)
        {
            gatherTimer += Time.deltaTime * GM.wispSpeed;
        }

        if(gatherTimer > gatherInterval)
        {
            gatherThisFrame = true;
            gatherTimer = 0;
        }
        else { gatherThisFrame = false; }

        buildTimer += Time.deltaTime * GM.builderSpeed;
        if(buildTimer > buildInterval)
        {
            buildThisFrame = true;
            buildTimer = 0;
        }
        else { buildThisFrame = false; }

        depleteLifetime = false;

        switch(state)
        {
            case State.Idle:
                if(remainingLifetime < lifetime) 
                {
                    remainingLifetime += GM.builderLifetimeRegen * Time.deltaTime;
                    CheckLifetimeBar();
                }

                break;

            case State.WalkingToBuild:

                break;

            case State.Climbing: 

                break;

            case State.Building:
                if(assignedBuilding != null)
                {
                    if (assignedBuilding.constructed == false && buildThisFrame)
                    {
                        assignedBuilding.IncrementBuildProgress();
                    }
                    depleteLifetime = true;
                }
                break;

            case State.Falling:
                fallingY = fallingVelocity * Time.deltaTime;

                if (fallingRight)
                {
                    fallingVector = new Vector3(horizontalRate * Time.deltaTime, fallingY, 0);
                }
                else
                {
                    fallingVector = new Vector3(-horizontalRate * Time.deltaTime, fallingY, 0);
                }

                transform.Translate(fallingVector);
                fallingVelocity += (fallingAcceleration * Time.deltaTime);
                spriteRenderer.transform.Rotate(Vector3.forward, rotationRate * Time.deltaTime);

                if(transform.position.y <= GM.builderSpawnPos.y + landingOffset)//slight offset
                {
                    SetState(State.Unconscious);
                }
                break;

            case State.GentleFalling:
                fallingY = fallingVelocity * Time.deltaTime;
                fallingVector = new Vector3(0, fallingY, 0);

                transform.Translate(fallingVector);
                //fallingVelocity += (fallingAcceleration * Time.deltaTime);

                if (transform.position.y < GM.builderSpawnPos.y)
                {
                    SetState(State.Idle);
                }
                break;

            case State.Unconscious:

                break;

            case State.GatherWood:
                if (gatherThisFrame)
                {
                    gatherAdd = Mathf.RoundToInt(GM.currencyGains[0] * GM.gathererMultiplier);
                    if(builderType == BuilderType.Wisp)//wisp
                    {
                        gatherAdd *= Global.wispGatherMultiplier;//wisps get extra wood gathering automatically (but are slower)
                    }
                    GM.currencies[0] += gatherAdd;
                    GM.CreateGatherNumber(gameObject, gatherAdd);
                    if(builderType == BuilderType.Minion)//wisps don't make gathering sounds
                    {
                        PlayBuildingSound();
                    }
                }
                break;

            case State.GatherRock:
                if (gatherThisFrame)
                {
                    gatherAdd = Mathf.RoundToInt(GM.currencyGains[1] * GM.gathererMultiplier);
                    GM.currencies[1] += gatherAdd;
                    GM.CreateGatherNumber(gameObject, gatherAdd);
                    PlayBuildingSound();

                    //audioSource.clip = GM.sfx.GetClip(GM.sfx.builderStoneGather);
                    //PlaySound(0.9f, 1.1f);
                }
                break;

            case State.GatherWater:
                if (gatherThisFrame)
                {
                    gatherAdd = Mathf.RoundToInt(GM.currencyGains[2] * GM.gathererMultiplier);
                    GM.currencies[2] += gatherAdd;
                    GM.CreateGatherNumber(gameObject, gatherAdd);
                }
                break;

            case State.TendingToMart:

                break;
        }

        //animation
        frameTimer += Time.deltaTime * GM.gameSpeed;
        if(frameTimer > frameTime)
        {
            frameTimer = 0;
            frameTracker += 1;
            if(frameTracker >= spriteList.Count)
            {
                frameTracker = 0;
            }
            spriteRenderer.sprite = spriteList[frameTracker];
        }

        //lifetime bar
        if (depleteLifetime && remainingLifetime > 0 && GM.prestige.unlockedUnlimitedLifespan == false)
        {
            remainingLifetime -= Time.deltaTime;
            if(remainingLifetime <= 0)
            {
                SetState(State.Falling);
            }
            CheckLifetimeBar();
        }
        else if (GM.prestige.unlockedUnlimitedLifespan && builderType == BuilderType.Minion)
        {
            remainingLifetime = lifetime;
            lifetimeBar[0].transform.parent.gameObject.SetActive(false);
        }
        else if(remainingLifetime >= lifetime && interactable)
        {
            lifetimeBar[0].transform.parent.gameObject.SetActive(false);
        }
    }

    public void SetState(State statePass)
    {
        state = statePass;

        if (boxCollider != null) { boxCollider.enabled = true; }//will be disabled elsewhere, but is almost always active by default
        if(fallingCollider != null) { fallingCollider.enabled = false; }//only used in one state, being extra cautious

        switch (statePass)
        {
            case State.Idle:
                spriteList = GM.DB.idleSprites;
                break;

            case State.WalkingToBuild:
                spriteList = GM.DB.walkingSprites;
                break;

            case State.Climbing:

                break;

            case State.Building:
                spriteList = GM.DB.constructingSprites;
                break;

            case State.Falling:
                spriteList = GM.DB.fallingSprites;
                RemoveFromBuilding();
                fallingVelocity = 0.5f;
                if(UnityEngine.Random.Range(0, 2) == 1)
                { 
                    fallingRight = true;
                }
                else { fallingRight = false; }
                lifetimeBar[0].transform.parent.gameObject.SetActive(false);
                audioSource.clip = GM.sfx.GetClip(GM.sfx.builderScream);
                PlaySound(0.95f, 1.05f);
                if (fallingCollider != null) { fallingCollider.enabled = true; }
                break;

            case State.GentleFalling:
                spriteList = GM.DB.gentleFallingSprites;
                fallingVelocity = -1.5f;
                lifetimeBar[0].transform.parent.gameObject.SetActive(false);
                //audioSource.clip = GM.sfx.GetClip(GM.sfx.builderScream);
                //PlaySound(0.95f, 1.05f);
                break;

            case State.Unconscious:
                spriteList = GM.DB.unconsciousSprites;
                explosion.SetActive(true);
                spriteRenderer.transform.rotation = Quaternion.Euler(Vector3.zero);
                remainingLifetime = lifetime;
                transform.position = new Vector3(transform.position.x, GM.builderSpawnPos.y, transform.position.z);
                audioSource.clip = GM.sfx.GetClip(GM.sfx.builderExplosion);
                PlaySound(0.95f, 1.05f, 0.25f);
                break;

            case State.GatherWood:
                if(builderType == BuilderType.Minion)
                {
                    gatherInterval = 2f;
                    spriteList = GM.DB.gatherWoodSprites;
                }
                else if(builderType == BuilderType.Wisp)
                {
                    gatherInterval = 3.5f;
                }
                break;

            case State.GatherRock:
                spriteList = GM.DB.gatherRockSprites;
                gatherInterval = 3;
                break;

            case State.Carrying:
                spriteList = GM.DB.carryingSprites;
                RemoveFromBuilding();
                boxCollider.enabled = false;
                remainingLifetime = lifetime;//not sure here, best thing would be to just pick up minions and put them right back, but that's fine...?
                audioSource.clip = GM.sfx.GetClip(GM.sfx.builderPickup);
                PlaySound(0.95f, 1.05f, 0.25f);
                break;

            case State.TendingToMart:
                spriteList = GM.DB.martSprites;
                break;

            case State.AssignedToStudy:
                spriteList = GM.DB.studyIdleSprites;
                break;

            case State.Studying:
                spriteList = GM.DB.studyActiveSprites;
                break;
        }

        spriteRenderer.sprite = spriteList[0];
    }

    public void RemoveFromBuilding()
    {
        if (assignedBuilding != null)
        {
            if (assignedBuilding.assignedBuilders.Contains(this))
            {
                assignedBuilding.assignedBuilders.Remove(this);
                if(assignedBuilding.buildMenu != null)
                {
                    if(assignedBuilding.buildMenu.studyMenu != null)
                    {
                        assignedBuilding.buildMenu.studyMenu.RemoveBuilder(this);
                    }
                }
            }
        }
    }

    public void SetToIdle(int index)
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, GM.builderSpawnPos.y, gameObject.transform.position.z + (index * 0.0001f));
        SetState(Builder.State.Idle);
    }

    public void PlayBuildingSound()
    {
        if (assignedBuilding != null)
        {
            if (assignedBuilding.queuedSounds == 0)
            {
                assignedBuilding.soundTimer = assignedBuilding.soundCooldown;
            }
            assignedBuilding.queuedSounds = Mathf.Min(10, assignedBuilding.queuedSounds + 1);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(interactable == false) { return; }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            PickUp();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(interactable == false) { return; }
        if (GM.playerInput.heldBuilders.Count > 0 && GM.groupPickupMax > 1)//if already in pickup mode, add to the hand
        {
            if(GM.playerInput.heldBuilders.Count < GM.groupPickupMax)
            {
                PickUp();
            }
        }
    }

    public void PickUp()
    {
        if (GM.playerInput.heldBuilders.Contains(this) == false && GM.playerInput.heldBuilders.Count < Global.maxBuilderCarry)
        {
            GM.playerInput.heldBuilders.Add(this);
            SetState(State.Carrying);
            assignedBuilding = null;
            spriteRenderer.transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }

    public void CheckLifetimeBar()
    {
        lifetimePercent = remainingLifetime / lifetime;
        lifetimeBar[0].transform.parent.gameObject.SetActive(true);
        foreach (SpriteRenderer s in lifetimeBar) { s.gameObject.SetActive(false); }
        if (lifetimePercent > 0)
        {
            lifetimeBar[0].gameObject.SetActive(true);
        }
        if (lifetimePercent > 0.25f)
        {
            lifetimeBar[1].gameObject.SetActive(true);
        }
        if (lifetimePercent > 0.5f)
        {
            lifetimeBar[2].gameObject.SetActive(true);
        }
        if(lifetimePercent >= 0.75f)
        {
            lifetimeBar[3].gameObject.SetActive(true);
        }
    }

    public void PlaySound(float minPitch = 1, float maxPitch = 1, float volume = 0.4f)
    {
        audioSource.pitch = Random.Range(minPitch, maxPitch);
        audioSource.volume = volume;
        audioSource.Play();
    }

}
