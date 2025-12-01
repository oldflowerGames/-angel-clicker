using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleFloater : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    public List<GameObject> teleportLocations = new List<GameObject>();
    public int currentPosition;

    public float teleportInterval = 10f;
    public float teleportTimer = 0;

    public GameObject teleportSprite1, teleportSprite2;

    // Update is called once per frame
    void Update()
    {
        teleportTimer += Time.deltaTime;
        if(teleportTimer > teleportInterval)
        {
            teleportTimer = 0;
            StartCoroutine(TeleportAnimation());
        }
    }

    public IEnumerator TeleportAnimation()
    {
        //if(teleportLocations.Count <= 1) { return; }
        Vector3 goalPos = Vector3.zero;
        int goalIndex = 0;
        while(goalIndex == currentPosition)
        {
            goalIndex = Random.Range(0, teleportLocations.Count - 1);
        }

        teleportSprite1.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        teleportSprite2.gameObject.SetActive(true);
        teleportSprite1.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.25f);

        gameObject.transform.position = teleportLocations[goalIndex].transform.position;
        currentPosition = goalIndex;

        yield return new WaitForSeconds(0.25f);
        teleportSprite2.gameObject.SetActive(false);
        teleportSprite1.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.25f);
        teleportSprite1.gameObject.SetActive(false);

    }
}
