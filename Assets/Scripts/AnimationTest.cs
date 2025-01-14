using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetFloat("WalkingSpeed", 1);
        }
        else
        {
            animator.SetFloat("WalkingSpeed", 0);
        }
    }
}
