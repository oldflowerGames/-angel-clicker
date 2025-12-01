using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class GenericHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject tooltip, landscapeTooltip, portraitTooltip;
    public bool available = true;
    public GameManager GM;
    public float delayTime = 0;
    public bool ignoreMobile = false;
    public bool moveTooltip = false;
    public Vector3 tooltipOffset = new Vector3(-0.15f, 0, 0);
    public TextMeshProUGUI text, text2;
    bool waiting = false;

    public void Reset()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (GM == null)
            {
                GM = GameObject.Find("GameManager").GetComponent<GameManager>();
            }
            if (tooltip == null)
            {
                for (int ii = 0; ii < transform.childCount; ii += 1)
                {
                    GameObject temp = transform.GetChild(ii).gameObject;
                    if (temp.name.Contains("Tooltip"))
                    {
                        tooltip = landscapeTooltip = portraitTooltip = temp;
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //if(GM.helpMode == false && GM.onMobile == false && available)//if helpMode is true or you're on mobile, totally ignore this script
        if (GM.onMobile && ignoreMobile) { return; }
        if (available)
        {
            if (delayTime != 0 && GM.onMobile == false)
            {
                if (waiting == false)
                {
                    StartCoroutine(ShowDelay());
                }
            }
            else
            {
                ShowTooltip();
            }
        }
    }

    public void ShowTooltip()
    {
        //if (GM != null)
        //{
        //    //GM.helpManager.HideAllTooltips();

        //    if (GM.settingsSave.landscape)
        //    {
        //        landscapeTooltip.gameObject.SetActive(true);
        //        //if (moveTooltip)
        //        //{
        //        //    landscapeTooltip.gameObject.transform.position = gameObject.transform.position + new Vector3(Global.screenWidthCamera * tooltipOffset.x, Global.screenHeightCamera * tooltipOffset.y, 0);
        //        //}
        //    }
        //    else
        //    {
        //        portraitTooltip.gameObject.SetActive(true);
        //    }
        //}
        //else
        //{
            tooltip.gameObject.SetActive(true);
            //if (moveTooltip)
            //{
            //    tooltip.gameObject.transform.position = gameObject.transform.position + new Vector3(Global.screenWidthCamera * tooltipOffset.x, Global.screenHeightCamera * tooltipOffset.y, 0);
            //}
        //}
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExit();
    }

    private void OnDisable()
    {
        PointerExit();
    }

    public void PointerExit()
    {
        if (GM.onMobile) { return; }
        if (GM.helpMode == false)
        {
            DisableTooltip();
        }
        if (delayTime != 0)
        {
            StopAllCoroutines();
            waiting = false;
        }
    }

    public IEnumerator ShowDelay()
    {
        waiting = true;
        yield return new WaitForSeconds(delayTime);
        ShowTooltip();
        waiting = false;
    }

    public void DisableTooltip()
    {
        tooltip.gameObject.SetActive(false);
        if (landscapeTooltip != null) { landscapeTooltip.SetActive(false); }
        if (portraitTooltip != null) { portraitTooltip.SetActive(false); }
    }
}
