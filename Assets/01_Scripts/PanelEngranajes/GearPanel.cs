using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GearPanel : MonoBehaviour
{
    [Header("Engranajes en orden correcto")]
    public GearInteractable[] gearsInOrder;

    [Header("Lamp")]
    public Renderer statusLamp;
    public Material lampNormal;
    public Material lampOk;
    public Material lampError;

    [Header("Canvas")]
    public CanvasColorPanel canvasPanel;

    private int currentStep = 0;
    private bool panelDone = false;

    void Start()
    {
        for (int i = 0; i < gearsInOrder.Length; i++)
        {
            gearsInOrder[i].gearID = i;
            gearsInOrder[i].panel = this;
        }

        if (statusLamp != null && lampNormal != null)
            statusLamp.material = lampNormal;

        if (canvasPanel != null)
            canvasPanel.SetGray();
    }

    void Update()
    {
        if (panelDone) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GearInteractable gear = hit.collider.GetComponentInParent<GearInteractable>();
                if (gear != null)
                    gear.OnClicked();
            }
        }
    }

    public void OnGearClicked(GearInteractable gear)
    {
        if (panelDone) return;

        if (gear.gearID == currentStep)
        {
            gear.SetActivo();
            currentStep++;

            if (canvasPanel != null) canvasPanel.SetGreen();

            if (currentStep >= gearsInOrder.Length)
                PanelCompleted();
        }
        else
        {
            StartCoroutine(HandleError(gear));
        }
    }

    IEnumerator HandleError(GearInteractable wrongGear)
    {
        if (statusLamp != null && lampError != null)
            statusLamp.material = lampError;

        if (canvasPanel != null) canvasPanel.SetRedThenGray();

        wrongGear.SetError();

        yield return new WaitForSeconds(0.8f);

        foreach (var g in gearsInOrder)
            g.SetNormal();

        currentStep = 0;

        if (statusLamp != null && lampNormal != null)
            statusLamp.material = lampNormal;
    }

    void PanelCompleted()
    {
        panelDone = true;

        if (statusLamp != null && lampOk != null)
            statusLamp.material = lampOk;

        if (canvasPanel != null) canvasPanel.SetGreen();

        Debug.Log("Panel Engranajes completado!");
        // GameManager.Instance.OnMiniGameCompleted();
    }
}