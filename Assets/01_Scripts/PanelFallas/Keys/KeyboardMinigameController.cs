using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class KeyPromptVisualEntry
{
    public KeySymbolType keySymbol;
    public Material material;
}

[System.Serializable]
public class KeyLane
{
    public KeySymbolType keySymbol;
    public Transform spawnPoint;
    public Transform missPoint;
}

public class KeyboardMinigameController : MonoBehaviour
{
    [Header("Luces decorativas")]
    public KeyLightsController keyLights;

    [Header("Prompt")]
    public GameObject promptPrefab;
    public Transform promptContainer;

    [Header("Carriles")]
    public List<KeyLane> lanes = new List<KeyLane>();

    [Header("Materiales de prompts")]
    public List<KeyPromptVisualEntry> promptVisuals = new List<KeyPromptVisualEntry>();

    [Header("UI opcional")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI errorsText;

    [Header("Dificultad")]
    [Min(1)] public int difficultyLevel = 1;

    [Header("Base Gameplay")]
    public int baseTargetScore = 8;
    public float baseTimeLimit = 35f;
    public int baseMaxErrors = 3;

    [Header("Base Movimiento")]
    public float baseMoveSpeed = 1.1f;
    public float baseSpawnInterval = 0.85f;
    public int baseMaxPromptsOnScreen = 4;

    [Header("Escalado por dificultad")]
    public int scorePerDifficulty = 2;
    public float speedPerDifficulty = 0.25f;
    public float spawnIntervalDecreasePerDifficulty = 0.08f;
    public int promptsPerDifficulty = 1;

    [Header("Límites")]
    public float minSpawnInterval = 0.35f;
    public float maxMoveSpeed = 4f;
    public int maxPromptsOnScreenLimit = 10;

    [Header("Prompt Scale")]
    public float promptScale = 0.08f;

    [Header("Puntaje")]
    public int scorePerHit = 1;
    public int penaltyOnMiss = 1;

    [Header("Final")]
    public float closePanelDelay = 1f;

    [Header("Debug")]
    public int currentScore = 0;
    public int currentErrors = 0;
    public float currentTime = 0f;

    private int targetScore;
    private int maxErrors;
    private float timeLimit;
    private float moveSpeed;
    private float spawnInterval;
    private int maxPromptsOnScreen;

    private readonly List<KeyPrompt3D> activePrompts = new List<KeyPrompt3D>();

    private bool isRunning = false;
    private bool isFinished = false;
    private Coroutine spawnRoutine;
    private MachineInteraction machineOwner;

    void OnEnable()
    {
        StartGame();
    }

    void OnDisable()
    {
        StopAllCoroutines();
        ClearAllPrompts();

        if (keyLights != null)
            keyLights.TurnOffAll();
    }

    void Update()
    {
        if (!isRunning || isFinished)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            UpdateUI();
            StartCoroutine(FailAndRestartRoutine());
            return;
        }

        CheckKeyboardInput();
        UpdateUI();
    }

    public void SetMachineOwner(MachineInteraction owner)
    {
        machineOwner = owner;
    }

    public void StartGame()
    {
        StopAllCoroutines();
        ClearAllPrompts();

        CalculateDifficulty();

        currentScore = 0;
        currentErrors = 0;
        currentTime = timeLimit;

        isRunning = true;
        isFinished = false;

        if (keyLights != null)
            keyLights.TurnOffAll();

        UpdateUI();

        spawnRoutine = StartCoroutine(SpawnRoutine());

        Debug.Log($"[KeyboardMinigame] Iniciado dificultad {difficultyLevel}");
    }

