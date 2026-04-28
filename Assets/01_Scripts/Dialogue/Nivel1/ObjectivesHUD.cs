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
    public TextMeshProUGUI keysText;
    public TextMeshProUGUI palancasText;

    [Header("Colores")]
    public Color pendingColor = Color.white;
    public Color completedColor = Color.green;

    void Awake()
    {
        Instance = this;

        SetPending(botonesText, "Reparar módulo de memoria");
        SetPending(cablesText, "Reconectar cables");
        SetPending(puzzleText, "Reconstruir imagen");
        SetPending(gearText, "Sincronizar engranajes");
        SetPending(arrowsText, "Respuesta rápida");
        SetPending(keysText, "Panel de teclas");
        SetPending(palancasText, "Código de palancas");
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

    public void CompleteBotones() => SetCompleted(botonesText, "Reparar módulo de memoria");
    public void CompleteCables() => SetCompleted(cablesText, "Reconectar cables");
    public void CompletePuzzle() => SetCompleted(puzzleText, "Reconstruir imagen");
    public void CompleteGear() => SetCompleted(gearText, "Sincronizar engranajes");
    public void CompleteArrows() => SetCompleted(arrowsText, "Respuesta rápida");
    public void CompleteTeclas() => SetCompleted(keysText, "Panel de teclas");
    public void CompletePalancas() => SetCompleted(palancasText, "Código de palancas");
}