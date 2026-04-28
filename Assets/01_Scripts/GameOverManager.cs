using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    [Header("UI")]
    public GameObject gameOverPanel;

    [Header("Audio")]
    public AudioClip gameOverVoice;

    private bool isGameOver = false;

    void Awake()
    {
        Instance = this;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void TriggerGameOver()
    {
        if (isGameOver) return;

        isGameOver = true;

        Debug.Log("[GameOver] ACTIVADO");

        // Mostrar panel
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Bloquear jugador
        PlayerMovement player = FindAnyObjectByType<PlayerMovement>();
        if (player != null)
            player.SetCanMove(false);

        // Cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Audio
        VoiceManager vm = FindAnyObjectByType<VoiceManager>();
        if (vm != null && gameOverVoice != null)
            vm.PlayVoice(gameOverVoice);
    }

    // 🔁 BOTÓN REINTENTAR
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 🚪 BOTÓN MENÚ
    public void GoToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }
}