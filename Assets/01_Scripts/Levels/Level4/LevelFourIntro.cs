using UnityEngine;

public class LevelFourIntro : MonoBehaviour
{
    [Header("Player")]
    public PlayerMovement playerMovement;

    [Header("Configuración")]
    public bool playIntroOnStart = true;
    public float pausaEntreDialogos = 0.6f;
    public float extraSinAudio = 2.5f;

    [Header("Audios de introducción")]
    public AudioClip audioConexionProduccion;
    public AudioClip audioPrimerSector;
    public AudioClip audioFallaExtendida;
    public AudioClip audioAreaProduccion;
    public AudioClip audioMasModulos;
    public AudioClip audioSistemasConocidos;
    public AudioClip audioNoRepetire;
    public AudioClip audioNuevaFalla;
    public AudioClip audioEngranajes;
    public AudioClip audioReparaModulos;
    public AudioClip audioAvanzaCuidado;

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
            "Operador, conexión establecida con la zona de producción.",
            "El primer sector fue restaurado correctamente.",
            "Pero la falla se ha extendido.",
            "Esta área controla procesos básicos de producción.",
            "Encontrarás más módulos dañados.",
            "Algunos sistemas ya los conoces.",
            "No repetiré instrucciones anteriores.",
            "Sin embargo, se detectó una nueva falla mecánica.",
            "Los engranajes de sincronización están fuera de posición.",
            "Repara los módulos activos antes de que el núcleo pierda estabilidad.",
            "Avanza con cuidado, operador."
        };

        audios = new AudioClip[]
        {
            audioConexionProduccion,
            audioPrimerSector,
            audioFallaExtendida,
            audioAreaProduccion,
            audioMasModulos,
            audioSistemasConocidos,
            audioNoRepetire,
            audioNuevaFalla,
            audioEngranajes,
            audioReparaModulos,
            audioAvanzaCuidado
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

        float duracion = audioActual != null
            ? audioActual.length + pausaEntreDialogos
            : extraSinAudio;

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