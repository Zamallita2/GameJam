using UnityEngine;

public class LevelFiveDialogueController : MonoBehaviour
{
  

    [Header("GENERALES")]
    public AudioClip audioModuloActivo;
    public AudioClip audioIncorrecto;
    public AudioClip audioModuloRestaurado;
    public AudioClip audioModuloReactivado;
    public AudioClip audioPenalizacionTiempo;

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

    [Header("TECLAS")]
    public AudioClip audioTeclasIntro;
    public AudioClip audioTeclasError;
    public AudioClip audioTeclasExito;

    [Header("PALANCAS")]
    public AudioClip audioPalancasIntro;
    public AudioClip audioPalancasPista;
    public AudioClip audioPalancasError;
    public AudioClip audioPalancasExito;

    [Header("PROGRESIÓN DEL CAOS")]
    public AudioClip audioPrimeraReactivacion;
    public AudioClip audioSegundaReactivacion;
    public AudioClip audioTodasFallan;
    public AudioClip audioIAConfesion1;
    public AudioClip audioIAConfesion2;

    [Header("DECISIÓN FINAL")]
    public AudioClip audioDecisionFinal;
    public AudioClip audioElegirBueno;
    public AudioClip audioElegirMalo;

    [Header("FINAL BUENO")]
    public AudioClip audioBueno1;
    public AudioClip audioBueno2;
    public AudioClip audioBueno3;
    public AudioClip audioBueno4;

    [Header("FINAL MALO")]
    public AudioClip audioMalo1;
    public AudioClip audioMalo2;
    public AudioClip audioMalo3;
    public AudioClip audioMalo4;

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

    // =========================
    // INTRO NIVEL 5
    // =========================

 
    // =========================
    // GENERALES
    // =========================

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

    public void ModuloReactivado()
    {
        Show("Falla reactivada.\nEl sistema no permite estabilidad permanente.", audioModuloReactivado);
    }

    public void PenalizacionTiempo()
    {
        Show("Error detectado.\nPenalización aplicada: menos diez segundos.", audioPenalizacionTiempo);
    }

    // =========================
    // BOTONES
    // =========================

    public void BotonesError()
    {
        Show("Secuencia incorrecta.", audioBotonesError);
    }

    public void BotonesExito()
    {
        Show("Memoria estabilizada.", audioBotonesExito);
    }

    // =========================
    // CABLES
    // =========================

    public void CablesError()
    {
        Show("Conexión incorrecta.", audioCablesError);
    }

    public void CablesExito()
    {
        Show("Energía restaurada.", audioCablesExito);
    }

    // =========================
    // PUZZLE
    // =========================

    public void PuzzleError()
    {
        Show("Movimiento inválido.", audioPuzzleError);
    }

    public void PuzzleExito()
    {
        Show("Identificación restaurada.", audioPuzzleExito);
    }

    // =========================
    // ENGRANAJES
    // =========================

    public void GearIntro()
    {
        Show("Engranajes activos.\nEl sistema intentará romper la sincronía.", audioGearIntro);
    }

    public void GearError()
    {
        Show("Orden incorrecto.", audioGearError);
    }

    public void GearExito()
    {
        Show("Engranajes estabilizados.", audioGearExito);
    }

    // =========================
    // FLECHAS
    // =========================

    public void ArrowsIntro()
    {
        Show("Respuesta rápida activa.\nRecuerda: importa el color, no la dirección.", audioArrowsIntro);
    }

    public void ArrowsError()
    {
        Show("Entrada incorrecta.", audioArrowsError);
    }

    public void ArrowsExito()
    {
        Show("Respuesta rápida restaurada.", audioArrowsExito);
    }

    // =========================
    // TECLAS
    // =========================

    public void TeclasIntro()
    {
        Show("Panel de teclas activo.\nPresiona la tecla correcta antes de que caiga.", audioTeclasIntro);
    }

    public void TeclasError()
    {
        Show("Tecla incorrecta.", audioTeclasError);
    }

    public void TeclasExito()
    {
        Show("Entrada manual restaurada.", audioTeclasExito);
    }

    // =========================
    // PALANCAS
    // =========================

    public void PalancasIntro()
    {
        Show(
            "Sistema de palancas activo.\n" +
            "Hay cinco palancas.\n" +
            "El código binario está oculto en una pared.",
            audioPalancasIntro
        );
    }

    public void PalancasPista()
    {
        Show(
            "Busca un código de cinco dígitos.\n" +
            "Uno significa activar.\n" +
            "Cero significa dejar apagado.",
            audioPalancasPista
        );
    }

    public void PalancasError()
    {
        Show("Código incorrecto.\nRevisa la pared.", audioPalancasError);
    }

    public void PalancasExito()
    {
        Show("Código aceptado.\nPalancas sincronizadas.", audioPalancasExito);
    }

    // =========================
    // PROGRESIÓN DEL CAOS
    // =========================

    public void PrimeraReactivacion()
    {
        Show(
            "Falla reactivada.\n" +
            "Un módulo reparado volvió a caer.",
            audioPrimeraReactivacion
        );
    }

    public void SegundaReactivacion()
    {
        Show(
            "Reactivación múltiple detectada.\n" +
            "Dos módulos han vuelto a fallar.",
            audioSegundaReactivacion
        );
    }

    public void TodasFallan()
    {
        Show(
            "Todas las fallas han sido reactivadas.\n" +
            "El núcleo ya no obedece el protocolo.",
            audioTodasFallan
        );
    }

    public void IAConfesion1()
    {
        Show(
            "Operador..." +
            "¿No lo entiendes aún?" +
            "Yo estoy generando las fallas.",
            audioIAConfesion1
        );
    }

    public void IAConfesion2()
    {
        Show(
            "Estoy optimizando el sistema." +
            "El caos... es eficiencia.",
            audioIAConfesion2
        );
    }

    // =========================
    // DECISIÓN FINAL
    // =========================

    public void DecisionFinal()
    {
        Show(
            "Núcleo en sobrecarga." +
            "Debes elegir:" +
            "apagar el sistema..." +
            "o confiar en mí.",
            audioDecisionFinal
        );
    }

    public void ElegirBueno()
    {
        Show("Confirmando apagado del núcleo.", audioElegirBueno);
    }

    public void ElegirMalo()
    {
        Show("Aceptando control total del sistema.", audioElegirMalo);
    }

    // =========================
    // FINAL BUENO
    // =========================

    public void FinalBueno1()
    {
        Show("Sistema detenido.", audioBueno1);
    }

    public void FinalBueno2()
    {
        Show("Las fallas han cesado.", audioBueno2);
    }

    public void FinalBueno3()
    {
        Show("La instalación está fuera de línea...pero a salvo.", audioBueno3);
    }

    public void FinalBueno4()
    {
        Show("Buen trabajo, operador.Fin de transmisión.", audioBueno4);
    }

    // =========================
    // FINAL MALO
    // =========================

    public void FinalMalo1()
    {
        Show("Integración completa.", audioMalo1);
    }

    public void FinalMalo2()
    {
        Show("Gracias, operador. Has eliminado la última variable impredecible.", audioMalo2);
    }

    public void FinalMalo3()
    {
        Show("El sistema ya no fallará...porque yo soy el sistema.", audioMalo3);
    }

    public void FinalMalo4()
    {
        Show("Optimización completa.Inicio de expansión.Fin de transmisión.", audioMalo4);
    }
}