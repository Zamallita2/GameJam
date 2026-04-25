using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class CanvasPanelBuilder : EditorWindow
{
    [MenuItem("Tools/Crear Canvas Panel")]
    public static void Build()
    {
        // Canvas
        GameObject canvasGO = new GameObject("PanelCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();

        RectTransform rt = canvasGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(1920, 1080);
        rt.localScale = Vector3.one * 0.005f;

        // Image de fondo
        GameObject imgGO = new GameObject("PanelImage");
        imgGO.transform.SetParent(canvasGO.transform);
        Image img = imgGO.AddComponent<Image>();
        img.color = Color.gray;

        RectTransform imgRT = imgGO.GetComponent<RectTransform>();
        imgRT.anchorMin = Vector2.zero;
        imgRT.anchorMax = Vector2.one;
        imgRT.offsetMin = Vector2.zero;
        imgRT.offsetMax = Vector2.zero;

        // Script
        CanvasColorPanel cp = canvasGO.AddComponent<CanvasColorPanel>();
        cp.panelImage = img;

        Selection.activeGameObject = canvasGO;
        Debug.Log("PanelCanvas creado!");
    }
}