using UnityEngine;

public class Soul : MonoBehaviour
{
    public GameManager GM;
    public float progress;
    public float moveRate = 0.1f;
    public float minMove = 0.8f;
    public float maxMove = 1.2f;
    public AnimationCurve curve;
    Vector3 startPos, endPos;

    public void OnEnable()
    {
        ActivateSoul();
    }

    public void ActivateSoul()
    {
        transform.position = new Vector3(Random.Range(-Global.mapWidth * 0.5f, Global.mapWidth * 0.5f), Global.soulStartPoint, -1);
        startPos = transform.position;
        endPos = GM.buildings[5].transform.position;
        endPos.z -= 1;
        moveRate = Random.Range(minMove, maxMove);
        progress = 0;
    }    

    // Update is called once per frame
    void Update()
    {
        progress += Time.deltaTime * moveRate;
        transform.position = Vector3.Lerp(startPos, endPos, curve.Evaluate(progress));
        if(progress >= 1)
        {
            GM.AddCurrency(3, false);
            gameObject.SetActive(false);
        }
    }
}
