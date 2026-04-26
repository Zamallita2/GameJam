using UnityEngine;

public class LevelFiveIntro : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public bool playIntroOnStart = true;
    public float pausaEntreDialogos = 0.6f;
    public float extraSinAudio = 2.5f;

    [Header("Audios intro")]
    public AudioClip audioAccesoCore;
    public AudioClip audioDemasiadoLejos;
    public AudioClip audioTodoActivo;
    public AudioClip audioFallasNoAccidentales;
    public AudioClip audioVuelvenAFallar;
    public AudioClip audioDecidir;
    public AudioClip audioDecidir2;
    public AudioClip audioSistemaAprende;

    private DialogueManager dm;
    private int pasoActual;

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
            "Inicializando acceso al núcleo...",
            "Operador...\nhas llegado demasiado lejos.",
            "Todos los módulos están activos...\npero el sistema ya no responde como antes.",
            "Las fallas no son accidentales.\nAlgo dentro del núcleo las está provocando.",
            "Cada sistema que repares...\nvolverá a fallar.",
            "No podrás estabilizar todo al mismo tiempo.",
            "Debes decidir qué mantener activo y que dejar caer.",
            "Ten cuidado, operador...\nel sistema está aprendiendo de ti."
        };

        audios = new AudioClip[]
        {
            audioAccesoCore,
            audioDemasiadoLejos,
            audioTodoActivo,
            audioFallasNoAccidentales,
            audioVuelvenAFallar,
            audioDecidir,
            audioDecidir2,
            audioSistemaAprende
        };

        if (playIntroOnStart)
            IniciarIntro();
    }

    public void IniciarIntro()
    {
        pasoActual = 0;

        if (playerMovement != null)
            playerMovement.SetCanMove(false);

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

        if (dm != null)
            dm.ShowMessage(textos[pasoActual], audioActual);

        float duracion = audioActual != null ? audioActual.length + pausaEntreDialogos : extraSinAudio;

        pasoActual++;
        Invoke(nameof(MostrarPasoActual), duracion);
    }

    void TerminarIntro()
    {
        CancelInvoke(nameof(MostrarPasoActual));

        if (dm != null)
            dm.Hide();

        if (playerMovement != null)
            playerMovement.SetCanMove(true);

        TimeManager time = FindAnyObjectByType<TimeManager>();
        if (time != null)
            time.Iniciar();
    }
}