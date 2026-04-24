using UnityEngine;
using System.Collections.Generic;

public class PlayerPanelVisibility : MonoBehaviour
{
    [Header("Objetos a ocultar al entrar al panel")]
    public List<GameObject> objectsToHide = new List<GameObject>();

    public void HideForPanel()
    {
        for (int i = 0; i < objectsToHide.Count; i++)
        {
            if (objectsToHide[i] != null)
                objectsToHide[i].SetActive(false);
        }
    }

    public void ShowAfterPanel()
    {
        for (int i = 0; i < objectsToHide.Count; i++)
        {
            if (objectsToHide[i] != null)
                objectsToHide[i].SetActive(true);
        }
    }
}