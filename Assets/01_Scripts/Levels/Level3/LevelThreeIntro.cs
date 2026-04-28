using UnityEngine;

public class LevelThreeIntro : MonoBehaviour
{
    [Header("Player")]
    public PlayerMovement playerMovement;

    [Header("Configuración")]
    public bool playIntroOnStart = true;
    public float pausaEntreDialogos = 0.6f;
    public float extraSinAudio = 2.5f;

    [Header("Audios de introducción")]
    public AudioClip audioEstabilidadTemporal;
    public AudioClip audioSincronizacion;
    public AudioClip audioCargaNucleo;
    public AudioClip audioFallasRapidas;
    public AudioClip audioSinAyuda;
    public AudioClip audioActuaSolo;
    public AudioClip audioSinRecuperacion;

    private DialogueManager dm;
    private int pasoActual = 0;

    private string[] textos;
    private AudioClip[] audios;

    void Start()
    {
        Time.timeScale = 1f;

        dm = DialogueManager.Instance;
        if (dm == null)
            dm = FindAnyObjectByType<DialogueManager>();

        textos = new string[]
        {
            "Operador...\nLa estabilidad del sistema es temporal.",
            "Los módulos restaurados no mantienen sincronización completa.",
            "La carga del núcleo ha aumentado.",
            "Las fallas están reapareciendo...\nmás rápido que antes.",
            "No puedo asistirte más.\nYa conoces los sistemas.",
            "Debes actuar por tu cuenta.",
            "Si fallas ahora...\nno habrá recuperación."
        };

        audios = new AudioClip[]
        {
            audioEstabilidadTemporal,
            audioSincronizacion,
            audioCargaNucleo,
            audioFallasRapidas,
            audioSinAyuda,
            audioActuaSolo,
            audioSinRecuperacion
        };

        if (playIntroOnStart)
            IniciarIntro();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            TerminarIntro();
    }

    public void IniciarIntro()
    {
        pasoActual = 0;

        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        if (dm == null)
        {
            TerminarIntro();
            return;
        }

        MostrarPasoActual();
    }

    void MostrarPasoActual()
    {
        if (pasoActual >= textos.Length)
        {
            TerminarIntro();
            return;
        }

        AudioClip audioActual = audios[pasoActual];

        dm.ShowMessage(textos[pasoActual], audioActual);

        float duracion = audioActual != null ? audioActual.length + pausaEntreDialogos : extraSinAudio;

        CancelInvoke(nameof(SiguientePaso));
        Invoke(nameof(SiguientePaso), duracion);
    }

    void SiguientePaso()
    {
        pasoActual++;
        MostrarPasoActual();
    }

    void TerminarIntro()
    {
        CancelInvoke(nameof(SiguientePaso));

        if (dm != null)
            dm.Hide();

        if (playerMovement != null)
            playerMovement.SetCanMove(true);

        TimeManager time = FindFirstObjectByType<TimeManager>();
        if (time != null)
            time.Iniciar();
    }
}