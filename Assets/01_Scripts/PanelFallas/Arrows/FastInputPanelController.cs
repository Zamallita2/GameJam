using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PromptVisualEntry
{
    public FastInputButton3D.ColorSymbol symbol;
    public FastInputPrompt3D.PromptType promptType;
    public Material material;
}

[System.Serializable]
public class PromptLane
{
    public string laneName;
    public Transform spawnPoint;
    public Transform missPoint;
}

public class FastInputPanelController : MonoBehaviour
{
    [Header("Botones físicos")]
    public List<FastInputButton3D> inputButtons = new List<FastInputButton3D>();

    [Header("Prompt prefab")]
    public GameObject promptPrefab;
    public Transform promptContainer;

    [Header("Carriles neutrales")]
    public List<PromptLane> lanes = new List<PromptLane>();

    [Header("Visuales de prompts")]
    public List<PromptVisualEntry> promptVisuals = new List<PromptVisualEntry>();

    [Header("Lámparas")]
    public List<StatusLampa> statusLamps = new List<StatusLampa>();

    [Header("Cámara")]
    public Camera inputCamera;
    public LayerMask buttonLayerMask = ~0;

    [Header("Gameplay")]
    public int nivelPrueba = 1;
    public bool iniciarAutomaticamente = true;

    [Header("Movimiento")]
    public float moveSpeed = 1.2f;
    public float spawnInterval = 0.7f;
    public float promptScale = 0.05f;

    [Header("Objetivo")]
    public int targetScore = 10;
    public int scorePerHit = 1;
    public float timeLimit = 45f;
    public bool loseOnMiss = false;
    public int penaltyOnMiss = 1;

    [Header("Generación")]
    [Range(0f, 1f)] public float arrowChance = 0.65f;
    public int maxPromptsOnScreen = 6;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip wrongSound;
    public AudioClip correctSound;

    [Header("Final")]
    public float closePanelDelay = 1f;

    [Header("Debug")]
    public int currentScore = 0;
    public float currentTime = 0f;

    private readonly List<FastInputPrompt3D> activePrompts = new List<FastInputPrompt3D>();

    private bool canPlay = false;
    private bool isRunning = false;
    private bool isFailing = false;
    private bool isCompleted = false;

    private Coroutine spawnRoutine;
    private MachineInteraction machineOwner;

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

    void OnDisable()
    {
        StopAllCoroutines();
        ClearAllPrompts();
    }

    public void SetMachineOwner(MachineInteraction owner)
    {
        machineOwner = owner;
    }

    public void Setup(int level)
    {
        StopAllCoroutines();
        ClearAllPrompts();

        canPlay = false;
        isRunning = false;
        isFailing = false;
        isCompleted = false;

        currentScore = 0;
        currentTime = timeLimit + ((level - 1) * 5f);

        moveSpeed = 0.1f + ((level - 1) * 0.075f);
        spawnInterval = Mathf.Max(0.35f, 0.75f - ((level - 1) * 0.05f));

        SetLampsNeutral();

        foreach (var button in inputButtons)
        {
            if (button != null)
                button.SetIdle();
        }

        if (!HasValidPromptReferences())
        {
            Debug.LogWarning("[FastInputPanel] Faltan referencias.");
            return;
        }

        StartCoroutine(BeginRoutine());
    }

    bool HasValidPromptReferences()
    {
        if (promptPrefab == null || promptContainer == null)
            return false;

        if (lanes == null || lanes.Count == 0)
            return false;

        foreach (var lane in lanes)
        {
            if (lane == null || lane.spawnPoint == null || lane.missPoint == null)
                return false;
        }

        return true;
    }

    IEnumerator BeginRoutine()
    {
        yield return new WaitForSeconds(0.4f);

        canPlay = true;
        isRunning = true;

        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        if (!isRunning || isFailing || isCompleted)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            StartCoroutine(FailRoutine());
            return;
        }

        if (!canPlay)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (inputCamera == null)
                inputCamera = Camera.main;

            if (inputCamera == null)
                return;

