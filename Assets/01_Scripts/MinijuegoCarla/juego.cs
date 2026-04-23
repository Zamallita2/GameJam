using UnityEngine;
using UnityEngine.Rendering;

public class juego : MonoBehaviour
{
    public GameObject MenuGanar;
    public GameObject PiezaSeleccionada;

    public int TotalPiezas = 10;

    int capa = 1;
    public int PiezasEncajadas = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(
                Camera.main.ScreenToWorldPoint(Input.mousePosition),
                Vector2.zero
            );

            if (hit.transform != null &&
                hit.transform.CompareTag("Puzzle"))
            {
                pieza piezaScript =
                    hit.transform.GetComponent<pieza>();

                if (!piezaScript.Encajada)
                {
                    PiezaSeleccionada = hit.transform.gameObject;

                    piezaScript.Seleccionada = true;

                    PiezaSeleccionada
                        .GetComponent<SortingGroup>()
                        .sortingOrder = capa;

                    capa++;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (PiezaSeleccionada != null)
            {
                PiezaSeleccionada
                    .GetComponent<pieza>()
                    .Seleccionada = false;

                PiezaSeleccionada = null;
            }
        }

        if (PiezaSeleccionada != null)
        {
            Vector3 raton =
                Camera.main.ScreenToWorldPoint(Input.mousePosition);

            PiezaSeleccionada.transform.position =
                new Vector3(raton.x, raton.y, 0);
        }

        // GANAR
        if (PiezasEncajadas == TotalPiezas)
        {
            MenuGanar.SetActive(true);
        }
    }
}