using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Puntos de spawn")]
    public List<Transform> spawnPoints;

    [Header("Prefabs a instanciar")]
    public List<GameObject> prefabs;

    [Header("Porcentaje de ocupación (0 - 100)")]
    [Range(0, 100)]
    public float porcentajeOcupado = 50f;

    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        if (spawnPoints.Count == 0 || prefabs.Count == 0)
        {
            Debug.LogWarning("No hay puntos o prefabs asignados unu");
            return;
        }

        // Cuántos puntos usar según porcentaje
        int cantidadAUsar = Mathf.RoundToInt(spawnPoints.Count * (porcentajeOcupado / 100f));

        // Copia de la lista para no repetir puntos
        List<Transform> puntosDisponibles = new List<Transform>(spawnPoints);

        for (int i = 0; i < cantidadAUsar; i++)
        {
            if (puntosDisponibles.Count == 0) break;

            // Elegir punto aleatorio
            int indexPunto = Random.Range(0, puntosDisponibles.Count);
            Transform punto = puntosDisponibles[indexPunto];

            // Quitar para no repetir
            puntosDisponibles.RemoveAt(indexPunto);

            // Elegir prefab aleatorio
            int indexPrefab = Random.Range(0, prefabs.Count);
            GameObject prefab = prefabs[indexPrefab];

            // Instanciar con posición y rotación del transform
            Instantiate(prefab, punto.position, punto.rotation);
        }
    }
}