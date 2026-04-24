using UnityEngine;

public class LevelOneIntro : MonoBehaviour
{
    [Header("Player")]
    public PlayerMovement playerMovement;

    [Header("Configuración")]
    public bool playIntroOnStart = true;

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
    private float[] duraciones;

    void Start()
    {
        Debug.Log("[LevelOneIntro] Start ejecutado");

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

        duraciones = new float[]
        {
            3f,
            5f,
            3f,
            5f,
            3.5f,
            2.5f,
            3f
        };

        if (playIntroOnStart)
            IniciarIntro();
    }

    public void IniciarIntro()
    {
        Debug.Log("[LevelOneIntro] Intro iniciada");

        pasoActual = 0;

        if (playerMovement != null)
            playerMovement.SetCanMove(false);

        if (dm == null)
        {
            Debug.LogError("[LevelOneIntro] DialogueManager no encontrado");
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

        Debug.Log("[LevelOneIntro] Paso " + pasoActual + ": " + textos[pasoActual]);

        dm.ShowMessage(textos[pasoActual], audios[pasoActual]);

        CancelInvoke(nameof(SiguientePaso));
        Invoke(nameof(SiguientePaso), duraciones[pasoActual]);
    }

    void SiguientePaso()
    {
        pasoActual++;
        MostrarPasoActual();
    }

    void TerminarIntro()
    {
        Debug.Log("[LevelOneIntro] Intro terminada");

        CancelInvoke(nameof(SiguientePaso));

        if (dm != null)
            dm.Hide();

        if (playerMovement != null)
            playerMovement.SetCanMove(true);
    }
}