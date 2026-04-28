using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CablesPanel : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject originPrefab;
    public GameObject targetPrefab;

    [Header("Cámara del panel")]
    public Camera panelCamera;

    [Header("Máquina dueña")]
    public MachineInteraction machineOwner;

    [Header("Spawn")]
    public int cableCount = 3;

    [Header("Área de Spawn")]
    public Transform topLeft;
    public Transform bottomRight;
    public float minDistance = 1.2f;

    [Header("Visual")]
    public Material lineMaterial;
    public float lineWidth = 0.05f;

    [Header("Cierre")]
    public float closePanelDelay = 1f;

    private List<CableNode> allNodes = new List<CableNode>();
    private List<Vector3> usedPositions = new List<Vector3>();
    private List<LineRenderer> finishedLines = new List<LineRenderer>();

    private CableNode selectedOrigin;
    private LineRenderer currentLine;

    private bool completed = false;

    [Header("Colores")]
    private List<Color> cableColors = new List<Color>()
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        new Color(0.5f, 0f, 0.5f),
        Color.black,
        Color.white,
        new Color(0.96f, 0.87f, 0.70f),
        new Color(0.59f, 0.29f, 0f),
        new Color(0.86f, 0.08f, 0.24f),
        new Color(0f, 0.81f, 0.82f),
        new Color(0.54f, 0.17f, 0.89f)
    };

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip wireSound;
    public AudioClip connecSound;
    public AudioClip errorSound;
    public AudioClip completeSound;
    private void Awake() {
        if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnEnable()
    {
        SetupPanel();
    }

    public void SetMachineOwner(MachineInteraction owner)
    {
        machineOwner = owner;
    }

    void SetupPanel()
    {
        completed = false;

        if (panelCamera == null)
            panelCamera = GetComponentInChildren<Camera>(true);

        if (panelCamera == null)
            panelCamera = Camera.main;

        if (originPrefab == null || targetPrefab == null)
        {
            Debug.LogError("[CablesPanel] Faltan prefabs.");
            enabled = false;
            return;
        }

        if (topLeft == null || bottomRight == null)
        {
            Debug.LogError("[CablesPanel] Faltan topLeft / bottomRight.");
            enabled = false;
            return;
        }

        if (lineMaterial == null)
        {
            Debug.LogError("[CablesPanel] Falta lineMaterial.");
            enabled = false;
            return;
        }

        ClearPanel();
        SpawnAll();
    }

    void Update()
    {
        if (completed) return;

        if (selectedOrigin != null && currentLine != null)
        {
            Vector3 start = selectedOrigin.transform.position;
            Vector3 end = GetMouseWorldPosition();

            currentLine.SetPosition(0, start);
            currentLine.SetPosition(1, end);
        }
    }

    void ClearPanel()
    {
        foreach (var node in allNodes)
        {
            if (node != null)
                Destroy(node.gameObject);
        }

        foreach (var line in finishedLines)
        {
            if (line != null)
                Destroy(line.gameObject);
        }

        if (currentLine != null)
            Destroy(currentLine.gameObject);

        allNodes.Clear();
        usedPositions.Clear();
        finishedLines.Clear();

        selectedOrigin = null;
        currentLine = null;
    }

    void SpawnAll()
    {
        List<int> ids = new List<int>();

        for (int i = 0; i < cableCount; i++)
            ids.Add(i);

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

            if (node == null)
            {
                Debug.LogError("[CablesPanel] El originPrefab no tiene CableNode.");
                continue;
            }

            node.id = id;
            node.isOrigin = true;
            node.isConnected = false;
            node.SetColor(cableColors[id % cableColors.Count]);

            allNodes.Add(node);
        }

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

            if (node == null)
            {
                Debug.LogError("[CablesPanel] El targetPrefab no tiene CableNode.");
                continue;
            }

            node.id = id;
            node.isOrigin = false;
            node.isConnected = false;
            node.SetColor(cableColors[id % cableColors.Count]);

            allNodes.Add(node);
        }
    }

    Vector3 GetValidPosition()
    {
        for (int i = 0; i < 50; i++)
        {
            float tX = Random.value;
            float tY = Random.value;

            Vector3 localPos = new Vector3(
                Mathf.Lerp(topLeft.localPosition.x, bottomRight.localPosition.x, tX),
                Mathf.Lerp(topLeft.localPosition.y, bottomRight.localPosition.y, tY),
                Mathf.Lerp(topLeft.localPosition.z, bottomRight.localPosition.z, tY)
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

    public void OnNodeClicked(CableNode node)
    {
        if (completed) return;
        if (node == null || node.isConnected) return;

        if (node.isOrigin && selectedOrigin == null)
        {
            audioSource.PlayOneShot(wireSound);
            selectedOrigin = node;
            StartCable();
            return;
        }

        if (node.isOrigin && selectedOrigin != null)
        {
            CancelCable();
            return;
        }

        if (!node.isOrigin && selectedOrigin != null)
        {
            TryConnect(node);
        }
    }

    void StartCable()
    {
        GameObject go = new GameObject("CableLine");
        go.transform.SetParent(transform); // lo hace hijo de este GameObject

        currentLine = go.AddComponent<LineRenderer>();

        currentLine.material = new Material(lineMaterial);
        currentLine.startWidth = lineWidth;
        currentLine.endWidth = lineWidth;
        currentLine.positionCount = 2;
        currentLine.useWorldSpace = true;

        Color c = cableColors[selectedOrigin.id % cableColors.Count];

        currentLine.startColor = c;
        currentLine.endColor = c;
        currentLine.material.color = c;
    }

    void CancelCable()
    {
        if (currentLine != null)
            Destroy(currentLine.gameObject);

        currentLine = null;
        selectedOrigin = null;
    }

    void TryConnect(CableNode target)
    {
        if (target.id == selectedOrigin.id)
        {
            Debug.Log("Cable correcto");
            audioSource.PlayOneShot(connecSound);

            selectedOrigin.isConnected = true;
            target.isConnected = true;

            FinishCable(target);
            CheckWin();
        }
        else
        {
            Debug.Log("Cable incorrecto");
            audioSource.PlayOneShot(errorSound);
            CancelCable();

            FindAnyObjectByType<LevelOneDialogueController>()?.CablesError();
        }
    }

    void FinishCable(CableNode target)
    {
        if (currentLine != null)
        {
            currentLine.SetPosition(0, selectedOrigin.transform.position);
            currentLine.SetPosition(1, target.transform.position);

            finishedLines.Add(currentLine);
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
        audioSource.PlayOneShot(completeSound);
        Debug.Log("TODOS LOS CABLES CONECTADOS");
        StartCoroutine(CompleteCablePanelRoutine());
    }

    IEnumerator CompleteCablePanelRoutine()
    {
        completed = true;

        FindAnyObjectByType<LevelOneDialogueController>()?.CablesExito();

        yield return new WaitForSeconds(closePanelDelay);

        if (machineOwner != null)
        {
            machineOwner.MarcarMaquinaReparada();
            machineOwner.CerrarPanelDesdeMinijuego();
        }
        else
        {
            Debug.LogWarning("[CablesPanel] No hay machineOwner asignado.");
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        if (panelCamera == null)
            return transform.position;

        Ray ray = panelCamera.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(transform.forward, transform.position);

        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return transform.position;
    }
}