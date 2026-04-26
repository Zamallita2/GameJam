using UnityEngine;

public class HelpMenuManager : MonoBehaviour
{
    public enum HelpState
    {
        Cerrado,
        Principal,
        Controles,
        Tutoriales
    }

    [Header("Paneles")]
    public GameObject menuAyuda;
    public GameObject menuControles;
    public GameObject menuTutoriales;

    [Header("Tiempo")]
    public TimeManager timeManager;

    [Header("Cámara")]
    public PlayerCameraController cameraController;

    private HelpState estadoActual = HelpState.Cerrado;

    void Start()
    {
        CerrarTodo();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (estadoActual == HelpState.Cerrado)
                AbrirMenu();
            else
                CerrarTodo();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estadoActual == HelpState.Controles || estadoActual == HelpState.Tutoriales)
            {
                VolverMenuPrincipal();
            }
            else if (estadoActual == HelpState.Principal)
            {
                CerrarTodo();
            }
        }
    }

    public void AbrirMenu()
    {
        estadoActual = HelpState.Principal;

        menuAyuda.SetActive(true);
        menuControles.SetActive(false);
        menuTutoriales.SetActive(false);

        if (timeManager != null)
            timeManager.timerPausado = true;

        if (cameraController != null)
            cameraController.cameraPaused = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void MostrarControles()
    {
        estadoActual = HelpState.Controles;

        menuAyuda.SetActive(false);
        menuControles.SetActive(true);
        menuTutoriales.SetActive(false);
    }

    public void MostrarTutoriales()
    {
        estadoActual = HelpState.Tutoriales;

        menuAyuda.SetActive(false);
        menuControles.SetActive(false);
        menuTutoriales.SetActive(true);
    }

    public void VolverMenuPrincipal()
    {
        estadoActual = HelpState.Principal;

        menuAyuda.SetActive(true);
        menuControles.SetActive(false);
        menuTutoriales.SetActive(false);
    }

    public void CerrarTodo()
    {
        estadoActual = HelpState.Cerrado;

        if (menuAyuda != null) menuAyuda.SetActive(false);
        if (menuControles != null) menuControles.SetActive(false);
        if (menuTutoriales != null) menuTutoriales.SetActive(false);

        if (timeManager != null)
            timeManager.timerPausado = false;

        if (cameraController != null)
            cameraController.cameraPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}