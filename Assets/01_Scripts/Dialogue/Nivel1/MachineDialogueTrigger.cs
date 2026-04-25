using UnityEngine;

public class MachineDialogueTrigger : MonoBehaviour
{
    public enum MachineDialogueType
    {
        Botones,
        Cables,
        Palancas,
        Puzzle,
        Gear
    }

    public MachineDialogueType type;
    public Transform player;
    public Transform interactionPoint;
    public float distance = 3f;
    public bool mostrarSoloUnaVez = true;

    private bool mostrado = false;
    private MonoBehaviour dialogue;

    void Start()
    {
        dialogue = FindAnyObjectByType<LevelOneDialogueController>();

        if (dialogue == null) dialogue = FindAnyObjectByType<LevelTwoDialogueController>();
        /*if (dialogue == null) dialogue = FindAnyObjectByType<LevelThreeDialogueController>();
        if (dialogue == null) dialogue = FindAnyObjectByType<LevelFourDialogueController>();
        if (dialogue == null) dialogue = FindAnyObjectByType<LevelFiveDialogueController>();*/
        Debug.Log("Dialogue encontrado: " + dialogue);
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    void Update()
    {
        if (player == null || dialogue == null) return;
        if (mostrarSoloUnaVez && mostrado) return;

        Vector3 point = interactionPoint != null ? interactionPoint.position : transform.position;
        float dist = Vector3.Distance(player.position, point);

        if (dist <= distance)
        {
            mostrado = true;

            switch (type)
            {
                case MachineDialogueType.Botones:
                    if (dialogue is LevelOneDialogueController d1) d1.BotonesIntro();
                    else if (dialogue is LevelTwoDialogueController d2) d2.BotonesIntro();
                    /*else if (dialogue is LevelThreeDialogueController d3) d3.BotonesIntro();
                    else if (dialogue is LevelFourDialogueController d4) d4.BotonesIntro();
                    else if (dialogue is LevelFiveDialogueController d5) d5.BotonesIntro();*/
                    break;

                case MachineDialogueType.Cables:
                    if (dialogue is LevelOneDialogueController e1) e1.CablesIntro();
                    else if (dialogue is LevelTwoDialogueController e2) e2.CablesIntro();
                    /*else if (dialogue is LevelThreeDialogueController d3) d3.BotonesIntro();
                    else if (dialogue is LevelFourDialogueController d4) d4.BotonesIntro();
                    else if (dialogue is LevelFiveDialogueController d5) d5.BotonesIntro();*/
                    break;

            
                case MachineDialogueType.Puzzle:
                    if (dialogue is LevelOneDialogueController f1) f1.PuzzleIntro();
                    else if (dialogue is LevelTwoDialogueController f2) f2.PuzzleIntro();
                    /*else if (dialogue is LevelThreeDialogueController d3) d3.BotonesIntro();
                    else if (dialogue is LevelFourDialogueController d4) d4.BotonesIntro();
                    else if (dialogue is LevelFiveDialogueController d5) d5.BotonesIntro();*/
                    break;
                case MachineDialogueType.Gear:
                    /*if (dialogue is LevelOneDialogueController f1) f1.PuzzleIntro();
                    else if (dialogue is LevelTwoDialogueController f2) f2.PuzzleIntro();
                    else if (dialogue is LevelThreeDialogueController d3) d3.BotonesIntro();
                    else if (dialogue is LevelFourDialogueController d4) d4.BotonesIntro();
                    else if (dialogue is LevelFiveDialogueController d5) d5.BotonesIntro();*/
                    break;
            }
        }
    }
}