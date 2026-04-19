using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Puntos de spawn")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Prefabs a instanciar")]
    public List<GameObject> prefabs = new List<GameObject>();

    [Header("Simulación de nivel")]
    [Min(1)]
    public int nivelActual = 1;

    [Header("Configuración de dificultad")]
    [Min(1)]
    public int spawnsBase = 2;

    [Min(0)]
    public int spawnsPorNivel = 1;

    [Min(1)]
    public int maxSpawns = 10;

    [Header("Opciones")]
    public bool spawnAlIniciar = true;
    public bool limpiarAntesDeSpawn = true;

    private List<GameObject> maquinasInstanciadas = new List<GameObject>();

    void Start()
    {
        if (spawnAlIniciar)
        {
            GenerarSpawnPorNivel();
        }
    }

    public void GenerarSpawnPorNivel()
    {
        if (spawnPoints.Count == 0 || prefabs.Count == 0)
        {
            Debug.LogWarning("No hay puntos o prefabs asignados unu");
            return;
        }

        if (limpiarAntesDeSpawn)
        {
            LimpiarSpawns();
        }

        int cantidadDeseada = spawnsBase + (nivelActual - 1) * spawnsPorNivel;
        cantidadDeseada = Mathf.Clamp(cantidadDeseada, 1, Mathf.Min(maxSpawns, spawnPoints.Count));

        List<Transform> puntosDisponibles = new List<Transform>(spawnPoints);

        for (int i = 0; i < cantidadDeseada; i++)
        {
            if (puntosDisponibles.Count == 0)
                break;

            int indexPunto = Random.Range(0, puntosDisponibles.Count);
            Transform punto = puntosDisponibles[indexPunto];
            puntosDisponibles.RemoveAt(indexPunto);

            int indexPrefab = Random.Range(0, prefabs.Count);
            GameObject prefab = prefabs[indexPrefab];

            GameObject maquina = Instantiate(prefab, punto.position, punto.rotation);
            maquinasInstanciadas.Add(maquina);
        }

        Debug.Log($"Nivel {nivelActual}: se generaron {cantidadDeseada} máquinas.");
    }

    public void LimpiarSpawns()
    {
        for (int i = maquinasInstanciadas.Count - 1; i >= 0; i--)
        {
            if (maquinasInstanciadas[i] != null)
            {
                Destroy(maquinasInstanciadas[i]);
            }
        }

        maquinasInstanciadas.Clear();
    }

    public void SubirNivelYRegenerar()
    {
        nivelActual++;
        GenerarSpawnPorNivel();
    }

    public void FijarNivelYRegenerar(int nuevoNivel)
    {
        nivelActual = Mathf.Max(1, nuevoNivel);
        GenerarSpawnPorNivel();
    }
}