using UnityEngine;

public class BuildingBar : MonoBehaviour
{
    public GameManager GM;
    public BuildingBarOption[] options = new BuildingBarOption[8];

    public void OnEnable()
    {
        //debug
        PopulateBar();
    }

    public void PopulateBar()
    {
        foreach (BuildingBarOption b in options) { b.gameObject.SetActive(false); }
        for(int ii = 0; ii < Mathf.Min(GM.buildings.Length, options.Length); ii++) 
        {
            if (GM.buildings[ii].constructed && GM.buildings[ii].gameObject.activeInHierarchy && GM.buildings[ii].ignoreBar == false)
            {
                options[ii].gameObject.SetActive(true);
                options[ii].PopulateOption((int)GM.buildings[ii].buildingID);
            }
            else
            {
                options[ii].gameObject.SetActive(false);
            }
        }
    }
}
