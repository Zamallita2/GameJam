using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        // Si no hay player asignado, no hace nada
        if (player == null) return;

        // Si el player está inactivo, no se mueve
        if (!player.gameObject.activeInHierarchy) return;

        // Sigue al player
        transform.position = player.position;
    }
}