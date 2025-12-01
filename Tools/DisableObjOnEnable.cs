using System.Collections.Generic;
using UnityEngine;

public class DisableObjOnEnable : MonoBehaviour
{
    public List<GameObject> objs = new List<GameObject>();

    private void OnEnable()
    {
        foreach (GameObject obj in objs) { obj.SetActive(false); }
    }
}
