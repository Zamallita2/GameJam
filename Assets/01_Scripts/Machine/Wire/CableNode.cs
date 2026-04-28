using UnityEngine;

public class CableNode : MonoBehaviour
{
    public int id;
    public bool isOrigin;
    public bool isConnected = false;

    [Header("Visual")]
    public Renderer targetRenderer;

    private CablesPanel panel;

    void Awake()
    {
        panel = GetComponentInParent<CablesPanel>();
    }

    void OnMouseDown()
    {
        if (panel != null)
            panel.OnNodeClicked(this);
    }

    public void SetColor(Color color)
    {
        if (targetRenderer != null)
        {
            targetRenderer.material = new Material(targetRenderer.material);
            targetRenderer.material.color = color;
        }
    }
}