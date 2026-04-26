using UnityEngine;

public class LevelTwoDialogueController : MonoBehaviour
{
    [Header("GENERALES")]
    public AudioClip audioModuloActivo;
    public AudioClip audioIncorrecto;
    public AudioClip audioModuloRestaurado;

    [Header("BOTONES")]
    public AudioClip audioBotonesError;
    public AudioClip audioBotonesExito;

    [Header("CABLES")]
    public AudioClip audioCablesError;
    public AudioClip audioCablesExito;

    [Header("PUZZLE")]
    public AudioClip audioPuzzleError;
    public AudioClip audioPuzzleExito;

    [Header("ENGRANAJES INTRO POR PARTES")]
    public AudioClip audioGearSistema;
    public AudioClip audioGearOrden;
    public AudioClip audioGearSinSecuencia;
    public AudioClip audioGearIntento;

    [Header("ENGRANAJES RESULTADO")]
    public AudioClip audioGearError;
    public AudioClip audioGearExito;

    [Header("FINAL")]
    public AudioClip audioFinalNivel;

    [Header("Tiempos")]
    public float pausaEntreDialogos = 0.3f;
    public float duracionSinAudio = 1.2f;

    private DialogueManager dm;
    private int gearPasoActual = 0;

    private string[] gearTextos;
    private AudioClip[] gearAudios;

    void Awake()
    {
        dm = DialogueManager.Instance;

        if (dm == null)
            dm = FindAnyObjectByType<DialogueManager>();

        gearTextos = new string[]
        {
            "Sistema de engranajes desincronizado.",
            "Presiona los engranajes en el orden correcto.",
            "El sistema no puede mostrar la secuencia.",
            "Tendrás que adivinar el orden."
        };

        gearAudios = new AudioClip[]
        {
            audioGearSistema,
            audioGearOrden,
            audioGearSinSecuencia,
            audioGearIntento
        };
    }

    void Show(string texto, AudioClip audio = null)
    {
        if (dm == null)
            dm = FindAnyObjectByType<DialogueManager>();

        if (dm != null)
            dm.ShowMessage(texto, audio);
    }

    public void ModuloActivo()
    {
        Show("Módulo activo.", audioModuloActivo);
    }

    public void Incorrecto()
    {
        Show("Incorrecto.", audioIncorrecto);
    }

    public void ModuloRestaurado()
    {
        Show("Módulo restaurado.", audioModuloRestaurado);
    }

    public void BotonesError()
    {
        Show("Secuencia incorrecta.", audioBotonesError);
    }

    public void BotonesExito()
    {
        Show("Memoria estabilizada.", audioBotonesExito);
    }

    public void CablesError()
    {
        Show("Conexión incorrecta.", audioCablesError);
    }

    public void CablesExito()
    {
        Show("Energía restaurada.", audioCablesExito);
    }

    public void PuzzleError()
    {
        Show("Movimiento inválido.", audioPuzzleError);
    }

    public void PuzzleExito()
    {
        Show("Identificación restaurada.", audioPuzzleExito);
    }

    public void GearIntro()
    {
        gearPasoActual = 0;
        CancelInvoke(nameof(MostrarSiguienteGear));
        MostrarSiguienteGear();
    }

    void MostrarSiguienteGear()
    {
        if (gearPasoActual >= gearTextos.Length)
            return;

        AudioClip audio = gearAudios[gearPasoActual];

        Show(gearTextos[gearPasoActual], audio);

        float duracion = audio != null ? audio.length + pausaEntreDialogos : duracionSinAudio;

        gearPasoActual++;
        Invoke(nameof(MostrarSiguienteGear), duracion);
    }

    public void GearError()
    {
        CancelInvoke(nameof(MostrarSiguienteGear));
        Show("Orden incorrecto.", audioGearError);
    }

    public void GearExito()
    {
        CancelInvoke(nameof(MostrarSiguienteGear));
        Show("Engranajes sincronizados.", audioGearExito);
    }

    public void FinalNivel()
    {
        CancelInvoke(nameof(MostrarSiguienteGear));
        Show("Zona estabilizada.\nBuen trabajo, operador.", audioFinalNivel);
    }
}