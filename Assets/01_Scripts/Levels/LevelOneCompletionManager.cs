using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelOneCompletionManager : MonoBehaviour
{
    public static LevelOneCompletionManager Instance;

    [Header("Cantidad de máquinas a reparar")]
    public int totalMachines = 4;

    [Header("Siguiente escena")]
    public string nextSceneName = "Nivel2";
    public float delayBeforeNextScene = 6f;

    [Header("Audio final opcional")]
    public AudioClip finalVoice;

    private int repairedMachines = 0;
    private bool levelFinished = false;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterMachineRepaired()
    {
        if (levelFinished) return;

        repairedMachines++;

        Debug.Log($"[LevelOneCompletion] Máquinas reparadas: {repairedMachines}/{totalMachines}");

        if (repairedMachines >= totalMachines)
        {
            FinishLevel();
        }
    }

    void FinishLevel()
    {
        levelFinished = true;

        DialogueManager dm = FindAnyObjectByType<DialogueManager>();

        if (dm != null)
        {
            dm.ShowMessage(
                "Todos los módulos han sido restaurados. El sistema vuelve a estar estable. Excelente trabajo, operador. Preparando siguiente zona...",
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