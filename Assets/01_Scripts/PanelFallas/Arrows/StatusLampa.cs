using UnityEngine;

public class StatusLampa : MonoBehaviour
{
    public Renderer lampRenderer;
    public Material neutralMaterial;
    public Material correctMaterial;
    public Material wrongMaterial;

    void Awake()
    {
        if (lampRenderer == null)
            lampRenderer = GetComponentInChildren<Renderer>();
    }

    public void SetNeutral()
    {
        if (lampRenderer != null && neutralMaterial != null)
            lampRenderer.material = neutralMaterial;
    }

    public void SetCorrect()
    {
        if (lampRenderer != null && correctMaterial != null)
            lampRenderer.material = correctMaterial;
    }

    public void SetWrong()
    {
        if (lampRenderer != null && wrongMaterial != null)
            lampRenderer.material = wrongMaterial;
    }
}