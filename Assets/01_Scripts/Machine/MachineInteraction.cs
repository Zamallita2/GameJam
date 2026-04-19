using UnityEngine;

public class MachineInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelRoot;
    public Camera panelCamera;
    public Camera playerCamera;
    public GameObject player;

    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionDistance = 3f;

    private bool panelActivo = false;
    private MachineType machineType;
    private PanelLoader panelLoader;

    void Awake()
    {
        machineType = GetComponent<MachineType>();
        AutoAssignReferences();
    }

    void AutoAssignReferences()
    {
        if (panelRoot == null)
        {
            GameObject foundPanel = GameObject.Find("PanelRoot");
            if (foundPanel != null)
                panelRoot = foundPanel;
        }

        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer;
        }

        if (playerCamera == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
                playerCamera = mainCam;
        }

        if (panelRoot != null)
        {
            if (panelCamera == null)
                panelCamera = panelRoot.GetComponentInChildren<Camera>(true);

            if (panelLoader == null)
                panelLoader = panelRoot.GetComponentInChildren<PanelLoader>(true);
        }
    }

    void Update()
    {
        if (player == null || panelRoot == null || panelCamera == null || playerCamera == null || machineType == null)
            return;

        float dist = Vector3.Distance(player.transform.position, transform.position);
        bool playerNear = dist <= interactionDistance;

        if (playerNear && Input.GetKeyDown(interactKey) && !panelActivo)
        {
            ActivarPanel();
        }

        if (panelActivo && Input.GetKeyDown(KeyCode.Escape))
        {
            DesactivarPanel();
        }
    }

    void ActivarPanel()
    {
        panelActivo = true;

        panelRoot.SetActive(true);
        panelCamera.gameObject.SetActive(true);
        playerCamera.gameObject.SetActive(false);

        if (panelLoader != null)
            panelLoader.CargarPanel(machineType.tipo, machineType.nivel);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;
    }

    void DesactivarPanel()
    {
        panelActivo = false;

        if (panelLoader != null)
            panelLoader.LimpiarPanelActual();

        if (panelCamera != null)
            panelCamera.gameObject.SetActive(false);

        if (playerCamera != null)
            playerCamera.gameObject.SetActive(true);

        if (panelRoot != null)
            panelRoot.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = true;
    }
}