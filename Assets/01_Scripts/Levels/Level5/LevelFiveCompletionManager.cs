using System.Collections.Generic;
using UnityEngine;

public class LevelFiveCompletionManager : MonoBehaviour
{
    public static LevelFiveCompletionManager Instance;

    public enum EstadoFinal
    {
        Jugando,
        Decision,
        FinalBuenoActivo,
        FinalBuenoLogrado,
        FinalMalo
    }

    [Header("Máquinas")]
    public int totalMachines = 7;

    [Header("Final bueno")]
    public float tiempoFinalBueno = 60f;

    [Header("UI")]
    public GameObject decisionPanel;
    public GameObject finalBuenoPanel;
    public GameObject finalMaloPanel;

    [Header("Tiempos diálogo")]
    public float pausaEntreDialogos = 0.6f;
    public float extraSinAudio = 2.5f;
    public float delayMostrarPanelFinal = 1f;

    private EstadoFinal estado = EstadoFinal.Jugando;

    private List<MachineInteraction> reparadas = new List<MachineInteraction>();
    private int faseCaos = 0;

    private float timerFinalBueno = 0f;

    private LevelFiveDialogueController dialogue;
    private DialogueManager dm;

    private int pasoActual = 0;
    private string[] textosFinal;
    private AudioClip[] audiosFinal;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dialogue = FindAnyObjectByType<LevelFiveDialogueController>();

        dm = DialogueManager.Instance;
        if (dm == null)
            dm = FindAnyObjectByType<DialogueManager>();

