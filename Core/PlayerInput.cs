
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerInput : MonoBehaviour
{
    public GameManager GM;
    public List<Builder> heldBuilders = new List<Builder>();
    public Building hoverBuilding;
    public Vector3 carryOffset = new Vector3(-0.1f, -0.05f, 0);
    public float stackingOffset = 0.01f;
    public FollowCursor collector;
    public GameObject collectorAnimation;
    public Magnet magnet;
    public Vector3[] carryOffsets = new Vector3[20];
    public TextMeshPro carryingCapacityText;
    public Vector3 carryingCapacityOffset;
    public GameObject[] clickParticles = new GameObject[2];
    public Vector3[] clickParticlesOffsets = new Vector3[2];
    Vector3 tempPos;
    Vector3 tempOffset;
    Vector3 screenToWorld;

    float magnetTimer = 0;
    float magnetDelay = 0.25f;
    bool activateMagnet = false;
    float holdTime = 0;
    Vector3 mousePos;
    GameObject tempParticles;

    public void Update()
    {
        if(heldBuilders.Count > 0)
        {
            screenToWorld = GM.mainCam.cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());//camera version
            for (int ii = 0; ii < heldBuilders.Count; ii++)
            {
                tempPos = screenToWorld;
                tempPos.z = -1;
                tempPos += carryOffsets[ii] * 2;
                //Debug.Log(tempPos);
                heldBuilders[ii].gameObject.transform.position = tempPos;
            }
            if(GM.groupPickupMax > 1)
            {
                carryingCapacityText.gameObject.SetActive(true);
                carryingCapacityText.text = heldBuilders.Count.ToString() + " / " + GM.groupPickupMax.ToString();
                carryingCapacityText.gameObject.transform.position = screenToWorld + carryingCapacityOffset;
                switch (GM.mainCam.zoomLevel)
                {
                    case 0:
                        carryingCapacityText.fontSize = 3;
                        break;
                    case 1:
                        carryingCapacityText.fontSize = 4;
                        break;
                    case 2:
                        carryingCapacityText.fontSize = 6;
                        break;
                }
            }
        }
        else
        {
            carryingCapacityText.gameObject.SetActive(false);
        }
        if (Mouse.current.leftButton.isPressed)
        {
            //magnetTimer += Time.deltaTime;
            //if(magnetTimer > magnetDelay)
            //{
            //    collector.gameObject.SetActive(true);
            //}
            //collector.gameObject.SetActive(false);
            holdTime += Time.deltaTime;
        }
        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (hoverBuilding != null && heldBuilders.Count <= 0)
            {
                if (hoverBuilding.collectible && holdTime > magnetDelay)
                {
                    collectorAnimation.gameObject.SetActive(true);
                    tempPos = GM.mainCam.cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());//camera version
                    tempPos.z = 0;
                    collectorAnimation.gameObject.transform.position = tempPos;
                }
            }

            Release();

            holdTime = 0;
        }
        if(hoverBuilding != null)
        {
            if (hoverBuilding.collectible) 
            {
                if(collector.gameObject.activeInHierarchy == false)
                {
                    collector.gameObject.SetActive(true);//change the cursor to a magnet?
                }
            }
            else
            {
                collector.gameObject.SetActive(false);
            }
        }
        else { collector.gameObject.SetActive(false); }
    }

    public void Release()
    {
        if(hoverBuilding != null && heldBuilders.Count > 0)
        {
            if(hoverBuilding.CanAssign() == false)//don't drop workers on already constructed buildings, but there are exceptions.
            {
                for (int ii = 0; ii < heldBuilders.Count; ii++)
                {
                    heldBuilders[ii].SetState(Builder.State.GentleFalling);
                }
                heldBuilders.Clear();
                return;
            }
            hoverBuilding.AssignBuilders(heldBuilders);
            for (int ii = 0; ii < heldBuilders.Count; ii++)
            {
                tempPos = GM.mainCam.cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());//camera version
                tempPos.z = heldBuilders[ii].gameObject.transform.position.z;
                if(heldBuilders.Count > 1)//don't offset if only carrying one, creates a jarring effect
                {
                    tempPos += new Vector3(carryOffsets[ii].x * Random.Range(2f, 3f), carryOffsets[ii].y * Random.Range(2f, 3f), 0);
                }
                else
                {
                    tempPos += carryOffsets[ii] * 2;
                }
                tempPos.z += Random.Range(0.001f, 0.00001f);
                heldBuilders[ii].gameObject.transform.position = tempPos;
            }
            heldBuilders.Clear();
        }
        else
        {
            for(int ii = 0; ii < heldBuilders.Count; ii++) //drop builders
            {
                heldBuilders[ii].SetState(Builder.State.GentleFalling);
            }
            heldBuilders.Clear();
        }
    }

    public void PlayClickParticles(int index)
    {
        mousePos = GM.mainCam.cam.ScreenToWorldPoint(Pointer.current.position.ReadValue());
        tempParticles = FindClickParticles(clickParticles);
        if(tempParticles != null)
        {
            tempParticles.gameObject.SetActive(true);
            mousePos.z = -1f;
            mousePos += clickParticlesOffsets[index];
            tempParticles.transform.position = mousePos;
        }
    }

    public GameObject FindClickParticles(GameObject[] arrayPass) 
    {
        GameObject returnObj = null;
        for(int ii = 0; ii < arrayPass.Length; ii++)
        {
            if (arrayPass[ii].activeInHierarchy == false)
            {
                returnObj = arrayPass[ii];
                ii = arrayPass.Length;
            }
        }
        return returnObj;
    }
}
