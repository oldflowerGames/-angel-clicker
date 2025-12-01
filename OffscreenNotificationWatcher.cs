using UnityEngine;

public class OffscreenNotificationWatcher : MonoBehaviour
{
    public GameObject[] notifications = new GameObject[6];
    public Camera cam;
    public GameObject leftNotification, rightNotification;
    Vector3 worldToScreen;
    bool enableLeft, enableRight;
    public float updateInterval = 0.3f;
    public float updateTimer = 0;


    // Update is called once per frame
    void Update()
    {
        updateTimer += Time.deltaTime;
        if(updateTimer > updateInterval)//don't need to do this every frame
        {
            CheckNotifications();
            updateTimer = 0;
        }
    }
    
    public void CheckNotifications()
    {
        enableLeft = enableRight = false;
        for (int ii = 0; ii < notifications.Length; ii++)
        {
            if (notifications[ii].gameObject.activeInHierarchy)
            {
                worldToScreen = cam.WorldToScreenPoint(notifications[ii].transform.position);
                if (worldToScreen.x <= 0)
                {
                    enableLeft = true;
                }
                else if (worldToScreen.x >= Screen.width)
                {
                    enableRight = true;
                }
            }
        }

        leftNotification.SetActive(enableLeft);
        rightNotification.SetActive(enableRight);
    }
}
