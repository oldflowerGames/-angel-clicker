using UnityEngine;
using TMPro;

public class CurrencyWatcher : MonoBehaviour
{
    public GameManager GM;
    public TextMeshProUGUI[] texts = new TextMeshProUGUI[5];
    public float interval = 0.1f;
    public bool bonusCurrencies = false;
    public bool soulCurrency = false;
    int tempIndex;
    //public float intervalTracker = 0;

    private void OnEnable()
    {
        InvokeRepeating("UpdateCurrencies", interval, interval);
    }

    //// Update is called once per frame
    //void LateUpdate()
    //{
    //    intervalTracker += Time.deltaTime;
    //    if(intervalTracker > interval)
    //    {
    //        intervalTracker = 0;

    //    }
    //}

    public void UpdateCurrencies()
    {
        for (int ii = 0; ii < texts.Length; ii++)
        {
            tempIndex = ii;
            if (bonusCurrencies) { tempIndex += 5; }
            if (soulCurrency) { tempIndex = 3; }
            if (GM.currencies[tempIndex] <= 0)
            {
                texts[ii].gameObject.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                texts[ii].gameObject.transform.parent.gameObject.SetActive(true);
                if (soulCurrency)
                {
                    texts[ii].text = GM.ToKMB((decimal)GM.currencies[tempIndex]);
                }
                else
                {
                    texts[ii].text = GM.currencies[tempIndex].ToString("N0");
                }
            }
        }
    }
}
