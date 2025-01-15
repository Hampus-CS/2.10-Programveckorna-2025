using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    public Animator animator;
    private string blendParameter = "Friendly Troop1";

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            animator.SetFloat(blendParameter, 1);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            animator.SetFloat(blendParameter, 2);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            animator.SetFloat(blendParameter, 3);
        }
        else
        {
            animator.SetFloat(blendParameter, 0);
        }


    }
}
