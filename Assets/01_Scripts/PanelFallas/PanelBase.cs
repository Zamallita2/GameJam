using UnityEngine;

public abstract class PanelBase : MonoBehaviour
{
    protected int nivelActual;

    public virtual void Setup(int nivel)
    {
        nivelActual = nivel;
    }
}