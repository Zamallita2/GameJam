using UnityEngine;

public class MachineDialogueTrigger : MonoBehaviour
{
    public enum MachineDialogueType
    {
        Botones,
        Cables,
        Palancas,
        Puzzle
    }

    public MachineDialogueType type;
    public Transform player;
    public Transform interactionPoint;
    public float distance = 3f;
    public bool mostrarSoloUnaVez = true;

    private bool mostrado = false;
    private LevelOneDialogueController dialogue;

    void Start()
    {
        dialogue = FindAnyObjectByType<LevelOneDialogueController>();

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
                    dialogue.BotonesIntro();
                    break;

                case MachineDialogueType.Cables:
                    dialogue.CablesIntro();
                    break;

            
                case MachineDialogueType.Puzzle:
                    dialogue.PuzzleIntro();
                    break;
            }
        }
    }
}