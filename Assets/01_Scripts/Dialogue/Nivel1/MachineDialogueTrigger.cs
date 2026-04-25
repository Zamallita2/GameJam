using UnityEngine;

public class MachineDialogueTrigger : MonoBehaviour
{
    public enum MachineDialogueType
    {
        Botones,
        Cables,
        Puzzle,
        Gear
    }

    [Header("Tipo de diálogo")]
    public MachineDialogueType type;

    [Header("Referencias")]
    public Transform player;
    public Transform interactionPoint;

    [Header("Configuración")]
    public float distance = 3f;
    public bool mostrarSoloUnaVez = true;
    public bool mostrarLogs = false;

    private bool mostrado = false;

    private LevelOneDialogueController levelOneDialogue;
    private LevelTwoDialogueController levelTwoDialogue;

    void Start()
    {
        BuscarDialogueController();
        BuscarPlayer();
    }

    void BuscarDialogueController()
    {
        levelOneDialogue = FindAnyObjectByType<LevelOneDialogueController>();
        levelTwoDialogue = FindAnyObjectByType<LevelTwoDialogueController>();
    }

    void BuscarPlayer()
    {
        if (player != null) return;

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
    }

    void Update()
    {
        if (player == null)
        {
            BuscarPlayer();
            return;
        }

        if (mostrarSoloUnaVez && mostrado) return;

        Vector3 point = interactionPoint != null ? interactionPoint.position : transform.position;
        float dist = Vector3.Distance(player.position, point);

        if (dist <= distance)
        {
            mostrado = true;
            EjecutarDialogo();
        }
    }

    void EjecutarDialogo()
    {
        if (levelTwoDialogue != null)
        {
            EjecutarNivelDos();
            return;
        }

        if (levelOneDialogue != null)
        {
            EjecutarNivelUno();
            return;
        }

        if (mostrarLogs)
            Debug.LogWarning("[MachineDialogueTrigger] No hay controlador de diálogos.");
    }

    void EjecutarNivelUno()
    {
        switch (type)
        {
            case MachineDialogueType.Botones:
                levelOneDialogue.BotonesIntro();
                break;

            case MachineDialogueType.Cables:
                levelOneDialogue.CablesIntro();
                break;

            case MachineDialogueType.Puzzle:
                levelOneDialogue.PuzzleIntro();
                break;

            case MachineDialogueType.Gear:
                if (mostrarLogs)
                    Debug.LogWarning("[MachineDialogueTrigger] Gear no existe en Nivel 1.");
                break;
        }
    }

    void EjecutarNivelDos()
    {
        switch (type)
        {
            case MachineDialogueType.Botones:
            case MachineDialogueType.Cables:
            case MachineDialogueType.Puzzle:
                levelTwoDialogue.ModuloActivo();
                break;

            case MachineDialogueType.Gear:
                levelTwoDialogue.GearIntro();
                break;
        }
    }

    public void ResetTrigger()
    {
        mostrado = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 point = interactionPoint != null ? interactionPoint.position : transform.position;
        Gizmos.DrawWireSphere(point, distance);
    }
}