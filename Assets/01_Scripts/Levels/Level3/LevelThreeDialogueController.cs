using UnityEngine;

public class LevelThreeDialogueController : MonoBehaviour
{
    [Header("Audios de máquinas")]
    public AudioClip audioTutorial;
    public AudioClip audioAlerta;

    public AudioClip audioBotonesIntro;
    public AudioClip audioBotonesDurante;
    public AudioClip audioBotonesError;
    public AudioClip audioBotonesExito;

    public AudioClip audioCablesIntro;
    public AudioClip audioCablesDurante;
    public AudioClip audioCablesError;
    public AudioClip audioCablesExito;


    public AudioClip audioPuzzleIntro;
    public AudioClip audioPuzzleDurante;
    public AudioClip audioPuzzleError;
    public AudioClip audioPuzzleExito;

    public AudioClip audioTiempoCritico;
    public AudioClip audioNucleoSobrecarga;
    public AudioClip audioFinalNivel;

    private DialogueManager dm;

    void Awake()
    {
        dm = DialogueManager.Instance;
        if (dm == null)
            dm = FindAnyObjectByType<DialogueManager>();
    }

    void Show(string texto, AudioClip audio)
    {
        if (dm == null)
            dm = FindAnyObjectByType<DialogueManager>();

        if (dm != null)
            dm.ShowMessage(texto, audio);
    }

    public void Tutorial()
    {
        Show("Utiliza el entorno para localizar las fallas.\nAcércate a una máquina dañada.\nCuando estés cerca, presiona la tecla E para interactuar.\nTrabaja rápido. El tiempo es limitado.", audioTutorial);
    }

    public void Alerta()
    {
        Show("Señal de falla detectada.", audioAlerta);
    }

    public void BotonesIntro()
    {
        Show("Panel de memoria dañado.Debes observar la secuencia de luces y repetirla exactamente.", audioBotonesIntro);
    }

    public void BotonesDurante()
    {
        Show("Memoriza el patrón...", audioBotonesDurante);
    }

    public void BotonesError()
    {
        Show("Error en la secuencia.Reiniciando patrón.", audioBotonesError);
    }

    public void BotonesExito()
    {
        Show("Secuencia correcta.Módulo de memoria restaurado.", audioBotonesExito);
    }

    public void CablesIntro()
    {
        Show("Sistema de energía inestable.Debes reconectar los cables correctamente.Cada color debe coincidir.", audioCablesIntro);
    }

    public void CablesDurante()
    {
        Show("Restableciendo flujo energético...", audioCablesDurante);
    }

    public void CablesError()
    {
        Show("Conexión incorrecta.El flujo sigue inestable.", audioCablesError);
    }

    public void CablesExito()
    {
        Show("Conexiones estabilizadas.Energía restaurada.", audioCablesExito);
    }



    public void PuzzleIntro()
    {
        Show("Módulo de identificación visual dañado.La imagen del operador principal fue fragmentada. Reconstruye el retrato moviendo las piezas hasta completar la imagen.", audioPuzzleIntro);
    }

    public void PuzzleDurante()
    {
        Show("Desliza las fichas hacia el espacio vacío.Restaura la imagen original para validar el sistema.", audioPuzzleDurante);
    }

    public void PuzzleError()
    {
        Show("Movimiento inválido. Solo puedes mover fichas junto al espacio vacío.", audioPuzzleError);
    }

    public void PuzzleExito()
    {
        Show("Imagen reconstruida. Identificación visual restaurada.", audioPuzzleExito);
    }
    /*public void GearIntro()
    {
        Show("Módulo de identificación visual dañado.La imagen del operador principal fue fragmentada. Reconstruye el retrato moviendo las piezas hasta completar la imagen.", audioPuzzleIntro);
    }

    public void GearExito()
    {
        Show("Imagen reconstruida. Identificación visual restaurada.", audioPuzzleExito);
    }*/

    public void TiempoCritico()
    {
        Show("Tiempo crítico. Aumenta la velocidad de reparación.", audioTiempoCritico);
    }

    public void NucleoSobrecarga()
    {
        Show("Advertencia. El núcleo se está sobrecargando.", audioNucleoSobrecarga);
    }

    public void FinalNivel()
    {
        Show("Todos los módulos han sido restaurados.El sistema vuelve a estar estable. Excelente trabajo, operador.Preparando siguiente zona...", audioFinalNivel);
    }
}