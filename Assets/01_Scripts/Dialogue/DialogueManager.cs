using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public Canvas dialogueCanvas;
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    void Awake()
    {
        Instance = this;

        if (dialogueCanvas == null)
            dialogueCanvas = GetComponentInParent<Canvas>();

        if (dialogueCanvas != null)
        {
            dialogueCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            dialogueCanvas.sortingOrder = 999;
            dialogueCanvas.gameObject.SetActive(true);
        }

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";

        Debug.Log("[DialogueManager] Usa texto: " + (dialogueText != null ? dialogueText.name : "NULL"));
    }

    public void ShowMessage(string message, AudioClip voiceClip = null)
    {
        if (dialoguePanel == null || dialogueText == null)
        {
            Debug.LogError("[DialogueManager] Faltan referencias");
            return;
        }

        dialoguePanel.SetActive(true);
        dialogueText.gameObject.SetActive(true);

        dialogueText.text = message;
        dialogueText.color = Color.white;
        dialogueText.fontSize = 28;

        dialogueText.textWrappingMode = TextWrappingModes.Normal;

        dialogueText.overflowMode = TextOverflowModes.Overflow;
        dialogueText.ForceMeshUpdate();

        Debug.Log("[DialogueManager] Texto puesto: " + message);

        VoiceManager vm = VoiceManager.Instance;
        if (vm == null)
            vm = FindAnyObjectByType<VoiceManager>();

        if (voiceClip != null && vm != null)
            vm.PlayVoice(voiceClip);
    }

    public void Hide()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (dialogueText != null)
            dialogueText.text = "";
    }
}