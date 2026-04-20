using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonButtonsPanel : MonoBehaviour
{
    [Header("Botones")]
    public List<SimonButton3D> buttons = new List<SimonButton3D>();

    [Header("Status Lamp")]
    public StatusLamp statusLamp;

    [Header("Configuración")]
    public int nivelPrueba = 1;
    public bool iniciarAutomaticamente = true;

    [Tooltip("Tiempo que cada botón queda encendido al mostrar la secuencia")]
    public float showDelay = 0.5f;

    [Tooltip("Pausa entre botones de la secuencia")]
    public float pauseBetween = 0.15f;

    [Tooltip("Duración del parpadeo de error")]
    public float wrongFlashDuration = 0.2f;

    [Tooltip("Longitud base de la secuencia")]
    public int baseSequenceLength = 3;

    [Tooltip("Máximo número de pasos en la secuencia")]
    public int maxSequenceLength = 10;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;

    private readonly List<int> sequence = new List<int>();
    private int currentInputIndex = 0;

    private bool isShowingSequence = false;
    private bool canPlay = false;
    private int nivelActual = 1;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        if (iniciarAutomaticamente)
        {
            Setup(nivelPrueba);
        }
    }

    public void Setup(int nivel)
    {
        nivelActual = Mathf.Max(1, nivel);

        StopAllCoroutines();

        sequence.Clear();
        currentInputIndex = 0;
        isShowingSequence = false;
        canPlay = false;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == null) continue;
            buttons[i].Init(this, i);
            buttons[i].SetInactive();
        }

        if (statusLamp != null)
            statusLamp.SetNeutral();

        int sequenceLength = Mathf.Clamp(
            baseSequenceLength + (nivelActual - 1),
            baseSequenceLength,
            maxSequenceLength
        );

        GenerateSequence(sequenceLength);
        StartCoroutine(ShowSequenceRoutine());
    }

    void GenerateSequence(int length)
    {
        sequence.Clear();

        if (buttons.Count == 0)
        {
            Debug.LogWarning("SimonButtonsPanel: No hay botones asignados.");
            return;
        }

        for (int i = 0; i < length; i++)
        {
            sequence.Add(Random.Range(0, buttons.Count));
        }

        Debug.Log("Secuencia generada: " + string.Join(", ", sequence));
    }

    IEnumerator ShowSequenceRoutine()
    {
        isShowingSequence = true;
        canPlay = false;
        currentInputIndex = 0;

        if (statusLamp != null)
            statusLamp.SetNeutral();

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < sequence.Count; i++)
        {
            int buttonIndex = sequence[i];

            if (buttonIndex >= 0 && buttonIndex < buttons.Count && buttons[buttonIndex] != null)
            {
                yield return StartCoroutine(buttons[buttonIndex].FlashActive(showDelay));
                yield return new WaitForSeconds(pauseBetween);
            }
        }

        currentInputIndex = 0;
        isShowingSequence = false;
        canPlay = true;

        Debug.Log("Turno del jugador. Debe ingresar " + sequence.Count + " pasos.");
    }

    public void OnButtonPressed(SimonButton3D pressedButton)
    {
        if (pressedButton == null) return;
        if (!canPlay || isShowingSequence) return;
        if (currentInputIndex >= sequence.Count) return;

        StartCoroutine(pressedButton.FlashActive(0.2f));

        int expectedId = sequence[currentInputIndex];
        int receivedId = pressedButton.buttonId;

        Debug.Log($"Input jugador -> índice: {currentInputIndex}, esperado: {expectedId}, recibido: {receivedId}");

        if (receivedId != expectedId)
        {
            StartCoroutine(WrongRoutine());
            return;
        }

        currentInputIndex++;

        if (currentInputIndex >= sequence.Count)
        {
            StartCoroutine(CorrectRoutine());
        }
    }

    IEnumerator WrongRoutine()
    {
        canPlay = false;
        isShowingSequence = true;

        if (statusLamp != null)
            statusLamp.SetWrong();

        if (audioSource != null && wrongSound != null)
            audioSource.PlayOneShot(wrongSound);

        for (int i = 0; i < 2; i++)
        {
            foreach (var btn in buttons)
            {
                if (btn != null)
                    btn.SetError();
            }

            yield return new WaitForSeconds(wrongFlashDuration);

            foreach (var btn in buttons)
            {
                if (btn != null)
                    btn.SetInactive();
            }

            yield return new WaitForSeconds(wrongFlashDuration);
        }

        if (statusLamp != null)
            statusLamp.SetNeutral();

        currentInputIndex = 0;
        StartCoroutine(ShowSequenceRoutine());
    }

    IEnumerator CorrectRoutine()
    {
        canPlay = false;
        isShowingSequence = true;

        if (statusLamp != null)
            statusLamp.SetCorrect();

        if (audioSource != null && correctSound != null)
            audioSource.PlayOneShot(correctSound);

        foreach (var btn in buttons)
        {
            if (btn != null)
                btn.SetActive();
        }

        yield return new WaitForSeconds(0.6f);

        foreach (var btn in buttons)
        {
            if (btn != null)
                btn.SetInactive();
        }

        if (statusLamp != null)
            statusLamp.SetNeutral();

        Debug.Log("Simon completado correctamente");
    }
}