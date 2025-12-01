using UnityEngine;

public class DisableOnDelay : MonoBehaviour
{
    public float delay = 1f;
    public bool destroy = false;
    float timer = 0;

    private void OnEnable()
    {
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > delay)
        {
            if (destroy)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
