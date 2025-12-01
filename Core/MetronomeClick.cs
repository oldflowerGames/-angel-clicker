using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MetronomeClick : MonoBehaviour
{
    public GameManager GM;
    public MusicOverseer overseer;
    public float interval = 1;
    public float timer = 0;
    public float successThreshold = 0.1f;
    public Slider slider;
    public Toggle toggle;
    public TextMeshProUGUI autoBonusText;
    bool reverse = false;
    float tempTime = 0;
    float earlyTime = 0;
    public float rate = 1;
    public float BPM = 110;
    public float offset = 0;
    public int beats = 0;
    public Image[] successMarkers = new Image[2];
    float prevTimer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interval = (60 / BPM) * 2;
        //offset = interval / 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (overseer.activeAudioSource == null) { return; }
        timer = ((overseer.activeAudioSource.time + offset) % interval);
        if (timer < prevTimer)//if the timer went down, that means the beat passed and reset
        {
            beats += 1;
        }

        if (Pointer.current.press.wasPressedThisFrame)
        {
            tempTime = Mathf.Abs(timer % interval);//detecting close to the end
            earlyTime = Mathf.Abs((interval - timer) % interval);//detecting at the beginning
            if (tempTime < successThreshold || earlyTime < successThreshold)
            {
                SuccessfulClick();
            }
            else
            {
                //Debug.Log("Missed click" + tempTime.ToString());
            }
        }


        slider.maxValue = interval;
        slider.minValue = 0;
        if (beats % 2 == 0)
        {
            slider.value = timer;
        }
        else
        {
            slider.value = interval - timer;
        }

        //slider.minValue = -interval;

        prevTimer = timer;
    }

    public void SuccessfulClick()
    {
        //Debug.Log("Success!" + tempTime.ToString());
        //if (GM.craftingStations[0].activeCraftable != -1)
        //{
            GM.craftingStations[0].bonusRate = Mathf.Min(GM.craftingStations[0].bonusRate + GM.craftingStations[0].bonusRateIncrement, GM.craftingStations[0].maxBonusRate);
        //}

        if(slider.value < slider.maxValue * 0.5f)
        {
            successMarkers[0].gameObject.SetActive(true);
        }
        else
        {
            successMarkers[1].gameObject.SetActive(true);
        }
    }

    public void ToggleMetronome()
    {
        autoBonusText.gameObject.SetActive(!toggle.isOn);
    }
}
