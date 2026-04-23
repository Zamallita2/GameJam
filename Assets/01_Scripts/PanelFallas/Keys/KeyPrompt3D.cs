using UnityEngine;

public class KeyPrompt3D : MonoBehaviour
{
    public KeyButtonController.KeySymbol keySymbol;
    public Renderer targetRenderer;

    private Transform missPoint;
    private float moveSpeed;
    private KeyboardMinigameController controller;
    private bool wasResolved = false;

    public void Init(
        KeyButtonController.KeySymbol newSymbol,
        Material visualMaterial,
        Transform newMissPoint,
        float newMoveSpeed,
        KeyboardMinigameController newController)
    {
        keySymbol = newSymbol;
        missPoint = newMissPoint;
        moveSpeed = newMoveSpeed;
        controller = newController;

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null && visualMaterial != null)
            targetRenderer.material = visualMaterial;
    }

    void Update()
    {
        if (wasResolved) return;
        if (missPoint == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            missPoint.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, missPoint.position) <= 0.06f)
        {
            if (controller != null)
                controller.OnPromptMissed(this);

            Destroy(gameObject);
        }
    }

    public void MarkResolved()
    {
        wasResolved = true;
    }
}