            Ray ray = inputCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, buttonLayerMask))
            {
                FastInputButton3D clickedButton = hit.collider.GetComponentInParent<FastInputButton3D>();

                if (clickedButton != null)
                    OnButtonPressed(clickedButton);
            }
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (isRunning && !isCompleted)
        {
            if (activePrompts.Count < maxPromptsOnScreen)
                SpawnRandomPrompt();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRandomPrompt()
    {
        PromptLane lane = GetRandomLane();

        if (lane == null || lane.spawnPoint == null || lane.missPoint == null)
            return;

        FastInputButton3D.ColorSymbol colorSymbol = GetRandomColor();

        FastInputPrompt3D.PromptType promptType =
            Random.value <= arrowChance
            ? FastInputPrompt3D.PromptType.Arrow
            : FastInputPrompt3D.PromptType.Circle;

        Material visualMaterial = GetMaterialFor(colorSymbol, promptType);

        GameObject obj = Instantiate(
            promptPrefab,
            lane.spawnPoint.position,
            lane.spawnPoint.rotation,
            promptContainer
        );

        obj.transform.localScale = Vector3.one * promptScale;

        FastInputPrompt3D prompt = obj.GetComponent<FastInputPrompt3D>();

        if (prompt == null)
        {
            Destroy(obj);
            return;
        }

        prompt.Init(
            colorSymbol,
            promptType,
            visualMaterial,
            lane.missPoint,
            moveSpeed,
            this
        );

        activePrompts.Add(prompt);
    }

    void OnButtonPressed(FastInputButton3D button)
    {
        if (button == null || activePrompts.Count == 0 || isFailing || isCompleted)
            return;

        button.TriggerVisualPress();

        FastInputPrompt3D expectedPrompt = activePrompts[0];

        if (expectedPrompt == null)
        {
            activePrompts.RemoveAt(0);
            return;
        }

        bool sameColor = expectedPrompt.symbol == button.symbol;

        bool sameType =
            (expectedPrompt.promptType == FastInputPrompt3D.PromptType.Arrow &&
             button.buttonType == FastInputButton3D.ButtonVisualType.Arrow)
            ||
            (expectedPrompt.promptType == FastInputPrompt3D.PromptType.Circle &&
             button.buttonType == FastInputButton3D.ButtonVisualType.Circle);

        if (!sameColor || !sameType)
        {
            StartCoroutine(FailRoutine());
            return;
        }

        expectedPrompt.MarkResolved();
        Destroy(expectedPrompt.gameObject);
        activePrompts.RemoveAt(0);

        StartCoroutine(button.FlashPressed(0.12f));

        currentScore += scorePerHit;

        if (currentScore >= targetScore)
            StartCoroutine(SuccessRoutine());
    }

    public void OnPromptMissed(FastInputPrompt3D prompt)
    {
        if (!isRunning || isFailing || isCompleted)
            return;

        if (activePrompts.Contains(prompt))
            activePrompts.Remove(prompt);

        if (loseOnMiss)
        {
            StartCoroutine(FailRoutine());
            return;
        }

        currentScore = Mathf.Max(0, currentScore - penaltyOnMiss);
    }

    IEnumerator FailRoutine()
    {
        if (isFailing || isCompleted) yield break;

        isFailing = true;
        isRunning = false;
        canPlay = false;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        SetLampsWrong();

        if (audioSource != null && wrongSound != null)
            audioSource.PlayOneShot(wrongSound);

        LevelFourDialogueController dialogue = FindAnyObjectByType<LevelFourDialogueController>();
        if (dialogue != null)
            dialogue.ArrowsError();

        foreach (var button in inputButtons)
        {
            if (button != null)
                StartCoroutine(button.FlashError(0.2f));
        }

        yield return new WaitForSeconds(1f);

        SetLampsNeutral();
        Setup(nivelPrueba);
    }

    IEnumerator SuccessRoutine()
    {
        if (isCompleted) yield break;

        isCompleted = true;
        isRunning = false;
        canPlay = false;
        isFailing = false;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        ClearAllPrompts();
        SetLampsCorrect();

        if (audioSource != null && correctSound != null)
            audioSource.PlayOneShot(correctSound);

        LevelFourDialogueController dialogue = FindAnyObjectByType<LevelFourDialogueController>();
        if (dialogue != null)
            dialogue.ArrowsExito();

        yield return new WaitForSeconds(closePanelDelay);

        SetLampsNeutral();

        if (machineOwner != null)
        {
            machineOwner.MarcarMaquinaReparada();
            machineOwner.CerrarPanelDesdeMinijuego();
        }
        else
        {
            Debug.LogWarning("[FastInputPanel] No tiene MachineOwner.");
        }
    }

    void ClearAllPrompts()
    {
        if (promptContainer != null)
        {
            for (int i = promptContainer.childCount - 1; i >= 0; i--)
                Destroy(promptContainer.GetChild(i).gameObject);
        }

        activePrompts.Clear();
    }

    void SetLampsNeutral()
    {
        foreach (var lamp in statusLamps)
        {
            if (lamp != null)
                lamp.SetNeutral();
        }
    }

    void SetLampsCorrect()
    {
        foreach (var lamp in statusLamps)
        {
            if (lamp != null)
                lamp.SetCorrect();
        }
    }

    void SetLampsWrong()
    {
        foreach (var lamp in statusLamps)
        {
            if (lamp != null)
                lamp.SetWrong();
        }
    }

    FastInputButton3D.ColorSymbol GetRandomColor()
    {
        int r = Random.Range(0, 4);

        if (r == 0) return FastInputButton3D.ColorSymbol.Red;
        if (r == 1) return FastInputButton3D.ColorSymbol.Blue;
        if (r == 2) return FastInputButton3D.ColorSymbol.Green;

        return FastInputButton3D.ColorSymbol.Pink;
    }

    PromptLane GetRandomLane()
    {
        if (lanes == null || lanes.Count == 0)
            return null;

        return lanes[Random.Range(0, lanes.Count)];
    }

    Material GetMaterialFor(FastInputButton3D.ColorSymbol colorSymbol, FastInputPrompt3D.PromptType promptType)
    {
        foreach (var entry in promptVisuals)
        {
            if (entry.symbol == colorSymbol && entry.promptType == promptType)
                return entry.material;
        }

        return null;
    }
}