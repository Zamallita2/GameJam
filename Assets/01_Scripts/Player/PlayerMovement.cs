using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    public CharacterController controller;
    public Animator animator;

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        controller.Move(move * currentSpeed * Time.deltaTime);

        // 🔥 ESTA ES LA PARTE IMPORTANTE
        float movementAmount = move.magnitude;

        if (isRunning && movementAmount > 0.1f)
            animator.SetFloat("Speed", 1f);
        else if (movementAmount > 0.1f)
            animator.SetFloat("Speed", 0.5f);
        else
            animator.SetFloat("Speed", 0f);
    }
}