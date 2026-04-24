using UnityEngine;

public class MachineType : MonoBehaviour
{
    public enum TipoMaquina
    {
        Botones,
        Cables,
        Palancas,
        Puzzle
    }

    [Header("Configuración de máquina")]
    public TipoMaquina tipo = TipoMaquina.Botones;

    [Min(1)]
    public int nivel = 1;
}