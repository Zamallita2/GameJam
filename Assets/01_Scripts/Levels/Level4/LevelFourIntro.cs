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
    public AudioClip audioZonaPesada;
    public AudioClip audioSistemasLentos;
    public AudioClip audioFallasCriticas;
    public AudioClip audioNuevaRespuesta;
    public AudioClip audioNoHabraTiempo;
    public AudioClip audioPenalizacion;
    public AudioClip audioSigueOperador;

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
            "Operador...\nacceso concedido al área pesada.",
            "Los sistemas aquí son más lentos...\npero sus fallas son críticas.",
            "Un error puede comprometer todo el núcleo.",
            "Se detectó un nuevo módulo de respuesta rápida.",
            "Los símbolos caerán en pantalla.\nDebes responder antes de que lleguen al límite.",
            "La penalización por fallar será mayor.",
            "Sigue avanzando, operador."
        };

        audios = new AudioClip[]
        {
            audioZonaPesada,
            audioSistemasLentos,
            audioFallasCriticas,
            audioNuevaRespuesta,
            audioNoHabraTiempo,
            audioPenalizacion,
            audioSigueOperador
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

        TimeManager time = FindAnyObjectByType<TimeManager>();
        if (time != null)
            time.Iniciar();
    }
}