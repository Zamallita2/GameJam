using UnityEngine;

public class HandManager : MonoBehaviour
{
    public Transform hand;          // La mano
    public Transform pointer;       // El dedito (surface point)
    public Camera cam;

    public float distanciaNormal = 5f;
    public float distanciaClick = 8f;
    public float velocidad = 10f;

    private float distanciaActual;
    private bool activo = false;

    void Start()
    {
        distanciaActual = distanciaNormal;
    }

    public void Activar()
    {
        activo = true;
        gameObject.SetActive(true);
    }

    public void Desactivar()
    {
        activo = false;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!activo) return;

        // Detectar click
        if (Input.GetMouseButton(0))
            distanciaActual = Mathf.Lerp(distanciaActual, distanciaClick, Time.deltaTime * velocidad);
        else
            distanciaActual = Mathf.Lerp(distanciaActual, distanciaNormal, Time.deltaTime * velocidad);

        // Posición del mouse con profundidad dinámica
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = distanciaActual;

        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
        worldPos.y-=0.5f;
        worldPos.x+=0.05f;

        // Mover el dedito
        pointer.position = worldPos;

        // Opcional: que la mano siga al dedito
        hand.position = pointer.position;
    }
}