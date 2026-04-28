using UnityEngine;

public class KeyPrompt3D : MonoBehaviour
{
    [Header("Tecla")]
    public KeySymbolType keySymbol;

    [Header("Renderer")]
    public Renderer targetRenderer;

    private Transform missPoint;
    private float moveSpeed;
    private KeyboardMinigameController controller;
    private bool resolved = false;

    public void Init(
        KeySymbolType symbol,
        Material material,
        Transform targetMissPoint,
        float speed,
        KeyboardMinigameController owner
    )
    {
        keySymbol = symbol;
        missPoint = targetMissPoint;
        moveSpeed = speed;
        controller = owner;
        resolved = false;

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null && material != null)
            targetRenderer.material = material;
    }

    void Update()
    {
        if (resolved) return;
        if (missPoint == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            missPoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, missPoint.position) <= 0.05f)
        {
            resolved = true;

            if (controller != null)
                controller.OnPromptMissed(this);

            Destroy(gameObject);
        }
    }

    public void Resolve()
    {
        if (resolved) return;

        resolved = true;
        Destroy(gameObject);
    }
}