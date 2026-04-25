using UnityEngine;

public class LevelOneIntro : MonoBehaviour
{
    [Header("Player")]
    public PlayerMovement playerMovement;

    [Header("Configuración")]
    public bool playIntroOnStart = true;
    public float pausaEntreDialogos = 0.6f;
    public float extraSinAudio = 2.5f;

    [Header("Audios de introducción")]
    public AudioClip audioInicializando;
    public AudioClip audioOperador;
    public AudioClip audioFallaTotal;
    public AudioClip audioSistemasColapsados;
    public AudioClip audioRestaurarModulos;
    public AudioClip audioSinMargen;
    public AudioClip audioBuenaSuerte;

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
            "Inicializando sistema...",
            "Operador...si puedes escuchar esto...\nsignifica que eres nuestra última conexión activa.",
            "La instalación ha sufrido una falla total.",
            "Los sistemas internos han colapsado...\ny el núcleo está entrando en estado crítico.",
            "Necesitamos que restaures los módulos manualmente.",
            "No hay margen de error.",
            "Buena suerte, operador."
        };

        audios = new AudioClip[]
        {
            audioInicializando,
            audioOperador,
            audioFallaTotal,
            audioSistemasColapsados,
            audioRestaurarModulos,
            audioSinMargen,
            audioBuenaSuerte
        };

        if (playIntroOnStart)
            IniciarIntro();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TerminarIntro();
        }
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
        time.Iniciar();
    }
}