using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimonButtonsPanel : MonoBehaviour
{
    [Header("Botones")]
    public List<SimonButton3D> buttons = new List<SimonButton3D>();

    [Header("Status Lamp")]
    public StatusLamp statusLamp;

    [Header("Máquina dueña")]
    public MachineInteraction machineOwner;

    [Header("Configuración")]
    public int nivelPrueba = 1;
    public bool iniciarAutomaticamente = true;

    [Tooltip("Cantidad de secuencias correctas necesarias para completar el panel")]
    public int secuenciasParaCompletar = 3;

    [Tooltip("Longitud inicial de la primera secuencia")]
    public int baseSequenceLength = 3;

    [Tooltip("Cuánto aumenta la secuencia por ronda correcta")]
    public int aumentoPorSecuencia = 1;

    [Tooltip("Máximo número de pasos en la secuencia")]
    public int maxSequenceLength = 10;

    [Tooltip("Tiempo que cada botón queda encendido al mostrar la secuencia")]
    public float showDelay = 0.5f;

    [Tooltip("Pausa entre botones de la secuencia")]
    public float pauseBetween = 0.15f;

    [Tooltip("Duración del parpadeo de error")]
    public float wrongFlashDuration = 0.2f;

    [Tooltip("Pausa antes de mostrar la siguiente secuencia")]
    public float nextSequenceDelay = 0.8f;

    [Tooltip("Pausa antes de cerrar el panel al completar todo")]
    public float closePanelDelay = 1f;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip completeSound;

    [Header("Click")]
    public Camera inputCamera;
    public LayerMask buttonLayerMask = ~0;

    private readonly List<int> sequence = new List<int>();

    private int currentInputIndex = 0;
    private int nivelActual = 1;
    private int secuenciaActual = 0;

    private bool isShowingSequence = false;
    private bool canPlay = false;
    private bool completed = false;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (inputCamera == null)
            inputCamera = Camera.main;
    }

    void OnEnable()
    {
        if (iniciarAutomaticamente)
            Setup(nivelPrueba);
    }

    void Update()
    {
        if (completed) return;
        if (!canPlay || isShowingSequence) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (inputCamera == null)
                inputCamera = Camera.main;

            if (inputCamera == null)
                return;

            Ray ray = inputCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, buttonLayerMask))
            {
                SimonButton3D clickedButton = hit.collider.GetComponentInParent<SimonButton3D>();

                if (clickedButton != null)
                    clickedButton.TriggerPress();
            }
        }
    }

    public void SetMachineOwner(MachineInteraction owner)
    {
        machineOwner = owner;
    }

    public void Setup(int nivel)
    {
        nivelActual = Mathf.Max(1, nivel);

        StopAllCoroutines();

        sequence.Clear();
        currentInputIndex = 0;
        secuenciaActual = 0;

        isShowingSequence = false;
        canPlay = false;
        completed = false;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (buttons[i] == null) continue;

            buttons[i].Init(this, i);
            buttons[i].SetInactive();
        }

        if (statusLamp != null)
            statusLamp.SetNeutral();

        StartCoroutine(StartNextSequenceRoutine());
    }

    IEnumerator StartNextSequenceRoutine()
    {
        canPlay = false;
        isShowingSequence = true;
        currentInputIndex = 0;

        yield return new WaitForSeconds(nextSequenceDelay);

        int sequenceLength = Mathf.Clamp(
            baseSequenceLength + (secuenciaActual * aumentoPorSecuencia) + (nivelActual - 1),
            baseSequenceLength,
            maxSequenceLength
        );

        GenerateSequence(sequenceLength);

        yield return StartCoroutine(ShowSequenceRoutine());
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
            sequence.Add(Random.Range(0, buttons.Count));

        Debug.Log($"Secuencia {secuenciaActual + 1}/{secuenciasParaCompletar}: " + string.Join(", ", sequence));
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

        Debug.Log($"Turno del jugador. Secuencia {secuenciaActual + 1}/{secuenciasParaCompletar}. Debe ingresar {sequence.Count} pasos.");
    }

    public void OnButtonPressed(SimonButton3D pressedButton)
    {
        if (pressedButton == null) return;
        if (!canPlay || isShowingSequence || completed) return;
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
            StartCoroutine(CorrectRoutine());
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

        yield return StartCoroutine(ShowSequenceRoutine());
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

        secuenciaActual++;

        Debug.Log($"Secuencia correcta {secuenciaActual}/{secuenciasParaCompletar}");

        if (secuenciaActual >= secuenciasParaCompletar)
        {
            StartCoroutine(CompletePanelRoutine());
        }
        else
        {
            yield return StartCoroutine(StartNextSequenceRoutine());
        }
    }

    IEnumerator CompletePanelRoutine()
    {
        completed = true;
        canPlay = false;
        isShowingSequence = true;

        if (statusLamp != null)
            statusLamp.SetCorrect();

        if (audioSource != null)
        {
            if (completeSound != null)
                audioSource.PlayOneShot(completeSound);
            else if (correctSound != null)
                audioSource.PlayOneShot(correctSound);
        }

        foreach (var btn in buttons)
        {
            if (btn != null)
                btn.SetActive();
        }

        Debug.Log("Panel de botones completado. Cerrando panel y apagando brillo de máquina.");

        yield return new WaitForSeconds(closePanelDelay);

        foreach (var btn in buttons)
        {
            if (btn != null)
                btn.SetInactive();
        }

        if (machineOwner != null)
        {
            machineOwner.MarcarMaquinaReparada();
            machineOwner.CerrarPanelDesdeMinijuego();
        }
        else
        {
            Debug.LogWarning("SimonButtonsPanel: No hay machineOwner asignado.");
        }
    }
}