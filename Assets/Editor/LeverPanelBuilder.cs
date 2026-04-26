using UnityEngine;
using UnityEditor;

public class LeverPanelBuilder : EditorWindow
{
    int leverCount = 5;

    [MenuItem("Tools/Crear Panel Palancas")]
    public static void ShowWindow()
    {
        GetWindow<LeverPanelBuilder>("Lever Panel Builder");
    }

    void OnGUI()
    {
        GUILayout.Label("Lever Panel Builder", EditorStyles.boldLabel);
        leverCount = EditorGUILayout.IntField("Cantidad de Palancas:", leverCount);
        GUILayout.Space(10);
        if (GUILayout.Button("CREAR PANEL")) Build();
    }

    void Build()
    {
        // Root
        GameObject root = new GameObject("LeverPanel");
        var lp = root.AddComponent<LeverPanel>();

        // Pared
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.name = "Pared";
        wall.transform.SetParent(root.transform);
        wall.transform.localPosition = Vector3.zero;
        wall.transform.localScale = new Vector3(leverCount * 1.5f, 3f, 0.3f);

        // Palancas
        LeverInteractable[] levers = new LeverInteractable[leverCount];

        for (int i = 0; i < leverCount; i++)
        {
            // Pivot en la base
            GameObject pivot = new GameObject($"Palanca_{i + 1:00}_Pivot");
            pivot.transform.SetParent(root.transform);
            pivot.transform.localPosition = new Vector3((i - (leverCount - 1) / 2f) * 1.5f, -0.5f, -0.2f);

            // Palo visual hijo del pivot
            GameObject palo = GameObject.CreatePrimitive(PrimitiveType.Cube);
            palo.name = "Palo";
            palo.transform.SetParent(pivot.transform);
            palo.transform.localPosition = new Vector3(0f, 0.5f, 0f); // centro del palo arriba del pivot
            palo.transform.localScale = new Vector3(0.15f, 1f, 0.15f);

            LeverInteractable li = pivot.AddComponent<LeverInteractable>();
            levers[i] = li;
        }

        // Puerta (segunda pared al frente)
        GameObject doorGO = new GameObject("Puerta");
        doorGO.transform.SetParent(root.transform);
        doorGO.transform.localPosition = new Vector3(0f, 0f, -3f);

        GameObject doorMesh = GameObject.CreatePrimitive(PrimitiveType.Cube);
        doorMesh.name = "PuertaMesh";
        doorMesh.transform.SetParent(doorGO.transform);
        doorMesh.transform.localPosition = Vector3.zero;
        doorMesh.transform.localScale = new Vector3(3f, 3f, 0.3f);

        Door door = doorGO.AddComponent<Door>();

        // Asignar via SerializedObject
        SerializedObject so = new SerializedObject(lp);

        SerializedProperty leversArr = so.FindProperty("levers");
        leversArr.arraySize = leverCount;
        for (int i = 0; i < leverCount; i++)
            leversArr.GetArrayElementAtIndex(i).objectReferenceValue = levers[i];

        SerializedProperty combArr = so.FindProperty("combination");
        combArr.arraySize = leverCount;
        for (int i = 0; i < leverCount; i++)
            combArr.GetArrayElementAtIndex(i).intValue = (i % 2 == 0) ? 1 : 0;

        so.FindProperty("door").objectReferenceValue = door;
        so.ApplyModifiedProperties();

        Selection.activeGameObject = root;
        Debug.Log($"Panel Palancas creado con {leverCount} palancas!");
    }
}