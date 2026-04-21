using UnityEngine;

public class ArmsBobbing : MonoBehaviour
{
    [Header("Bobbing Settings")]
    public float bobSpeed = 8f;      // velocidad del movimiento
    public float bobAmount = 0.1f;   // intensidad (sube/baja)
    public float sideAmount = 0.05f; // movimiento lateral (opcional)

    private float timer = 0;
    private Vector3 startPos;

    private CharacterController controller;

    void Start()
    {
        startPos = transform.localPosition;

        // Busca el CharacterController en el Player (padre)
        controller = GetComponentInParent<CharacterController>();
    }

    void Update()
    {
        // Velocidad real del jugador
        float speed = controller.velocity.magnitude;

        if (speed > 0.1f)
        {
            // Avanza el tiempo del bobbing
            timer += Time.deltaTime * bobSpeed;

            // Movimiento vertical (arriba/abajo)
            float y = Mathf.Sin(timer) * bobAmount;

            // Movimiento lateral (izquierda/derecha)
            float x = Mathf.Cos(timer * 0.5f) * sideAmount;

            // Aplicar movimiento
            transform.localPosition = startPos + new Vector3(x, y, 0);
        }
        else
        {
            // Reset suave cuando no se mueve
            timer = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Time.deltaTime * 5f);
        }
    }
}