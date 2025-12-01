using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Collectible : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
{
    public GameManager GM;
    public int index = 0;
    bool buttonHeld = false;
    public bool instantiated = false;
    public Rigidbody2D rb;
    public float detachDistance = 3;


    bool hasTarget = false;
    Vector2 targetDirection;
    GameObject targetObj;
    public float moveSpeed = 1f;

    int gatherValue = 0;

    public void OnEnable()
    {
        hasTarget = false;
    }

    private void FixedUpdate()
    {
        if (!hasTarget) { rb.linearVelocity = Vector2.zero; return; }
        if (targetObj == null) { rb.linearVelocity = Vector2.zero; hasTarget = false; return; }
        if (targetObj.activeInHierarchy == false) { rb.linearVelocity = Vector2.zero; hasTarget = false; return; }
        if(Vector2.Distance(transform.position, targetObj.transform.position) > detachDistance) { rb.linearVelocity = Vector3.zero; hasTarget = false; return; }

        targetDirection = (targetObj.transform.position - transform.position).normalized;
        rb.linearVelocity = targetDirection * moveSpeed;
    }

    public void SetTarget(GameObject objPass)
    {
        targetObj = objPass;
        hasTarget = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right) { return; }//no right clicks
        
        Gather();
        //StartCoroutine(HoldDelay());
        //GM.sfx.PlaySound(GM.sfx.tactile);
    }

    public void Gather()
    {
        gatherValue = GM.AddCurrency(index, true);

        GM.CreateGatherNumber(gameObject, gatherValue);

        GM.sfx.PlaySound(GM.sfx.collectTactile);

        //if (instantiated)
        //{
        //    Destroy(gameObject);
        //}
        //else
        //{
            gameObject.SetActive(false);
        //}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //if (eventData.button == PointerEventData.InputButton.Right) { return; }//no right clicks

        //buttonHeld = false;
        //StopAllCoroutines();

        ////reset visuals 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //enable hover visuals
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //disable hover visuals
    }

}
