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

    [Header("Mano del panel (opcional)")]
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
        {
            if (mostrarLogs)
                Debug.LogWarning($"[MachineInteraction] Faltan referencias en {gameObject.name}");
            return;
        }

        Vector3 point = interactionPoint != null ? interactionPoint.position : transform.position;
        float dist = Vector3.Distance(player.transform.position, point);
        bool playerNear = dist <= interactionDistance;

        if (Input.GetKeyDown(interactKey) && mostrarLogs)
            Debug.Log($"[{gameObject.name}] Distancia = {dist} | Cerca = {playerNear}");

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

        if (mostrarLogs)
            Debug.Log($"[MachineInteraction] Activando panel: {gameObject.name}");

        if (panelRoot != null)
            panelRoot.SetActive(true);

        if (panelCamera != null)
            panelCamera.enabled = true;

        if (playerCamera != null)
            playerCamera.enabled = false;

        if (panelLoader != null)
            panelLoader.CargarPanel(machineType.tipo, machineType.nivel);

        // 🔥 ASIGNAR MACHINE OWNER A TODOS LOS PANELES
        SimonButtonsPanel simon = panelRoot.GetComponentInChildren<SimonButtonsPanel>(true);
        if (simon != null)
            simon.SetMachineOwner(this);

        CablesPanel cables = panelRoot.GetComponentInChildren<CablesPanel>(true);
        if (cables != null)
            cables.SetMachineOwner(this);

        Game puzzle = panelRoot.GetComponentInChildren<Game>(true);
        if (puzzle != null)
            puzzle.SetMachineOwner(this);

        // MANO
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

        if (mostrarLogs)
            Debug.Log($"[MachineInteraction] Desactivando panel: {gameObject.name}");

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

        // 🔥 APAGAR BRILLO
        if (machineGlow != null)
            machineGlow.StopGlow();

        // 🔥 ACTUALIZAR OBJETIVOS
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
            }
        }

        // 🔥 CONTADOR DE NIVEL
        LevelOneCompletionManager manager = LevelOneCompletionManager.Instance;

        if (manager == null)
            manager = FindAnyObjectByType<LevelOneCompletionManager>();

        if (manager != null)
            manager.RegisterMachineRepaired();

        Debug.Log($"[MachineInteraction] Máquina reparada: {gameObject.name}");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 point = interactionPoint != null ? interactionPoint.position : transform.position;
        Gizmos.DrawWireSphere(point, interactionDistance);
    }
}