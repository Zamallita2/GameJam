using UnityEngine;

public class PlayerModeSwitcher : MonoBehaviour
{
    public GameObject thirdPersonPlayer;
    public GameObject firstPersonPlayer;

    private bool isFirstPerson = false;

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
        Vector3 pos = thirdPersonPlayer.transform.position;

        firstPersonPlayer.transform.position = pos;
        firstPersonPlayer.transform.rotation = thirdPersonPlayer.transform.rotation;

        firstPersonPlayer.SetActive(true);
        thirdPersonPlayer.SetActive(false);

        FirstPersonController fp = firstPersonPlayer.GetComponent<FirstPersonController>();
        if (fp != null)
            fp.ResetVelocity();
    }

    void ActivateThirdPerson()
    {
        Vector3 pos = firstPersonPlayer.transform.position;

        // 🔥 ajuste inverso
        pos.y -= 1.0f;

        thirdPersonPlayer.transform.position = pos;
        thirdPersonPlayer.transform.rotation = firstPersonPlayer.transform.rotation;

        thirdPersonPlayer.SetActive(true);
        firstPersonPlayer.SetActive(false);

    }
}