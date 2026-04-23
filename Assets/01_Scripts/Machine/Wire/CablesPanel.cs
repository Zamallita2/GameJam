using System.Collections.Generic;
using UnityEngine;

public class CablesPanel : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject originPrefab;
    public GameObject targetPrefab;

    [Header("Spawn")]
    public int cableCount = 3;
    [Header("Area de Spawn")]
    public Transform topLeft;
    public Transform bottomRight;    public float minDistance = 1.2f;

    [Header("Visual")]
    public Material lineMaterial;
    public float lineWidth = 0.05f;

    private List<CableNode> allNodes = new List<CableNode>();
    private List<Vector3> usedPositions = new List<Vector3>();

    private CableNode selectedOrigin;
    private LineRenderer currentLine;

    void Start()
    {
        SpawnAll();
    }

    void Update()
    {
        if (selectedOrigin != null && currentLine != null)
        {
            Vector3 start = selectedOrigin.transform.position;
            Vector3 end = GetMouseWorldPosition();

            currentLine.SetPosition(0, start);
            currentLine.SetPosition(1, end);
        }
    }

    // 🧩 SPAWN
    void SpawnAll()
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < cableCount; i++)
            ids.Add(i);

        // ORIGINS
        foreach (int id in ids)
        {
            Vector3 pos = GetValidPosition();
            GameObject obj = Instantiate(
                originPrefab,
                pos,
                transform.rotation * Quaternion.Euler(0f, -90f, 0f),
                transform
            );

            CableNode node = obj.GetComponent<CableNode>();
            node.id = id;
            node.isOrigin = true;
            node.SetColor(cableColors[id]);

            allNodes.Add(node);
        }

        // TARGETS (mezclados)
        Shuffle(ids);

        foreach (int id in ids)
        {
            Vector3 pos = GetValidPosition();
            GameObject obj = Instantiate(
                targetPrefab,
                pos,
                transform.rotation * Quaternion.Euler(0f, -90f, 0f),
                transform
            );

            CableNode node = obj.GetComponent<CableNode>();
            node.id = id;
            node.isOrigin = false;
            node.SetColor(cableColors[id]);

            allNodes.Add(node);
        }
    }

    Vector3 GetValidPosition()
    {
        for (int i = 0; i < 50; i++)
        {
            // interpolación entre esquinas
            float tX = Random.value;
            float tY = Random.value;

            Vector3 localPos = new Vector3(
                Mathf.Lerp(topLeft.localPosition.x, bottomRight.localPosition.x, tX),
                Mathf.Lerp(topLeft.localPosition.y, bottomRight.localPosition.y, tY),
                Mathf.Lerp(topLeft.localPosition.z, bottomRight.localPosition.z, tY) // opcional
            );

            Vector3 worldPos = transform.TransformPoint(localPos);

            if (IsFarEnough(worldPos))
            {
                usedPositions.Add(worldPos);
                return worldPos;
            }
        }

        return transform.position;
    }

    bool IsFarEnough(Vector3 pos)
    {
        foreach (var p in usedPositions)
        {
            if (Vector3.Distance(p, pos) < minDistance)
                return false;
        }
        return true;
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    // 🖱 INTERACCIÓN
    public void OnNodeClicked(CableNode node)
    {
        if (node.isConnected) return;

        // 🟡 seleccionar origen
        if (node.isOrigin && selectedOrigin == null)
        {
            selectedOrigin = node;
            StartCable();
            return;
        }

        // 🔁 cancelar si clic en otro origen
        if (node.isOrigin && selectedOrigin != null)
        {
            CancelCable();
            return;
        }

        // 🟢 intentar conectar
        if (!node.isOrigin && selectedOrigin != null)
        {
            TryConnect(node);
        }
    }

    void StartCable()
    {
        GameObject go = new GameObject("CableLine");
        currentLine = go.AddComponent<LineRenderer>();

        // 🔥 MATERIAL NUEVO (clave)
        currentLine.material = new Material(lineMaterial);

        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.positionCount = 2;

        // 🎨 FORZAR COLOR BIEN
        Color c = cableColors[selectedOrigin.id];

        currentLine.startColor = c;
        currentLine.endColor = c;

        // 🔥 EXTRA: asegurar que el material también tenga color
        currentLine.material.color = c;
    }

    void CancelCable()
    {
        if (currentLine != null)
            Destroy(currentLine.gameObject);

        selectedOrigin = null;
    }

    void TryConnect(CableNode target)
    {
        if (target.id == selectedOrigin.id)
        {
            Debug.Log("Correcto 🟢");

            selectedOrigin.isConnected = true;
            target.isConnected = true;

            FinishCable(target);
            CheckWin();
        }
        else
        {
            Debug.Log("Incorrecto 🔴");
            CancelCable();
        }
    }

    void FinishCable(CableNode target)
    {
        if (currentLine != null)
        {
            currentLine.SetPosition(0, selectedOrigin.transform.position);
            currentLine.SetPosition(1, target.transform.position);

            currentLine = null;
        }

        selectedOrigin = null;
    }

    void CheckWin()
    {
        foreach (var node in allNodes)
        {
            if (!node.isConnected)
                return;
        }

        Debug.Log("🎉 TODOS LOS CABLES CONECTADOS 🐾✨");
    }

    Vector3 GetMouseWorldPosition()
    {   
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // plano alineado al panel 🐾
        Plane plane = new Plane(transform.forward, transform.position);

        if (plane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }

        return transform.position;
    }
    [Header("Colores")]
    private List<Color> cableColors = new List<Color>()
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.purple,
        Color.black,
        Color.white,
        Color.wheat,
        Color.brown,
        Color.crimson,
        Color.darkTurquoise,
        Color.blueViolet
    };
}