using UnityEngine;

public class FastInputPrompt3D : MonoBehaviour
{
    public enum PromptType
    {
        Arrow,
        Circle
    }

    public FastInputButton3D.ColorSymbol symbol;
    public PromptType promptType;
    public Renderer targetRenderer;

    private Transform missPoint;
    private float moveSpeed;
    private FastInputPanelController controller;
    private bool wasResolved = false;

    public void Init(
        FastInputButton3D.ColorSymbol newSymbol,
        PromptType newType,
        Material visualMaterial,
        Transform newMissPoint,
        float newMoveSpeed,
        FastInputPanelController newController)
    {
        symbol = newSymbol;
        promptType = newType;
        missPoint = newMissPoint;
        moveSpeed = newMoveSpeed;
        controller = newController;

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null && visualMaterial != null)
            targetRenderer.material = visualMaterial;

        // Solo flechas rotan aleatoriamente
        if (promptType == PromptType.Arrow)
        {
            float angle = Random.Range(0, 4) * 90f;
            transform.Rotate(Vector3.forward, angle);
        }
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

        if (Vector3.Distance(transform.position, missPoint.position) <= 0.08f)
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