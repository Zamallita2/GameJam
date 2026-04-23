using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KeyPromptVisualEntry
{
    public KeyButtonController.KeySymbol keySymbol;
    public Material material;
}

[System.Serializable]
public class KeyLane
{
    public KeyButtonController.KeySymbol keySymbol;
    public Transform spawnPoint;
    public Transform missPoint;
}

public class KeyboardMinigameController : MonoBehaviour
{
    [Header("Botones")]
    public List<KeyButtonController> buttons = new List<KeyButtonController>();

    [Header("Prompt")]
    public GameObject promptPrefab;
    public Transform promptContainer;

    [Header("Carriles")]
    public List<KeyLane> lanes = new List<KeyLane>();

    [Header("Visuales")]
    public List<KeyPromptVisualEntry> promptVisuals = new List<KeyPromptVisualEntry>();

    [Header("Gameplay")]
    public int targetScore = 10;
    public int scorePerHit = 1;
    public int penaltyOnMiss = 1;
    public float timeLimit = 40f;

    [Header("Spawning")]
    public float moveSpeed = 1.2f;
    public float spawnInterval = 0.7f;
    public float promptScale = 0.08f;
    public int maxPromptsOnScreen = 6;

    [Header("Debug")]
    public int currentScore = 0;
    public float currentTime = 0f;

    private List<KeyPrompt3D> activePrompts = new List<KeyPrompt3D>();
    private bool isRunning = false;
    private bool isFailing = false;
    private Coroutine spawnRoutine;

    void Start()
    {
        SetupGame();
    }

    void Update()
    {
        if (!isRunning || isFailing)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            StartCoroutine(FailRoutine());
            return;
        }

        CheckKeyboardInput();
    }

    void SetupGame()
    {
        StopAllCoroutines();
        ClearAllPrompts();

        currentScore = 0;
        currentTime = timeLimit;
        isRunning = true;
        isFailing = false;

        foreach (var b in buttons)
        {
            if (b != null)
                b.SetIdle();
        }

        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        while (isRunning)
        {
            if (activePrompts.Count < maxPromptsOnScreen)
            {
                SpawnRandomPrompt();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void CheckKeyboardInput()
    {
        CheckOneKey(KeyCode.A, KeyButtonController.KeySymbol.A);
        CheckOneKey(KeyCode.S, KeyButtonController.KeySymbol.S);
        CheckOneKey(KeyCode.D, KeyButtonController.KeySymbol.D);
        CheckOneKey(KeyCode.F, KeyButtonController.KeySymbol.F);
    }

    void CheckOneKey(KeyCode keyCode, KeyButtonController.KeySymbol symbol)
    {
        if (!Input.GetKeyDown(keyCode))
            return;

        KeyButtonController button = GetButton(symbol);

        if (button != null)
            button.TriggerCorrectFeedback();

        if (activePrompts.Count == 0)
        {
            StartCoroutine(FailRoutine());
            return;
        }

        KeyPrompt3D expectedPrompt = activePrompts[0];

        if (expectedPrompt == null)
        {
            activePrompts.RemoveAt(0);
            return;
        }

        if (expectedPrompt.keySymbol != symbol)
        {
            if (button != null)
                button.TriggerWrongFeedback();

            StartCoroutine(FailRoutine());
            return;
        }

        expectedPrompt.MarkResolved();
        Destroy(expectedPrompt.gameObject);
        activePrompts.RemoveAt(0);

        currentScore += scorePerHit;
        Debug.Log($"Puntos: {currentScore}/{targetScore} | Tiempo: {currentTime:F1}");

        if (currentScore >= targetScore)
        {
            StartCoroutine(WinRoutine());
        }
    }

    void SpawnRandomPrompt()
    {
        KeyButtonController.KeySymbol symbol = GetRandomSymbol();
        KeyLane lane = GetLane(symbol);
        Material mat = GetMaterial(symbol);

        if (lane == null || lane.spawnPoint == null || lane.missPoint == null || mat == null)
            return;

        GameObject obj = Instantiate(promptPrefab, lane.spawnPoint.position, lane.spawnPoint.rotation, promptContainer);
        obj.transform.localScale = Vector3.one * promptScale;

        KeyPrompt3D prompt = obj.GetComponent<KeyPrompt3D>();
        if (prompt == null)
        {
            Destroy(obj);
            return;
        }

        prompt.Init(symbol, mat, lane.missPoint, moveSpeed, this);
        activePrompts.Add(prompt);
    }

    public void OnPromptMissed(KeyPrompt3D prompt)
    {
        if (!isRunning || isFailing)
            return;

        if (activePrompts.Contains(prompt))
            activePrompts.Remove(prompt);

        currentScore = Mathf.Max(0, currentScore - penaltyOnMiss);
        StartCoroutine(FailRoutine());
    }

    IEnumerator FailRoutine()
    {
        if (isFailing) yield break;

        isFailing = true;
        isRunning = false;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        yield return new WaitForSeconds(1f);

        SetupGame();
    }

    IEnumerator WinRoutine()
    {
        isRunning = false;

        if (spawnRoutine != null)
            StopCoroutine(spawnRoutine);

        Debug.Log("Ganaste");

        yield return new WaitForSeconds(1f);
    }

    KeyButtonController GetButton(KeyButtonController.KeySymbol symbol)
    {
        foreach (var b in buttons)
        {
            if (b != null && b.keySymbol == symbol)
                return b;
        }

        return null;
    }

    KeyButtonController.KeySymbol GetRandomSymbol()
    {
        int r = Random.Range(0, 4);

        switch (r)
        {
            case 0: return KeyButtonController.KeySymbol.A;
            case 1: return KeyButtonController.KeySymbol.S;
            case 2: return KeyButtonController.KeySymbol.D;
            default: return KeyButtonController.KeySymbol.F;
        }
    }

    KeyLane GetLane(KeyButtonController.KeySymbol symbol)
    {
        foreach (var lane in lanes)
        {
            if (lane.keySymbol == symbol)
                return lane;
        }

        return null;
    }

    Material GetMaterial(KeyButtonController.KeySymbol symbol)
    {
        foreach (var v in promptVisuals)
        {
            if (v.keySymbol == symbol)
                return v.material;
        }

        return null;
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