using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float acceleration = 10f;
    public float gravity = -9.81f;

    [Header("Mouse")]
    public float mouseSensitivity = 200f;
    public Transform cameraPivot;

    [Header("Animator")]
    public Animator armsAnimator;

    private float xRotation = 0f;

    private CharacterController controller;
    private Vector3 velocity;
    private Vector3 currentMove;
    private bool isRunning;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
        HandleAnimation();
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        isRunning = Input.GetKey(KeyCode.LeftShift);

        float targetSpeed = isRunning ? runSpeed : walkSpeed;

        Vector3 inputDir = (transform.right * h + transform.forward * v).normalized;
        Vector3 targetMove = inputDir * targetSpeed;

        currentMove = Vector3.Lerp(currentMove, targetMove, acceleration * Time.deltaTime);

        controller.Move(currentMove * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleAnimation()
    {
        if (armsAnimator == null) return;

        Vector3 horizontalMove = new Vector3(currentMove.x, 0f, currentMove.z);
        float moveAmount = horizontalMove.magnitude;

        if (isRunning && moveAmount > 0.15f)
            armsAnimator.SetFloat("Speed", 1f);
        else if (moveAmount > 0.05f)
            armsAnimator.SetFloat("Speed", 0.5f);
        else
            armsAnimator.SetFloat("Speed", 0f);
    }

    public float GetPitch()
    {
        return xRotation;
    }

    public void SetPitch(float pitch)
    {
        xRotation = pitch;
        cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    public void ResetVelocity()
    {
        velocity = Vector3.zero;
    }

    public void SnapToGround()
    {
        RaycastHit hit;

        // lanzamos ray hacia abajo
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 5f))
        {
            // colocamos al player justo sobre el suelo
            transform.position = hit.point + Vector3.up * 0.1f;
        }

        // resetear gravedad
        velocity = Vector3.zero;
    }
}