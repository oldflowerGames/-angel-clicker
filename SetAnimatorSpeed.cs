using UnityEngine;

public class SetAnimatorSpeed : MonoBehaviour
{
    public Animator animator;
    public float startSpeed;
    public bool randomSpeed;
    public float minSpeed, maxSpeed;
    // Start is called before the first frame update
    void OnEnable()
    {
        if (animator != null)
        {
            if (randomSpeed)
            {
                animator.speed = Random.Range(minSpeed, maxSpeed);
            }
            else
            {
                animator.speed = startSpeed;
            }
        }
    }
}
