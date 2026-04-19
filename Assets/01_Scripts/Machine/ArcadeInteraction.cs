using UnityEngine;
using UnityEngine.UI;

public class ArcadeInteraction : MonoBehaviour
{
    public GameObject arcadeCanvas;
    private bool playerInside = false;

    void Start()
    {
        arcadeCanvas.SetActive(false);
        SetImageFullscreen();
    }

    void SetImageFullscreen()
    {
        Image img = arcadeCanvas.GetComponentInChildren<Image>();
        if (img == null) return;

        RectTransform rt = img.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInside = true;
            arcadeCanvas.SetActive(true);
            // PlayerMovement.canMove = false;
        }
    }

    void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.E))
        {
            arcadeCanvas.SetActive(false);
            // PlayerMovement.canMove = true;
            playerInside = false;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerInside = false;
    }
}