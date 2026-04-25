using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelThreeCompletionManager : MonoBehaviour
{
    public static LevelThreeCompletionManager Instance;

    [Header("Cantidad de m�quinas a reparar")]
    public int totalMachines = 4;

    [Header("Luces")]
    public float lightDisableRadius = 1f;

    [Header("Siguiente escena")]
    public string nextSceneName = "Nivel2";
    public float delayBeforeNextScene = 6f;

    [Header("Audio final opcional")]
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
        // encuentra TODAS las luces autom�ticamente
        allLights = FindObjectsByType<LightFlicker>(
            FindObjectsSortMode.None
        );
    }

    public void RegisterMachineRepaired(Transform machine)
    {
        if (levelFinished) return;

        repairedMachines++;

        Debug.Log(
            $"[LevelTwoCompletion] M�quinas reparadas: " +
            $"{repairedMachines}/{totalMachines}"
        );

        DisableNearbyLights(machine);

        if (repairedMachines >= totalMachines)
        {
            FinishLevel();
        }
    }

    void DisableNearbyLights(Transform machine)
    {
        foreach (LightFlicker light in allLights)
        {
            if (light == null || light.isTurnedOff)
                continue;

            float distance =
                Vector3.Distance(
                    machine.position,
                    light.transform.position
                );

            if (distance <= lightDisableRadius)
            {
                light.TurnOff();
            }
        }
    }

    void FinishLevel()
    {
        levelFinished = true;

        DialogueManager dm =
            FindAnyObjectByType<DialogueManager>();

        if (dm != null)
        {
            dm.ShowMessage(
                "Todos los m�dulos han sido restaurados. " +
                "El sistema vuelve a estar estable. " +
                "Excelente trabajo, operador. " +
                "Preparando siguiente zona...",
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