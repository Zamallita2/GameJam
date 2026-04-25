using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float lookSpeed = 2f;

    private float rotX = 0f;
    private float rotY = 0f;

    void Start()
    {
        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;
    }

    void Update()
    {
        // Mover con WASD
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        transform.Translate(new Vector3(h, 0f, v) * moveSpeed * Time.deltaTime);

        // Rotar con click derecho sostenido
        if (Input.GetMouseButton(1))
        {
            rotY += Input.GetAxis("Mouse X") * lookSpeed;
            rotX -= Input.GetAxis("Mouse Y") * lookSpeed;
            transform.eulerAngles = new Vector3(rotX, rotY, 0f);
        }
    }
}