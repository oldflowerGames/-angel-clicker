using UnityEngine;
using UnityEngine.UI;

public class BuildingBarOption : MonoBehaviour
{
    public BuildingBar bar;
    public Button button;
    public GameObject notificationIcon;
    public int buildingIndex = 0;
    GameObject target;
    Vector3 targetPos;

    public void PopulateOption(int indexPass)
    {
        buildingIndex = indexPass;
        button.image.sprite = bar.GM.DB.buildingBarSprites[buildingIndex];
        notificationIcon.SetActive(false);
    }

    public void JumpToThis()
    {
        target = bar.GM.buildings[buildingIndex].gameObject;
        targetPos = new Vector3(target.transform.position.x, target.transform.position.y, bar.GM.mainCam.transform.position.z);
        targetPos.x = Mathf.Clamp(targetPos.x, bar.GM.mainCam.minX, bar.GM.mainCam.maxX);
        targetPos.y = Mathf.Clamp(targetPos.y, bar.GM.mainCam.minY, bar.GM.mainCam.maxY);
        bar.GM.mainCam.transform.position = targetPos;
    }
}
