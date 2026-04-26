using UnityEngine;

public class SimpleHelpMenu : MonoBehaviour
{
    public enum HelpState
    {
        Cerrado,
        Principal,
        Controles,
        Tutoriales
    }

    [Header("Paneles")]
    public GameObject panelAyuda;
    public GameObject panelControles;
    public GameObject panelTutoriales;

    [Header("Cursor")]
    public bool bloquearCursorAlCerrar = false;

    private HelpState estadoActual = HelpState.Cerrado;

    void Start()
    {
        OcultarPaneles();
        estadoActual = HelpState.Cerrado;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (estadoActual == HelpState.Controles || estadoActual == HelpState.Tutoriales)
                VolverAyudaPrincipal();
            else if (estadoActual == HelpState.Principal)
                CerrarTodo();
        }
    }

    public void AbrirAyuda()
    {
        estadoActual = HelpState.Principal;

        if (panelAyuda != null)
            panelAyuda.SetActive(true);

        if (panelControles != null)
            panelControles.SetActive(false);

        if (panelTutoriales != null)
            panelTutoriales.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("[SimpleHelpMenu] Ayuda abierta");
    }

    public void MostrarControles()
    {
        estadoActual = HelpState.Controles;

        if (panelAyuda != null)
            panelAyuda.SetActive(false);

        if (panelControles != null)
            panelControles.SetActive(true);

        if (panelTutoriales != null)
            panelTutoriales.SetActive(false);
    }

    public void MostrarTutoriales()
    {
        estadoActual = HelpState.Tutoriales;

        if (panelAyuda != null)
            panelAyuda.SetActive(false);

        if (panelControles != null)
            panelControles.SetActive(false);

        if (panelTutoriales != null)
            panelTutoriales.SetActive(true);
    }

    public void VolverAyudaPrincipal()
    {
        estadoActual = HelpState.Principal;

        if (panelAyuda != null)
            panelAyuda.SetActive(true);

        if (panelControles != null)
            panelControles.SetActive(false);

        if (panelTutoriales != null)
            panelTutoriales.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CerrarTodo()
    {
        estadoActual = HelpState.Cerrado;

        OcultarPaneles();

        if (bloquearCursorAlCerrar)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OcultarPaneles()
    {
        if (panelAyuda != null)
            panelAyuda.SetActive(false);

        if (panelControles != null)
            panelControles.SetActive(false);

        if (panelTutoriales != null)
            panelTutoriales.SetActive(false);
    }
}