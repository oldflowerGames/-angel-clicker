using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Collectible c = collision.GetComponent<Collectible>();
        if(c != null)
        {
            c.Gather();
        }
    }
}
