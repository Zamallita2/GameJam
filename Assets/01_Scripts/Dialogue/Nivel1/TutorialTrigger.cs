using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    private PlayerMovement player;
    private LevelOneDialogueController dialogue;

    private bool tutorialMostrado = false;

    void Start()
    {
        player = FindAnyObjectByType<PlayerMovement>();
        dialogue = FindAnyObjectByType<LevelOneDialogueController>();
    }

    void Update()
    {
        if (tutorialMostrado) return;
        if (player == null || dialogue == null) return;

        // Detecta si el jugador se está moviendo
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            tutorialMostrado = true;

            Debug.Log("[Tutorial] Jugador empezó a moverse");

            dialogue.Tutorial();
        }
    }
}