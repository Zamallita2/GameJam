using UnityEngine;

public class CableNode : MonoBehaviour
{
    public int id;
    public bool isOrigin;
    public bool isConnected = false;

    [Header("Visual")]
    public Renderer targetRenderer; // arrastras el hijo aquí

    private CablesPanel panel;

    void Start()
    {
        panel = FindFirstObjectByType<CablesPanel>();
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
            // 🔥 IMPORTANTE: esto crea instancia única del material
            targetRenderer.material = new Material(targetRenderer.material);
            targetRenderer.material.color = color;
        }
    }
}
