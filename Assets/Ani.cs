using UnityEngine;

public class Ani : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            animator.SetTrigger("walking");
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            animator.SetTrigger("attack1");
        }

        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            animator.SetTrigger("jumping");
        }

        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            animator.SetTrigger("crouch");
        }

        if (Input.GetKeyDown(KeyCode.Keypad5))
        {
            animator.SetTrigger("hit");
        }

        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            animator.SetTrigger("attack2");
        }

        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            animator.SetTrigger("block");
        }

        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            animator.SetTrigger("jump_attack");
        }

        // J를 누르고 있는 동안
        animator.SetBool("naruto_run", Input.GetKey(KeyCode.J));

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("lose");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            animator.SetTrigger("win");
        }
    }
}
