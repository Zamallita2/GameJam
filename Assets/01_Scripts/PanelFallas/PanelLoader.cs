using System.Collections.Generic;
using UnityEngine;

public class PanelLoader : MonoBehaviour
{
    [Header("Dónde aparece el contenido")]
    public Transform miniGameContainer;

    [Header("Paneles disponibles por tipo")]
    public List<PanelDefinition> paneles = new List<PanelDefinition>();

    private GameObject panelActual;

    public void CargarPanel(MachineType.TipoMaquina tipo, int nivel)
    {
        LimpiarPanelActual();

        GameObject prefab = BuscarPanel(tipo);

        if (prefab == null)
        {
            Debug.LogWarning($"No se encontró panel para tipo {tipo}");
            return;
        }

        panelActual = Instantiate(prefab, miniGameContainer);
        panelActual.transform.localPosition = Vector3.zero;
        panelActual.transform.localRotation = Quaternion.identity;
        panelActual.transform.localScale = Vector3.one;

        PanelBase panelBase = panelActual.GetComponent<PanelBase>();
        if (panelBase != null)
        {
            panelBase.Setup(nivel);
        }
    }

    public void LimpiarPanelActual()
    {
        if (panelActual != null)
        {
            Destroy(panelActual);
            panelActual = null;
        }
    }

    GameObject BuscarPanel(MachineType.TipoMaquina tipo)
    {
        foreach (var panel in paneles)
        {
            if (panel.tipoMaquina == tipo)
                return panel.panelPrefab;
        }

        return null;
    }
}