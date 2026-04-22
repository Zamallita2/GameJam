using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;

    [Header("Mouse")]
    public float mouseSensitivity = 120f;
    public float minPitch = -20f;
    public float maxPitch = 45f;

    [Header("Tercera Persona")]
    public Vector3 thirdPersonOffset = new Vector3(0f, 0.05f, -1.6f);
    public float thirdPersonSmooth = 12f;

    [Header("Primera Persona")]
    public Vector3 firstPersonOffset = new Vector3(0f, 0.02f, 0.03f);
    public float firstPersonSmooth = 16f;

    [Header("Control")]
    public KeyCode switchViewKey = KeyCode.V;
    public bool startInThirdPerson = true;

    [Header("Colisión Cámara")]
    public float cameraCollisionRadius = 0.15f;
    public float minDistance = 0.25f;
    public LayerMask collisionMask;

    private float yaw;
    private float pitch;
    private bool isThirdPerson;

    void Start()
    {
        isThirdPerson = startInThirdPerson;
        yaw = target.eulerAngles.y;
        pitch = 8f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SnapCameraToCurrentMode();
    }

    void LateUpdate()
    {
        HandleMouseLook();
        HandleViewSwitch();
        UpdateCameraPosition();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
    }

    void HandleViewSwitch()
    {
        if (Input.GetKeyDown(switchViewKey))
        {
            isThirdPerson = !isThirdPerson;
            SnapCameraToCurrentMode();
        }
    }

    void SnapCameraToCurrentMode()
    {
        transform.position = GetDesiredPosition();
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void UpdateCameraPosition()
    {
        Vector3 desiredPosition = GetDesiredPosition();
        float smooth = isThirdPerson ? thirdPersonSmooth : firstPersonSmooth;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            Time.deltaTime * smooth
        );

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    Vector3 GetDesiredPosition()
    {
        Quaternion yawRotation = Quaternion.Euler(0f, yaw, 0f);

        if (isThirdPerson)
        {
            Vector3 desiredPosition = target.position + yawRotation * thirdPersonOffset;
            return ResolveCollision(desiredPosition);
        }
        else
        {
            Quaternion fullRotation = Quaternion.Euler(pitch, yaw, 0f);
            return target.position + fullRotation * firstPersonOffset;
        }
    }

    Vector3 ResolveCollision(Vector3 desiredPosition)
    {
        Vector3 direction = desiredPosition - target.position;
        float distance = direction.magnitude;

        if (distance <= 0.001f)
            return desiredPosition;

        if (Physics.SphereCast(
            target.position,
            cameraCollisionRadius,
            direction.normalized,
            out RaycastHit hit,
            distance,
            collisionMask))
        {
            return target.position + direction.normalized * Mathf.Max(hit.distance - 0.08f, minDistance);
        }

        return desiredPosition;
    }

    public bool IsThirdPerson()
    {
        return isThirdPerson;
    }
}