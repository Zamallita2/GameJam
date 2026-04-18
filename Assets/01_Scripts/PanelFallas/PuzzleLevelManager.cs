using UnityEngine;

public class PuzzleLevelManager : MonoBehaviour
{
    public static PuzzleLevelManager Instance;

    public int currentLevel = 1;

    public FallaBotonesManager botonesManager;
    public FallaCablesManager cablesManager;
    public FallaPalancasManager palancasManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadLevel(currentLevel);
    }

    public void LoadLevel(int level)
    {
        currentLevel = level;

        botonesManager.gameObject.SetActive(false);
        cablesManager.gameObject.SetActive(false);
        palancasManager.gameObject.SetActive(false);

        if (level >= 1 && level <= 3)
        {
            botonesManager.gameObject.SetActive(true);
            botonesManager.SetupLevel(level);
        }
        else if (level >= 4 && level <= 6)
        {
            cablesManager.gameObject.SetActive(true);
            cablesManager.SetupLevel(level);
        }
        else if (level >= 7 && level <= 9)
        {
            palancasManager.gameObject.SetActive(true);
            palancasManager.SetupLevel(level);
        }
        else
        {
            Debug.Log("Juego completado");
        }
    }

    public void CompleteLevel()
    {
        LoadLevel(currentLevel + 1);
    }

    public void RestartCurrentLevel()
    {
        LoadLevel(currentLevel);
    }
}