using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Velocidad")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public float acceleration = 12f;

    [Header("Referencias")]
    public Rigidbody rb;
    public Transform cameraTransform;

    private Vector3 moveDirection;
    private bool isRunning;
    public Animator thirdPersonAnimator;
    public Animator armsAnimator;

    private Animator currentAnimator;
    public bool isFirstPerson = false;
    void Start()
    {   
        currentAnimator = thirdPersonAnimator;
    }

    void Update()
    {
        HandleInput();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        MovePlayer();
        HandleRotation();
        StopResidualSpin();
    }

    void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        isRunning = Input.GetKey(KeyCode.LeftShift);

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * v + camRight * h).normalized;
    }

    void MovePlayer()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 targetVelocity = moveDirection * currentSpeed;

        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        horizontalVelocity = Vector3.Lerp(
            horizontalVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(
            horizontalVelocity.x,
            rb.linearVelocity.y,
            horizontalVelocity.z
        );
    }

    void HandleRotation()
    {
        // Gira SOLO por input real
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                )
            );
        }
    }

    void StopResidualSpin()
    {
        // Mata cualquier giro residual físico
        rb.angularVelocity = Vector3.zero;

        // Si no hay input, también mata micro movimiento horizontal
        if (moveDirection.sqrMagnitude < 0.001f)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    void HandleAnimation()
    {
        if (currentAnimator == null) return;

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float moveAmount = horizontalVelocity.magnitude;

        if (isRunning && moveAmount > 0.15f)
            currentAnimator.SetFloat("Speed", 1f);
        else if (moveAmount > 0.05f)
            currentAnimator.SetFloat("Speed", 0.5f);
        else
            currentAnimator.SetFloat("Speed", 0f);
    }
    public void SetFirstPerson(bool value)
    {
        isFirstPerson = value;

        if (isFirstPerson)
            currentAnimator = armsAnimator;
        else
            currentAnimator = thirdPersonAnimator;
    }
}