using UnityEngine;

public class PlayerModeSwitcher : MonoBehaviour
{
    public GameObject thirdPersonPlayer;
    public GameObject firstPersonPlayer;

    private bool isFirstPerson = false;
    public PlayerMovement playerMovement;

    void Start()
    {
        ActivateThirdPerson();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;

            if (isFirstPerson)
                ActivateFirstPerson();
            else
                ActivateThirdPerson();
        }
    }

    void ActivateFirstPerson()
    {

        firstPersonPlayer.SetActive(true);
        thirdPersonPlayer.SetActive(false);

        playerMovement.SetFirstPerson(true);
    }

    void ActivateThirdPerson()
    {
        thirdPersonPlayer.SetActive(true);
        firstPersonPlayer.SetActive(false);

        playerMovement.SetFirstPerson(false);
    }
}