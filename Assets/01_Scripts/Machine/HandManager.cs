using UnityEngine;

public class HandManager : MonoBehaviour
{
    public Transform hand;
    public Transform pointer;
    public Camera cam;

    public float distanciaNormal = 2f;
    public float distanciaClick = 3f;
    public float velocidad = 10f;

    private float distanciaActual;
    private bool activo = false;

    void Start()
    {
        distanciaActual = distanciaNormal;
        gameObject.SetActive(false);
    }

    public void Activar(Camera nuevaCam, Transform nuevaHand, Transform nuevoPointer)
    {
        cam = nuevaCam;
        hand = nuevaHand;
        pointer = nuevoPointer;

        activo = true;
        distanciaActual = distanciaNormal;
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
        if (cam == null || hand == null || pointer == null) return;

        if (Input.GetMouseButton(0))
            distanciaActual = Mathf.Lerp(distanciaActual, distanciaClick, Time.deltaTime * velocidad);
        else
            distanciaActual = Mathf.Lerp(distanciaActual, distanciaNormal, Time.deltaTime * velocidad);

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = distanciaActual;

        Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);

        pointer.position = worldPos;
        hand.position = pointer.position;
    }
}