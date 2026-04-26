using UnityEngine;

public class MachineInteraction : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject panelRoot;
    public Camera panelCamera;
    public Camera playerCamera;
    public GameObject player;
    public GameObject arms;

    [Header("Punto real de interacción")]
    public Transform interactionPoint;

    [Header("Mano del panel")]
    public GameObject panelHand;
    public Transform panelPointer;

    [Header("Configuración")]
    public KeyCode interactKey = KeyCode.E;
    public float interactionDistance = 3f;
    public bool mostrarLogs = true;

    private bool panelActivo = false;
    private bool maquinaReparada = false;

    private MachineType machineType;
    private PanelLoader panelLoader;
    private HandManager handManager;
    private PlayerPanelVisibility playerPanelVisibility;
    private MachineGlow machineGlow;

    void Awake()
    {
        machineType = GetComponent<MachineType>();
        machineGlow = GetComponent<MachineGlow>();
        AutoAssignReferences();
    }

    void Start()
    {
        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (panelCamera != null)
            panelCamera.enabled = false;

        if (playerCamera != null)
            playerCamera.enabled = true;
    }

    void AutoAssignReferences()
    {
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

        if (panelRoot != null && panelLoader == null)
            panelLoader = panelRoot.GetComponentInChildren<PanelLoader>(true);

        if (handManager == null)
            handManager = FindFirstObjectByType<HandManager>(FindObjectsInactive.Include);

        if (playerPanelVisibility == null)
            playerPanelVisibility = FindFirstObjectByType<PlayerPanelVisibility>(FindObjectsInactive.Include);
    }

    void Update()
    {
        if (maquinaReparada) return;

        if (player == null || panelRoot == null || panelCamera == null || playerCamera == null || machineType == null)
            return;

        Vector3 point = interactionPoint != null ? interactionPoint.position : transform.position;
        float dist = Vector3.Distance(player.transform.position, point);
        bool playerNear = dist <= interactionDistance;

        if (playerNear && Input.GetKeyDown(interactKey) && !panelActivo)
            ActivarPanel();

        if (panelActivo && Input.GetKeyDown(KeyCode.Escape))
            DesactivarPanel();
    }

    void ActivarPanel()
    {
        panelActivo = true;

        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (panelCamera != null)
            panelCamera.enabled = true;

        if (playerCamera != null)
            playerCamera.enabled = false;

        if (panelLoader != null)
            panelLoader.CargarPanel(machineType.tipo, machineType.nivel);

        SimonButtonsPanel simon = panelRoot.GetComponentInChildren<SimonButtonsPanel>(true);
        if (simon != null)
            simon.SetMachineOwner(this);

        CablesPanel cables = panelRoot.GetComponentInChildren<CablesPanel>(true);
        if (cables != null)
            cables.SetMachineOwner(this);

        Game puzzle = panelRoot.GetComponentInChildren<Game>(true);
        if (puzzle != null)
            puzzle.SetMachineOwner(this);

        FastInputPanelController arrows = panelRoot.GetComponentInChildren<FastInputPanelController>(true);
        if (arrows != null)
            arrows.SetMachineOwner(this);

        if (handManager != null)
        {
            if (panelHand != null && panelPointer != null)
                handManager.Activar(panelCamera, panelHand, panelPointer);
            else
                handManager.Desactivar();
        }

        if (playerPanelVisibility != null)
            playerPanelVisibility.HideForPanel();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (player != null)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = false;
        }
    }

    void DesactivarPanel()
    {
        panelActivo = false;

        if (panelLoader != null)
            panelLoader.LimpiarPanelActual();

        if (panelCamera != null)
            panelCamera.enabled = false;

        if (playerCamera != null)
            playerCamera.enabled = true;

        if (panelRoot != null)
            panelRoot.SetActive(false);

        if (handManager != null)
            handManager.Desactivar();

        if (playerPanelVisibility != null)
            playerPanelVisibility.ShowAfterPanel();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (player != null)
        {
            PlayerMovement movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
                movement.enabled = true;
        }
    }

    public void CerrarPanelDesdeMinijuego()
    {
        DesactivarPanel();
    }

    public void MarcarMaquinaReparada()
    {
        if (maquinaReparada) return;

        maquinaReparada = true;

        if (machineGlow != null)
            machineGlow.StopGlow();

        ObjectivesHUD hud = ObjectivesHUD.Instance;

        if (hud == null)
            hud = FindAnyObjectByType<ObjectivesHUD>();

        if (hud != null && machineType != null)
        {
            switch (machineType.tipo)
            {
                case MachineType.TipoMaquina.Botones:
                    hud.CompleteBotones();
                    break;

                case MachineType.TipoMaquina.Cables:
                    hud.CompleteCables();
                    break;

                case MachineType.TipoMaquina.Puzzle:
                    hud.CompletePuzzle();
                    break;

                case MachineType.TipoMaquina.Gear:
                    hud.CompleteGear();
                    break;

                case MachineType.TipoMaquina.Arrows:
                    hud.CompleteArrows();
                    break;
            }
        }

        LevelOneCompletionManager m1 = FindAnyObjectByType<LevelOneCompletionManager>();
        if (m1 != null)
        {
            m1.RegisterMachineRepaired(transform);
            return;
        }

        LevelTwoCompletionManager m2 = FindAnyObjectByType<LevelTwoCompletionManager>();
        if (m2 != null)
        {
            m2.RegisterMachineRepaired(transform);
            return;
        }

        LevelThreeCompletionManager m3 = FindAnyObjectByType<LevelThreeCompletionManager>();
        if (m3 != null)
        {
            m3.RegisterMachineRepaired(transform);
            return;
        }

        LevelFourCompletionManager m4 = FindAnyObjectByType<LevelFourCompletionManager>();
        if (m4 != null)
        {
            m4.RegisterMachineRepaired(transform);
            return;
        }

        Debug.LogWarning("[MachineInteraction] No se encontró manager de nivel.");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 point = interactionPoint != null ? interactionPoint.position : transform.position;
        Gizmos.DrawWireSphere(point, interactionDistance);
    }
}