using UnityEngine;

public class HandManager : MonoBehaviour
{
    public Transform hand;
    public GameObject handObj;
    public Transform pointer;
    public Camera cam;

    public float distanciaNormal = 2f;
    public float distanciaClick = 3f;
    public float possY = 0.3f;
    public float possX = 0.1f;
    
    public float velocidad = 10f;

    private float distanciaActual;
    private bool activo = false;

    void Start()
    {
        distanciaActual = distanciaNormal;
        gameObject.SetActive(false);
    }

    public void Activar(Camera nuevaCam, GameObject nuevaHand, Transform nuevoPointer)
    {
        cam = nuevaCam;
        handObj = nuevaHand;
        handObj.SetActive(true);
        hand = handObj.transform;
        pointer = nuevoPointer;

        activo = true;
        distanciaActual = distanciaNormal;
        gameObject.SetActive(true);
    }

    public void Desactivar()
    {
        activo = false;
        handObj.SetActive(false);
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

        // derecha según la cámara (puede ser X o Z dependiendo de rotación)
        Vector3 right = cam.transform.right;

        // opcional: ignorar inclinación vertical
        right.y = 0f;
        right.Normalize();

        // offset
        worldPos += right * possX;
        worldPos += Vector3.down * possY;

        pointer.position = worldPos;
        hand.position = pointer.position;
    }
}