    void CalculateDifficulty()
    {
        int level = Mathf.Max(1, difficultyLevel);
        int extra = level - 1;

        targetScore = baseTargetScore + (extra * scorePerDifficulty);
        maxErrors = baseMaxErrors;
        timeLimit = baseTimeLimit;

        moveSpeed = Mathf.Min(
            baseMoveSpeed + (extra * speedPerDifficulty),
            maxMoveSpeed
        );

        spawnInterval = Mathf.Max(
            baseSpawnInterval - (extra * spawnIntervalDecreasePerDifficulty),
            minSpawnInterval
        );

        maxPromptsOnScreen = Mathf.Min(
            baseMaxPromptsOnScreen + (extra * promptsPerDifficulty),
            maxPromptsOnScreenLimit
        );
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        while (isRunning && !isFinished)
        {
            if (activePrompts.Count < maxPromptsOnScreen)
                SpawnRandomPrompt();

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnRandomPrompt()
    {
        if (promptPrefab == null)
        {
            Debug.LogError("[KeyboardMinigame] Falta Prompt Prefab");
            return;
        }

        if (promptContainer == null)
        {
            Debug.LogError("[KeyboardMinigame] Falta Prompt Container");
            return;
        }

        KeySymbolType symbol = GetRandomSymbol();
        KeyLane lane = GetLane(symbol);
        Material material = GetMaterial(symbol);

        if (lane == null || lane.spawnPoint == null || lane.missPoint == null)
        {
            Debug.LogWarning("[KeyboardMinigame] Falta lane/spawn/miss para " + symbol);
            return;
        }

        GameObject obj = Instantiate(
            promptPrefab,
            lane.spawnPoint.position,
            lane.spawnPoint.rotation,
            promptContainer
        );

        obj.transform.localScale = Vector3.one * promptScale;

        KeyPrompt3D prompt = obj.GetComponent<KeyPrompt3D>();

        if (prompt == null)
        {
            Debug.LogError("[KeyboardMinigame] El prefab no tiene KeyPrompt3D");
            Destroy(obj);
            return;
        }

        prompt.Init(symbol, material, lane.missPoint, moveSpeed, this);
        activePrompts.Add(prompt);
    }

    void CheckKeyboardInput()
    {
        CheckOneKey(KeyCode.A, KeySymbolType.A);
        CheckOneKey(KeyCode.S, KeySymbolType.S);
        CheckOneKey(KeyCode.D, KeySymbolType.D);
        CheckOneKey(KeyCode.F, KeySymbolType.F);
    }

    void CheckOneKey(KeyCode keyCode, KeySymbolType symbol)
    {
        if (!Input.GetKeyDown(keyCode))
            return;

        if (activePrompts.Count == 0)
        {
            if (keyLights != null)
                keyLights.FlashError(symbol);

            AddError();
            return;
        }

        KeyPrompt3D prompt = activePrompts[0];

        if (prompt == null)
        {
            activePrompts.RemoveAt(0);
            return;
        }

        if (prompt.keySymbol == symbol)
        {
            if (keyLights != null)
                keyLights.Flash(symbol);

            prompt.Resolve();
            activePrompts.RemoveAt(0);

            currentScore += scorePerHit;

            Debug.Log("[KeyboardMinigame] Correcto " + currentScore + "/" + targetScore);

            if (currentScore >= targetScore)
                StartCoroutine(WinRoutine());
        }
        else
        {
            if (keyLights != null)
                keyLights.FlashError(symbol);

            AddError();
        }
    }

    void AddError()
    {
        currentErrors++;
        currentScore = Mathf.Max(0, currentScore - penaltyOnMiss);

        Debug.Log("[KeyboardMinigame] Error " + currentErrors + "/" + maxErrors);

        if (currentErrors >= maxErrors)
            StartCoroutine(FailAndRestartRoutine());
    }

    public void OnPromptMissed(KeyPrompt3D prompt)
    {
        if (!isRunning || isFinished)
            return;

        if (activePrompts.Contains(prompt))
            activePrompts.Remove(prompt);

        AddError();
    }

    IEnumerator FailAndRestartRoutine()
    {
        if (isFinished) yield break;

        isRunning = false;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        ClearAllPrompts();

        if (keyLights != null)
        {
            keyLights.FlashError(KeySymbolType.A);
            keyLights.FlashError(KeySymbolType.S);
            keyLights.FlashError(KeySymbolType.D);
            keyLights.FlashError(KeySymbolType.F);
        }

        Debug.Log("[KeyboardMinigame] Fallaste. Reiniciando...");

        yield return new WaitForSeconds(1f);

        StartGame();
    }

    IEnumerator WinRoutine()
    {
        if (isFinished) yield break;

        isFinished = true;
        isRunning = false;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        ClearAllPrompts();

        if (keyLights != null)
        {
            keyLights.Flash(KeySymbolType.A);
            keyLights.Flash(KeySymbolType.S);
            keyLights.Flash(KeySymbolType.D);
            keyLights.Flash(KeySymbolType.F);
        }

        Debug.Log("[KeyboardMinigame] Ganaste");

        yield return new WaitForSeconds(closePanelDelay);

        if (machineOwner != null)
        {
            machineOwner.MarcarMaquinaReparada();
            machineOwner.CerrarPanelDesdeMinijuego();
        }
        else
        {
            Debug.LogWarning("[KeyboardMinigame] No tiene MachineOwner. Se ganó pero no cerró panel.");
        }
    }

    void ClearAllPrompts()
    {
        foreach (KeyPrompt3D prompt in activePrompts)
        {
            if (prompt != null)
                Destroy(prompt.gameObject);
        }

        activePrompts.Clear();

        if (promptContainer != null)
        {
            for (int i = promptContainer.childCount - 1; i >= 0; i--)
            {
                Transform child = promptContainer.GetChild(i);
                if (child != null)
                    Destroy(child.gameObject);
            }
        }
    }

    void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = currentScore + " / " + targetScore;

        if (timerText != null)
            timerText.text = Mathf.CeilToInt(currentTime).ToString();

        if (errorsText != null)
            errorsText.text = currentErrors + " / " + maxErrors;
    }

    KeyLane GetLane(KeySymbolType symbol)
    {
        foreach (KeyLane lane in lanes)
        {
            if (lane != null && lane.keySymbol == symbol)
                return lane;
        }

        return null;
    }

    Material GetMaterial(KeySymbolType symbol)
    {
        foreach (KeyPromptVisualEntry entry in promptVisuals)
        {
            if (entry != null && entry.keySymbol == symbol)
                return entry.material;
        }

        return null;
    }

    KeySymbolType GetRandomSymbol()
    {
        int r = Random.Range(0, 4);

        if (r == 0) return KeySymbolType.A;
        if (r == 1) return KeySymbolType.S;
        if (r == 2) return KeySymbolType.D;

        return KeySymbolType.F;
    }
}