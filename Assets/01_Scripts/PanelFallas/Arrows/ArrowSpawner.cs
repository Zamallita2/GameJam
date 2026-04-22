using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowSpawner : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform spawnPoint;
    public Transform hitPoint;

    public float spawnDelay = 1f;

    private List<int> sequence = new List<int>();

    public void StartGame(int length)
    {
        sequence.Clear();

        for (int i = 0; i < length; i++)
        {
            sequence.Add(Random.Range(0, 4));
        }

        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        for (int i = 0; i < sequence.Count; i++)
        {
            SpawnArrow(sequence[i]);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    void SpawnArrow(int dir)
    {
        GameObject arrow = Instantiate(arrowPrefab, spawnPoint.position, Quaternion.identity, transform);

        ArrowObject obj = arrow.GetComponent<ArrowObject>();
        obj.Init(dir, hitPoint.position);
    }
}