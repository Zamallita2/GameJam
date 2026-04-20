using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float gravity = -9.81f;

    [Header("Mouse")]
    public float mouseSensitivity = 200f;
    public Transform cameraPivot;

    private float xRotation = 0f;

    private CharacterController controller;
    private Vector3 velocity;

    [Header("Animator")]
    public Animator armsAnimator;

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
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

        Vector3 move = transform.right * h + transform.forward * v;

        controller.Move(move * speed * Time.deltaTime);

        // gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 🔥 CALCULAR VELOCIDAD PARA ANIMACIONES
        float inputMagnitude = new Vector2(h, v).magnitude;

        float animSpeed = 0f;

        if (inputMagnitude > 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                animSpeed = 1f;      // correr
            else
                animSpeed = 0.5f;   // caminar
        }
        else
        {
            animSpeed = 0f;         // idle
        }

        // 🔥 ENVIAR AL ANIMATOR
        if (armsAnimator != null)
        {
            armsAnimator.SetFloat("Speed", animSpeed);
        }
    }
}