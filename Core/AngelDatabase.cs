using System.Collections.Generic;
using UnityEngine;

public class AngelDatabase : MonoBehaviour
{
    public List<Sprite> idleSprites = new List<Sprite>();
    public List<Sprite> walkingSprites = new List<Sprite>();
    public List<Sprite> gatherWoodSprites = new List<Sprite>();
    public List<Sprite> gatherRockSprites = new List<Sprite>();
    public List<Sprite> carryingSprites = new List<Sprite>();
    public List<Sprite> fallingSprites = new List<Sprite>();
    public List<Sprite> gentleFallingSprites = new List<Sprite>();
    public List<Sprite> constructingSprites = new List<Sprite>();
    public List<Sprite> unconsciousSprites = new List<Sprite>();
    public List<Sprite> buildingBarSprites = new List<Sprite>();
    public List<Sprite> martSprites = new List<Sprite>();
    public List<Sprite> studyIdleSprites = new List<Sprite>();
    public List<Sprite> studyActiveSprites = new List<Sprite>();

    public GameObject[] collectiblePrefabs = new GameObject[5];
    public PrestigeData[] prestigeDatas = new PrestigeData[20];
}
