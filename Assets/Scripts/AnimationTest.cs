using UnityEngine;

public class AnimationTest : MonoBehaviour
{
    public Animator animator;
    private string blendParameter = "Blend";

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
        else
        {
            animator.SetFloat(blendParameter, 0);
        }
    }
}
