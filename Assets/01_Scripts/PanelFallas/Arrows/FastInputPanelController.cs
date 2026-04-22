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
    public Transform hitPoint;
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

    [Header("Lámpara")]
    public StatusLamp statusLamp;

    [Header("Cámara")]
    public Camera inputCamera;
    public LayerMask buttonLayerMask = ~0;

    [Header("Gameplay")]
    public int nivelPrueba = 1;
    public bool iniciarAutomaticamente = true;

    public float moveSpeed = 1.2f;
    public float spawnInterval = 0.8f;
    public float hitWindowDistance = 0.18f;

    public int basePromptCount = 4;
    public int maxPromptCount = 12;

    [Range(0f, 1f)] public float arrowChance = 0.65f;

    [Header("Escala prompts")]
    public float promptScale = 0.1f;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip wrongSound;
    public AudioClip correctSound;

    private readonly List<FastInputPrompt3D> activePrompts = new List<FastInputPrompt3D>();
    private readonly List<FastInputButton3D.ColorSymbol> generatedSequence = new List<FastInputButton3D.ColorSymbol>();

    private bool canPlay = false;
    private bool isRunning = false;
    private bool isFailing = false;

    private int totalSpawned = 0;
    private int totalToSpawn = 0;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        if (inputCamera == null)
            inputCamera = Camera.main;
    }

    void Start()
    {
        if (iniciarAutomaticamente)
            Setup(nivelPrueba);
    }

    void Update()
    {
        if (!canPlay || !isRunning || isFailing)
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
                {
                    OnButtonPressed(clickedButton);
                }
            }
        }
    }

    public void Setup(int level)
    {
        StopAllCoroutines();

        ClearAllPrompts();

        generatedSequence.Clear();
        totalSpawned = 0;
        totalToSpawn = 0;

        canPlay = false;
        isRunning = false;
        isFailing = false;

        if (statusLamp != null)
            statusLamp.SetNeutral();

        foreach (var button in inputButtons)
        {
            if (button != null)
                button.SetIdle();
        }

        int promptCount = Mathf.Clamp(basePromptCount + (level - 1), basePromptCount, maxPromptCount);
        totalToSpawn = promptCount;

        GenerateSequence(promptCount);

        StartCoroutine(BeginRoutine());
    }

    IEnumerator BeginRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        canPlay = true;
        isRunning = true;

        StartCoroutine(SpawnRoutine());
    }

    void GenerateSequence(int count)
    {
        generatedSequence.Clear();

        List<FastInputButton3D.ColorSymbol> pool = new List<FastInputButton3D.ColorSymbol>()
        {
            FastInputButton3D.ColorSymbol.Red,
            FastInputButton3D.ColorSymbol.Blue,
            FastInputButton3D.ColorSymbol.Green,
            FastInputButton3D.ColorSymbol.Pink
        };

        for (int i = 0; i < count; i++)
        {
            generatedSequence.Add(pool[Random.Range(0, pool.Count)]);
        }

        Debug.Log("Secuencia de colores: " + string.Join(", ", generatedSequence));
    }

    IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < generatedSequence.Count; i++)
        {
            SpawnPrompt(generatedSequence[i]);
            totalSpawned++;
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnPrompt(FastInputButton3D.ColorSymbol colorSymbol)
    {
        if (promptPrefab == null || promptContainer == null)
        {
            Debug.LogWarning("FastInputPanelController: faltan referencias del prompt.");
            return;
        }

        PromptLane lane = GetRandomLane();

        if (lane == null || lane.spawnPoint == null || lane.hitPoint == null || lane.missPoint == null)
        {
            Debug.LogWarning("No hay carriles bien configurados.");
            return;
        }

        FastInputPrompt3D.PromptType type =
            Random.value <= arrowChance
            ? FastInputPrompt3D.PromptType.Arrow
            : FastInputPrompt3D.PromptType.Circle;

        Material visualMaterial = GetMaterialFor(colorSymbol, type);

        GameObject obj = Instantiate(promptPrefab, lane.spawnPoint.position, lane.spawnPoint.rotation, promptContainer);
        obj.transform.localScale = Vector3.one * promptScale;

        FastInputPrompt3D prompt = obj.GetComponent<FastInputPrompt3D>();
        if (prompt == null)
        {
            Debug.LogWarning("El promptPrefab no tiene FastInputPrompt3D.");
            Destroy(obj);
            return;
        }

        prompt.Init(
            colorSymbol,
            type,
            visualMaterial,
            lane.hitPoint,
            lane.missPoint,
            moveSpeed,
            this
        );

        activePrompts.Add(prompt);
    }

    PromptLane GetRandomLane()
    {
        if (lanes == null || lanes.Count == 0)
            return null;

        return lanes[Random.Range(0, lanes.Count)];
    }

    Material GetMaterialFor(FastInputButton3D.ColorSymbol colorSymbol, FastInputPrompt3D.PromptType type)
    {
        foreach (var entry in promptVisuals)
        {
            if (entry.symbol == colorSymbol && entry.promptType == type)
                return entry.material;
        }

        return null;
    }

    void OnButtonPressed(FastInputButton3D button)
    {
        if (button == null || activePrompts.Count == 0 || isFailing)
            return;

        button.TriggerVisualPress();

        FastInputPrompt3D expectedPrompt = activePrompts[0];

        if (expectedPrompt == null)
        {
            activePrompts.RemoveAt(0);
            return;
        }

        if (!expectedPrompt.IsInsideHitWindow(hitWindowDistance))
        {
            StartCoroutine(FailRoutine());
            return;
        }

        if (expectedPrompt.symbol != button.symbol)
        {
            StartCoroutine(FailRoutine());
            return;
        }

        expectedPrompt.MarkResolved();
        Destroy(expectedPrompt.gameObject);
        activePrompts.RemoveAt(0);

        StartCoroutine(button.FlashPressed(0.12f));

        if (totalSpawned >= totalToSpawn && activePrompts.Count == 0)
        {
            StartCoroutine(SuccessRoutine());
        }
    }

    public void OnPromptMissed(FastInputPrompt3D prompt)
    {
        if (!isRunning || isFailing)
            return;

        if (activePrompts.Contains(prompt))
            activePrompts.Remove(prompt);

        StartCoroutine(FailRoutine());
    }

    IEnumerator FailRoutine()
    {
        if (isFailing) yield break;

        isFailing = true;
        isRunning = false;
        canPlay = false;

        if (statusLamp != null)
            statusLamp.SetWrong();

        if (audioSource != null && wrongSound != null)
            audioSource.PlayOneShot(wrongSound);

        foreach (var button in inputButtons)
        {
            if (button != null)
                StartCoroutine(button.FlashError(0.2f));
        }

        yield return new WaitForSeconds(0.8f);

        if (statusLamp != null)
            statusLamp.SetNeutral();

        Setup(nivelPrueba);
    }

    IEnumerator SuccessRoutine()
    {
        isRunning = false;
        canPlay = false;

        if (statusLamp != null)
            statusLamp.SetCorrect();

        if (audioSource != null && correctSound != null)
            audioSource.PlayOneShot(correctSound);

        yield return new WaitForSeconds(0.8f);

        if (statusLamp != null)
            statusLamp.SetNeutral();

        Debug.Log("Minijuego completado");
    }

    void ClearAllPrompts()
    {
        if (promptContainer != null)
        {
            for (int i = promptContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(promptContainer.GetChild(i).gameObject);
            }
        }

        activePrompts.Clear();
    }
}