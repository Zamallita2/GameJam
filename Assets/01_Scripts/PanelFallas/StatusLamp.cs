using System.Collections;
using UnityEngine;

public class StatusLamp : MonoBehaviour
{
    [Header("Renderer")]
    public Renderer lampRenderer;

    [Header("Materiales opcionales")]
    public Material neutralMaterial;
    public Material correctMaterial;
    public Material wrongMaterial;

    [Header("Emisión")]
    public Color neutralEmission = Color.white;
    public Color correctEmission = Color.green;
    public Color wrongEmission = Color.red;
    public float intensity = 2f;

    void Awake()
    {
        if (lampRenderer == null)
            lampRenderer = GetComponentInChildren<Renderer>();
    }

    void Start()
    {
        SetNeutral();
    }

    public void SetNeutral()
    {
        ApplyState(neutralMaterial, neutralEmission);
    }

    public void SetCorrect()
    {
        ApplyState(correctMaterial, correctEmission);
    }

    public void SetWrong()
    {
        ApplyState(wrongMaterial, wrongEmission);
    }

    void ApplyState(Material mat, Color emission)
    {
        if (lampRenderer == null) return;

        if (mat != null)
            lampRenderer.material = mat;

        lampRenderer.material.EnableKeyword("_EMISSION");
        lampRenderer.material.SetColor("_EmissionColor", emission * intensity);
    }

    public IEnumerator FlashWrong(float duration)
    {
        SetWrong();
        yield return new WaitForSeconds(duration);
        SetNeutral();
    }

    public IEnumerator FlashCorrect(float duration)
    {
        SetCorrect();
        yield return new WaitForSeconds(duration);
        SetNeutral();
    }
}