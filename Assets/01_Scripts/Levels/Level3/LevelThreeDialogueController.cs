using UnityEngine;

public class LevelThreeDialogueController : MonoBehaviour
{
    [Header("Audios generales")]
    public AudioClip audioTutorial;
    public AudioClip audioAlerta;
    public AudioClip audioTiempoCritico;
    public AudioClip audioNucleoSobrecarga;

    [Header("BOTONES")]
    public AudioClip audioBotonesIntro;
    public AudioClip audioBotonesError;
    public AudioClip audioBotonesExito;

    [Header("CABLES")]
    public AudioClip audioCablesIntro;
    public AudioClip audioCablesError;
    public AudioClip audioCablesExito;

    [Header("PUZZLE")]
    public AudioClip audioPuzzleIntro;
    public AudioClip audioPuzzleError;
    public AudioClip audioPuzzleExito;

    [Header("GEAR / ENGRANAJES")]
    public AudioClip audioGearIntro;
    public AudioClip audioGearError;
    public AudioClip audioGearExito;

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

    public void Tutorial()
    {
        Show("Trabaja rápido.\nEl sistema no esperará.", audioTutorial);
    }

    public void Alerta()
    {
        Show("Inestabilidad detectada.", audioAlerta);
    }

    public void TiempoCritico()
    {
        Show("Tiempo crítico.", audioTiempoCritico);
    }

    public void NucleoSobrecarga()
    {
        Show("Carga del núcleo aumentando.", audioNucleoSobrecarga);
    }

    public void BotonesIntro()
    {
        Show("Módulo de memoria activo.", audioBotonesIntro);
    }

    public void BotonesError()
    {
        Show("Secuencia incorrecta.", audioBotonesError);
    }

    public void BotonesExito()
    {
        Show("Memoria estabilizada.", audioBotonesExito);
    }

    public void CablesIntro()
    {
        Show("Módulo de energía activo.", audioCablesIntro);
    }

    public void CablesError()
    {
        Show("Conexión incorrecta.", audioCablesError);
    }

    public void CablesExito()
    {
        Show("Energía restaurada.", audioCablesExito);
    }

    public void PuzzleIntro()
    {
        Show("Módulo visual activo.", audioPuzzleIntro);
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
        Show("Engranajes activos.\nLa secuencia será más rápida.", audioGearIntro);
    }

    public void GearError()
    {
        Show("Orden incorrecto.", audioGearError);
    }

    public void GearExito()
    {
        Show("Engranajes estabilizados.", audioGearExito);
    }
}