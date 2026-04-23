using UnityEngine;

public class PlayerVisualSwitcher : MonoBehaviour
{
    public PlayerCameraController cameraController;

    public GameObject thirdPersonBody;
    public GameObject firstPersonArms;

    void Update()
    {
        bool isThirdPerson = cameraController.IsThirdPerson();

        thirdPersonBody.SetActive(isThirdPerson);
        firstPersonArms.SetActive(!isThirdPerson);
    }
}