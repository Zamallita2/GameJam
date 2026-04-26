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
    public float delayBeforeNextScene = 1f;

    [Header("Tiempos de diálogo")]
    public float pausaEntreDialogos = 0.6f;
    public float extraSinAudio = 2.5f;

    [Header("Audios finales")]
    public AudioClip audioAreaEstabilizada;
    public AudioClip audioNucleoResiste;
    public AudioClip audioLecturasAnormales;
    public AudioClip audioAlgoNoEstaBien;
    public AudioClip audioUltimoNivel;

    private int repairedMachines = 0;
    private bool levelFinished = false;

    private LightFlicker[] allLights;
    private DialogueManager dm;

    private int pasoActual = 0;
    private string[] textos;
    private AudioClip[] audios;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        allLights = FindObjectsByType<LightFlicker>(FindObjectsSortMode.None);

        dm = DialogueManager.Instance;
        if (dm == null)
            dm = FindAnyObjectByType<DialogueManager>();

        textos = new string[]
        {
            "Área pesada estabilizada.",
            "El núcleo aún resiste...\npero sus lecturas no son normales.",
            "La energía no se está restaurando.\nSolo se está conteniendo.",
            "Operador...\nalgo dentro del núcleo sigue empeorando.",
            "Preparando acceso al último nivel..."
        };

        audios = new AudioClip[]
        {
            audioAreaEstabilizada,
            audioNucleoResiste,
            audioLecturasAnormales,
            audioAlgoNoEstaBien,
            audioUltimoNivel
        };
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

        pasoActual = 0;
        MostrarPasoFinal();
    }

    void MostrarPasoFinal()
    {
        if (pasoActual >= textos.Length)
        {
            Invoke(nameof(LoadNextScene), delayBeforeNextScene);
            return;
        }

        AudioClip audioActual = audios[pasoActual];

        if (dm != null)
            dm.ShowMessage(textos[pasoActual], audioActual);

        float duracion = audioActual != null
            ? audioActual.length + pausaEntreDialogos
            : extraSinAudio;

        pasoActual++;
        Invoke(nameof(MostrarPasoFinal), duracion);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}