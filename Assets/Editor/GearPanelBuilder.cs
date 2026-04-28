using UnityEngine;
using UnityEditor;

public class GearPanelBuilder : EditorWindow
{
    int gearCount = 5;

    [MenuItem("Tools/Crear Panel Engranajes")]
    public static void ShowWindow()
    {
        GetWindow<GearPanelBuilder>("Gear Panel Builder");
    }

    void OnGUI()
    {
        GUILayout.Label("Panel Engranajes Builder", EditorStyles.boldLabel);
        GUILayout.Space(5);
        gearCount = EditorGUILayout.IntField("Cantidad de Engranajes:", gearCount);
        GUILayout.Space(10);
        if (GUILayout.Button("CREAR PANEL"))
        {
            BuildPanel();
        }
    }

    void BuildPanel()
    {
        GameObject root = new GameObject("Panel_Engranajes");
        root.AddComponent<GearPanel>();

        new GameObject("ModeloPanel").transform.SetParent(root.transform);

        GameObject lamp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        lamp.name = "StatusLamp";
        lamp.transform.SetParent(root.transform);
        lamp.transform.localPosition = new Vector3((gearCount - 1) * 1.5f + 1.5f, 1f, 0f);
        lamp.transform.localScale = Vector3.one * 0.3f;

        GameObject container = new GameObject("MiniGameContainer");
        container.transform.SetParent(root.transform);

        GameObject slotsParent = new GameObject("GearSlots");
        slotsParent.transform.SetParent(container.transform);

        GameObject gearsParent = new GameObject("Engranajes");
        gearsParent.transform.SetParent(container.transform);

        Component[] gears = new Component[gearCount];

        for (int i = 0; i < gearCount; i++)
        {
            GameObject slot = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            slot.name = $"Slot_{i + 1:00}";
            slot.transform.SetParent(slotsParent.transform);
            slot.transform.localPosition = new Vector3(i * 1.5f, 0f, 0f);
            slot.transform.localScale = new Vector3(0.9f, 0.05f, 0.9f);
            DestroyImmediate(slot.GetComponent<Collider>());

            GameObject gear = GameObject.CreatePrimitive(PrimitiveType.Cube);
            gear.name = $"Gear_{i + 1:00}";
            gear.transform.SetParent(gearsParent.transform);
            gear.transform.localPosition = new Vector3(i * 1.5f, 0.5f, 0f);
            gear.transform.localScale = Vector3.one * 0.8f;

            var type = System.Type.GetType("GearInteractable, Assembly-CSharp");
            gears[i] = gear.AddComponent(type);
        }

        // Asignar via SerializedObject para evitar conflicto de assemblies
        GearPanel panel = root.GetComponent<GearPanel>();
        SerializedObject so = new SerializedObject(panel);

        SerializedProperty gearsInOrder = so.FindProperty("gearsInOrder");
        gearsInOrder.arraySize = gearCount;
        for (int i = 0; i < gearCount; i++)
        {
            gearsInOrder.GetArrayElementAtIndex(i).objectReferenceValue = gears[i];
        }

        so.FindProperty("statusLamp").objectReferenceValue = lamp.GetComponent<Renderer>();
        so.ApplyModifiedProperties();

        Selection.activeGameObject = root;
        Debug.Log($"Panel Engranajes creado con {gearCount} engranajes!");
    }
}