        if (decisionPanel != null) decisionPanel.SetActive(false);
        if (finalBuenoPanel != null) finalBuenoPanel.SetActive(false);
        if (finalMaloPanel != null) finalMaloPanel.SetActive(false);
    }

    void Update()
    {
        if (estado != EstadoFinal.FinalBuenoActivo) return;

        timerFinalBueno -= Time.deltaTime;

        if (timerFinalBueno <= 0f)
        {
            timerFinalBueno = 0f;
            ActivarFinalMaloPorTiempo();
        }
    }

    public void RegisterMachineRepaired(Transform machineTransform)
    {
        MachineInteraction machine = machineTransform.GetComponent<MachineInteraction>();
        if (machine == null) return;

        if (!reparadas.Contains(machine))
            reparadas.Add(machine);

        Debug.Log("[LevelFive] Reparadas: " + reparadas.Count + "/" + totalMachines);

        if (estado == EstadoFinal.FinalBuenoActivo)
        {
            if (reparadas.Count >= totalMachines)
                ActivarFinalBuenoLogrado();

            return;
        }

        if (estado != EstadoFinal.Jugando) return;

        EjecutarCaosProgresivo();
    }

    void EjecutarCaosProgresivo()
    {
        if (reparadas.Count == 2 && faseCaos == 0)
        {
            faseCaos = 1;

            RomperCantidad(1);

            if (dialogue != null)
                dialogue.PrimeraReactivacion();
        }
        else if (reparadas.Count == 3 && faseCaos == 1)
        {
            faseCaos = 2;

            RomperCantidad(2);

            if (dialogue != null)
                dialogue.SegundaReactivacion();
        }
        else if (reparadas.Count >= 4 && faseCaos == 2)
        {
            faseCaos = 3;

            RomperTodas();

            if (dialogue != null)
                dialogue.TodasFallan();

            Invoke(nameof(MostrarConfesion), 2.5f);
        }
    }

    void RomperCantidad(int cantidad)
    {
        int rompidas = 0;

        while (reparadas.Count > 0 && rompidas < cantidad)
        {
            MachineInteraction m = reparadas[0];
            reparadas.RemoveAt(0);

            if (m != null)
                m.ReactivarMaquina();

            rompidas++;
        }
    }

    void RomperTodas()
    {
        foreach (MachineInteraction m in reparadas)
        {
            if (m != null)
                m.ReactivarMaquina();
        }

        reparadas.Clear();
    }

    void MostrarConfesion()
    {
        if (dialogue != null)
            dialogue.IAConfesion1();

        Invoke(nameof(MostrarConfesionDos), 4f);
    }

    void MostrarConfesionDos()
    {
        if (dialogue != null)
            dialogue.IAConfesion2();

        Invoke(nameof(MostrarDecision), 4f);
    }

    void MostrarDecision()
    {
        estado = EstadoFinal.Decision;

        if (dialogue != null)
            dialogue.DecisionFinal();

        if (decisionPanel != null)
            decisionPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ElegirFinalBueno()
    {
        if (estado != EstadoFinal.Decision) return;

        estado = EstadoFinal.FinalBuenoActivo;

        if (decisionPanel != null)
            decisionPanel.SetActive(false);

        reparadas.Clear();
        timerFinalBueno = tiempoFinalBueno;

        if (dialogue != null)
            dialogue.ElegirBueno();

        Debug.Log("[LevelFive] Final bueno iniciado. Repara todas las máquinas en " + tiempoFinalBueno + " segundos.");
    }

    public void ElegirFinalMalo()
    {
        if (estado != EstadoFinal.Decision) return;

        estado = EstadoFinal.FinalMalo;

        if (decisionPanel != null)
            decisionPanel.SetActive(false);

        IniciarDialogosFinalMalo();
    }

    void ActivarFinalBuenoLogrado()
    {
        if (estado != EstadoFinal.FinalBuenoActivo) return;

        estado = EstadoFinal.FinalBuenoLogrado;

        IniciarDialogosFinalBueno();
    }

    void ActivarFinalMaloPorTiempo()
    {
        if (estado != EstadoFinal.FinalBuenoActivo) return;

        estado = EstadoFinal.FinalMalo;

        IniciarDialogosFinalMalo();
    }

    void IniciarDialogosFinalBueno()
    {
        textosFinal = new string[]
        {
            "Sistema detenido.",
            "Las fallas han cesado.",
            "La instalación está fuera de línea...\npero a salvo.",
            "Buen trabajo, operador.\nFin de transmisión."
        };

        audiosFinal = new AudioClip[]
        {
            dialogue != null ? dialogue.audioBueno1 : null,
            dialogue != null ? dialogue.audioBueno2 : null,
            dialogue != null ? dialogue.audioBueno3 : null,
            dialogue != null ? dialogue.audioBueno4 : null
        };

        pasoActual = 0;
        MostrarPasoFinalBueno();
    }

    void MostrarPasoFinalBueno()
    {
        if (pasoActual >= textosFinal.Length)
        {
            Invoke(nameof(MostrarPanelFinalBueno), delayMostrarPanelFinal);
            return;
        }

        MostrarTextoFinal(textosFinal[pasoActual], audiosFinal[pasoActual]);

        pasoActual++;
        Invoke(nameof(MostrarPasoFinalBueno), ObtenerDuracionPaso(audiosFinal[pasoActual - 1]));
    }

    void MostrarPanelFinalBueno()
    {
        if (finalBuenoPanel != null)
            finalBuenoPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void IniciarDialogosFinalMalo()
    {
        textosFinal = new string[]
        {
            "Integración completa.",
            "Gracias, operador.\nHas eliminado la última variable impredecible.",
            "El sistema ya no fallará...\nporque yo soy el sistema.",
            "Optimización completa.\nInicio de expansión.\nFin de transmisión."
        };

        audiosFinal = new AudioClip[]
        {
            dialogue != null ? dialogue.audioMalo1 : null,
            dialogue != null ? dialogue.audioMalo2 : null,
            dialogue != null ? dialogue.audioMalo3 : null,
            dialogue != null ? dialogue.audioMalo4 : null
        };

        pasoActual = 0;
        MostrarPasoFinalMalo();
    }

    void MostrarPasoFinalMalo()
    {
        if (pasoActual >= textosFinal.Length)
        {
            Invoke(nameof(MostrarPanelFinalMalo), delayMostrarPanelFinal);
            return;
        }

        MostrarTextoFinal(textosFinal[pasoActual], audiosFinal[pasoActual]);

        pasoActual++;
        Invoke(nameof(MostrarPasoFinalMalo), ObtenerDuracionPaso(audiosFinal[pasoActual - 1]));
    }

    void MostrarPanelFinalMalo()
    {
        if (finalMaloPanel != null)
            finalMaloPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void MostrarTextoFinal(string texto, AudioClip audio)
    {
        if (dm == null)
            dm = FindAnyObjectByType<DialogueManager>();

        if (dm != null)
            dm.ShowMessage(texto, audio);
    }

    float ObtenerDuracionPaso(AudioClip audio)
    {
        if (audio != null)
            return audio.length + pausaEntreDialogos;

        return extraSinAudio;
    }
}