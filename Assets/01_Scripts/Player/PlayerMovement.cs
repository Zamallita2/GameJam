using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Estado")]
    public bool canMove = true;

    [Header("Velocidad")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public float acceleration = 12f;

    [Header("Referencias")]
    public Rigidbody rb;
    public Transform cameraTransform;

    [Header("Animadores")]
    public Animator thirdPersonAnimator;
    public Animator armsAnimator;

    private Vector3 moveDirection;
    private bool isRunning;
    private Animator currentAnimator;

    public bool isFirstPerson = false;

    [Header("Audio Sources")]
    public AudioSource source;

    [Header("Clips")]
    public AudioClip Walk;
    public AudioClip Run;
    private bool isWalk=false;
    private bool isRun=false;

    [Header("Volúmenes")]
    [Range(0f, 1f)] public float volWalk = 0.5f;
    [Range(0f, 1f)] public float volRun = 0.5f;

    void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        currentAnimator = isFirstPerson ? armsAnimator : thirdPersonAnimator;
        if (source == null)
            source = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        currentAnimator = isFirstPerson ? armsAnimator : thirdPersonAnimator;
        source.loop = true;
    }

    void Update()
    {
        if (!canMove)
        {
            moveDirection = Vector3.zero;
            isRunning = false;
            HandleAnimation();
            return;
        }

        HandleInput();
        HandleAnimation();
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            StopPlayerCompletely();
            return;
        }

        MovePlayer();
        HandleRotation();
        StopResidualSpin();
    }

    public void SetCanMove(bool value)
    {
        canMove = value;

        moveDirection = Vector3.zero;
        isRunning = false;

        if (currentAnimator == null)
            currentAnimator = isFirstPerson ? armsAnimator : thirdPersonAnimator;

        if (currentAnimator != null)
            currentAnimator.SetFloat("Speed", 0f);

        StopPlayerCompletely();

        Debug.Log("[PlayerMovement] CanMove = " + canMove);
    }

    void StopPlayerCompletely()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        rb.angularVelocity = Vector3.zero;
    }

    void HandleInput()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("[PlayerMovement] Falta cameraTransform");
            return;
        }

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
        if (rb == null) return;

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
        if (rb == null) return;

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
        if (rb == null) return;

        rb.angularVelocity = Vector3.zero;

        if (moveDirection.sqrMagnitude < 0.001f)
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }
    }

    void HandleAnimation()
    {
        if (currentAnimator == null || rb == null) return;

        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float moveAmount = horizontalVelocity.magnitude;

        if (isRunning && moveAmount > 0.15f)
        {
            currentAnimator.SetFloat("Speed", 1f);
            if (!isRun)
            {
                CambiarStep(Run, volRun,true);
            }
        }
        else if (moveAmount > 0.05f)
        {
            currentAnimator.SetFloat("Speed", 0.5f);
            if (!isWalk)
            {
                CambiarStep(Walk, volWalk,false);
            }
        }
        else
        {
            currentAnimator.SetFloat("Speed", 0f);
            source.Stop();
            isRun=false;
            isWalk=false;
        }
    }

    public void SetFirstPerson(bool value)
    {
        isFirstPerson = value;
        currentAnimator = isFirstPerson ? armsAnimator : thirdPersonAnimator;
    }
    void CambiarStep(AudioClip clip, float vol, bool si)
    {
        if (si) 
        {
            isRun=true;
            isWalk=false;
        }
        else
        {
            isRun=false;
            isWalk=true;
        }
        source.Stop();
        source.clip = clip;
        source.volume = vol;
        source.loop = true;
        source.Play();
    }
}