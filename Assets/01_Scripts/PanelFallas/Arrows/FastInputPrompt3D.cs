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

    private Transform hitPoint;
    private Transform missPoint;
    private float moveSpeed;
    private FastInputPanelController controller;
    private bool wasResolved = false;

    public void Init(
        FastInputButton3D.ColorSymbol newSymbol,
        PromptType newType,
        Material visualMaterial,
        Transform newHitPoint,
        Transform newMissPoint,
        float newMoveSpeed,
        FastInputPanelController newController)
    {
        symbol = newSymbol;
        promptType = newType;
        hitPoint = newHitPoint;
        missPoint = newMissPoint;
        moveSpeed = newMoveSpeed;
        controller = newController;

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (targetRenderer != null && visualMaterial != null)
            targetRenderer.material = visualMaterial;

        // SOLO las flechas rotan aleatoriamente
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

        if (Vector3.Distance(transform.position, missPoint.position) < 0.03f)
        {
            if (controller != null)
                controller.OnPromptMissed(this);

            Destroy(gameObject);
        }
    }

    public bool IsInsideHitWindow(float hitDistance)
    {
        if (hitPoint == null) return false;
        return Vector3.Distance(transform.position, hitPoint.position) <= hitDistance;
    }

    public void MarkResolved()
    {
        wasResolved = true;
    }
}