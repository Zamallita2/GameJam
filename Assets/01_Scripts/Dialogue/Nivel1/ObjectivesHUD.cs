using TMPro;
using UnityEngine;

public class ObjectivesHUD : MonoBehaviour
{
    public static ObjectivesHUD Instance;

    [Header("Textos")]
    public TextMeshProUGUI botonesText;
    public TextMeshProUGUI cablesText;
    public TextMeshProUGUI puzzleText;
    public TextMeshProUGUI gearText;
    public TextMeshProUGUI arrowsText;

    [Header("Colores")]
    public Color pendingColor = Color.white;
    public Color completedColor = Color.green;

    void Awake()
    {
        Instance = this;

        SetPending(botonesText, "Reparar módulo de memoria");
        SetPending(cablesText, "Reconectar cables de energía");
        SetPending(puzzleText, "Reconstruir identificación visual");
        SetPending(gearText, "Acomodar engranajes del núcleo");
        SetPending(arrowsText, "Restaurar respuesta rápida");
    }

    void SetPending(TextMeshProUGUI text, string label)
    {
        if (text == null) return;
        text.text = "□ " + label;
        text.color = pendingColor;
    }

    void SetCompleted(TextMeshProUGUI text, string label)
    {
        if (text == null) return;
        text.text = "✓ " + label;
        text.color = completedColor;
    }

    public void CompleteBotones()
    {
        SetCompleted(botonesText, "Reparar módulo de memoria");
    }

    public void CompleteCables()
    {
        SetCompleted(cablesText, "Reconectar cables de energía");
    }

    public void CompletePuzzle()
    {
        SetCompleted(puzzleText, "Reconstruir identificación visual");
    }

    public void CompleteGear()
    {
        SetCompleted(gearText, "Acomodar engranajes del núcleo");
    }

    public void CompleteArrows()
    {
        SetCompleted(arrowsText, "Restaurar respuesta rápida");
    }
}