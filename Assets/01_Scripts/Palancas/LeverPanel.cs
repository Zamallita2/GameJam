using UnityEngine;
using UnityEngine.InputSystem;

public class LeverPanel : MonoBehaviour
{
    [Header("Palancas")]
    public LeverInteractable[] levers;

    [Header("Combinacion correcta (1=activo, 0=inactivo)")]
    public int[] combination = { 1, 1, 0, 1, 0 };

    [Header("Puerta")]
    public Door door;

    private bool solved = false;

    void Start()
    {
        for (int i = 0; i < levers.Length; i++)
        {
            levers[i].leverID = i;
            levers[i].panel = this;
        }
    }

    void Update()
    {
        if (solved) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                LeverInteractable lever = hit.collider.GetComponentInParent<LeverInteractable>();
                if (lever != null)
                    lever.Toggle();
            }
        }
    }

    public void OnLeverToggled()
    {
        if (solved) return;

        // Debug estado actual
        string estado = "";
        for (int i = 0; i < levers.Length; i++)
            estado += levers[i].isActive ? "1," : "0,";
        Debug.Log("Estado actual: " + estado);

        if (CheckCombination())
        {
            solved = true;
            Debug.Log("Combinacion correcta! Abriendo puerta...");
            if (door != null) door.Open();
            else Debug.Log("DOOR ES NULL!");
        }
    }

    bool CheckCombination()
    {
        if (levers.Length != combination.Length) return false;
        for (int i = 0; i < levers.Length; i++)
        {
            bool shouldBeActive = combination[i] == 1;
            if (levers[i].isActive != shouldBeActive) return false;
        }
        return true;
    }
}