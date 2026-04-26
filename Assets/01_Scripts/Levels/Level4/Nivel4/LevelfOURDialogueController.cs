using UnityEngine;

public class LevelFourDialogueController : MonoBehaviour
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

    [Header("ENGRANAJES")]
    public AudioClip audioGearIntro;
    public AudioClip audioGearError;
    public AudioClip audioGearExito;

    [Header("FLECHAS / RESPUESTA RÁPIDA")]
    public AudioClip audioArrowsIntro;
    public AudioClip audioArrowsError;
    public AudioClip audioArrowsExito;

    private DialogueManager dm;

    void Awake()
    {
        dm = DialogueManager.Instance;
        if (dm == null)
            dm = FindAnyObjectByType<DialogueManager>();
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
        Show("Engranajes activos.\nEl orden será más difícil.", audioGearIntro);
    }

    public void GearError()
    {
        Show("Orden incorrecto.", audioGearError);
    }

    public void GearExito()
    {
        Show("Engranajes estabilizados.", audioGearExito);
    }

    public void ArrowsIntro()
    {
        Show(
            "Sistema de respuesta rápida activo.\nPresiona el botón correcto antes de que el símbolo caiga.",
            audioArrowsIntro
        );
    }

    public void ArrowsError()
    {
        Show("Entrada incorrecta.", audioArrowsError);
    }

    public void ArrowsExito()
    {
        Show("Respuesta rápida restaurada.", audioArrowsExito);
    }
}