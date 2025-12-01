using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrestigeSlider : MonoBehaviour
{
    public GameManager GM;
    public Slider slider;
    public TextMeshProUGUI pointsAvailableText;

    // Update is called once per frame
    void Update()
    {
        slider.value = GM.currencies[3];
        slider.maxValue = GM.prestige.prestigeExpToLevel[GM.prestige.prestigeLevel];
        pointsAvailableText.text = GM.prestige.GetAvailablePoints().ToString();
    }
}
