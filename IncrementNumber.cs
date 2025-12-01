using UnityEngine;
using TMPro;
using System.Globalization;

public class IncrementNumber : MonoBehaviour
{
    public float score;

    public float increment = 1;
    public float rate = 1;
    public float accel = 1;

    public float baseIncrement = 1;
    public float baseRate = 1;
    public float baseAccel = 1;
    public float baseScore = 0;

    public float scoreInterval = 0.1f;
    public float scoreTimer;

    public TextMeshProUGUI scoreText;
    int tempScore;


    public void ResetValues()
    {
        increment = baseIncrement;
        rate = baseRate;
        accel = baseAccel;
        score = baseScore;
    }

    public void OnEnable()
    {
        SetText();
    }

    // Update is called once per frame
    void Update()
    {
        scoreTimer += Time.deltaTime;
        rate += accel * Time.deltaTime;
        if (scoreTimer > scoreInterval)
        {
            scoreTimer = 0;
            score += increment * rate;
            SetText();
        }
    }

    public void SetText()
    {
        if (scoreText.gameObject.activeInHierarchy)//no need to set this if it isn't visible
        {
            tempScore = Mathf.RoundToInt(score);
            scoreText.text = ToKMB((decimal)tempScore);
        }
    }

    public void SetScore(int pass)
    {
        score = pass;
        SetText();
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
}
