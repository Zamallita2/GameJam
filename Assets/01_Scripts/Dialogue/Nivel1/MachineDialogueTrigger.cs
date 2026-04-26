using UnityEngine;

public class MachineDialogueTrigger : MonoBehaviour
{
    public enum MachineDialogueType
    {
        Botones,
        Cables,
        Puzzle,
        Gear,
        Arrows
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
    private LevelThreeDialogueController levelThreeDialogue;
    private LevelFourDialogueController levelFourDialogue;

    void Start()
    {
        BuscarDialogueController();
        BuscarPlayer();
    }

    void BuscarDialogueController()
    {
        levelOneDialogue = FindAnyObjectByType<LevelOneDialogueController>();
        levelTwoDialogue = FindAnyObjectByType<LevelTwoDialogueController>();
        levelThreeDialogue = FindAnyObjectByType<LevelThreeDialogueController>();
        levelFourDialogue = FindAnyObjectByType<LevelFourDialogueController>();
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
        if (levelFourDialogue != null)
        {
            EjecutarNivelCuatro();
            return;
        }

        if (levelThreeDialogue != null)
        {
            EjecutarNivelTres();
            return;
        }

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

    void EjecutarNivelTres()
    {
        switch (type)
        {
            case MachineDialogueType.Botones:
                levelThreeDialogue.BotonesIntro();
                break;

            case MachineDialogueType.Cables:
                levelThreeDialogue.CablesIntro();
                break;

            case MachineDialogueType.Puzzle:
                levelThreeDialogue.PuzzleIntro();
                break;

            case MachineDialogueType.Gear:
                levelThreeDialogue.GearIntro();
                break;
        }
    }

    void EjecutarNivelCuatro()
    {
        switch (type)
        {
            case MachineDialogueType.Botones:
            case MachineDialogueType.Cables:
            case MachineDialogueType.Puzzle:
                levelFourDialogue.ModuloActivo();
                break;

            case MachineDialogueType.Gear:
                levelFourDialogue.GearIntro();
                break;

            case MachineDialogueType.Arrows:
                levelFourDialogue.ArrowsIntro();
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