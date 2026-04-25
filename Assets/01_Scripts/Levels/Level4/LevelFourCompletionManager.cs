using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelFourCompletionManager : MonoBehaviour
{
    public static LevelFourCompletionManager Instance;

    [Header("Cantidad de máquinas a reparar")]
    public int totalMachines = 5;

    [Header("Luces")]
    public float lightDisableRadius = 5f;

    [Header("Siguiente escena")]
    public string nextSceneName = "Nivel5";
    public float delayBeforeNextScene = 6f;

    [Header("Audio final")]
    public AudioClip finalVoice;

    private int repairedMachines = 0;
    private bool levelFinished = false;

    private LightFlicker[] allLights;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        allLights = FindObjectsByType<LightFlicker>(FindObjectsSortMode.None);
    }

    public void RegisterMachineRepaired(Transform machine)
    {
        if (levelFinished) return;

        repairedMachines++;

        Debug.Log("[LevelFourCompletion] Máquinas reparadas: " + repairedMachines + "/" + totalMachines);

        DisableNearbyLights(machine);

        if (repairedMachines >= totalMachines)
            FinishLevel();
    }

    void DisableNearbyLights(Transform machine)
    {
        foreach (LightFlicker light in allLights)
        {
            if (light == null || light.isTurnedOff)
                continue;

            float distance = Vector3.Distance(machine.position, light.transform.position);

            if (distance <= lightDisableRadius)
                light.TurnOff();
        }
    }

    void FinishLevel()
    {
        levelFinished = true;

        TimeManager time = FindAnyObjectByType<TimeManager>();
        if (time != null)
            time.timerPausado = true;

        DialogueManager dm = FindAnyObjectByType<DialogueManager>();

        if (dm != null)
        {
            dm.ShowMessage(
                "Área pesada estabilizada.\nEl núcleo aún resiste.\nPreparando acceso al último nivel...",
                finalVoice
            );
        }

        Invoke(nameof(LoadNextScene), delayBeforeNextScene);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}