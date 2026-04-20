using UnityEngine;

public class ArmsController : MonoBehaviour
{
    public Animator animator;

    void Start()
    {
        Invoke("GoIdle", 1.5f); // ajusta al tiempo del intro
    }

    void GoIdle()
    {
        animator.SetTrigger("ToIdle");
    }
}