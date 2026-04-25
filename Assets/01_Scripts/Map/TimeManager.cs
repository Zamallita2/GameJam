using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public float timer = 0;
    public float limitTime = 10;

    public GameObject gameOverPanel;
    public GameObject gamePanel;

    public TextMeshProUGUI timerText;

    public bool started = false;
    public bool timerPausado = false;

    private bool isGameOver = false;
    private Material textMaterial;

    public void Iniciar()
    {
        if (timerText != null)
        {
            textMaterial = Instantiate(timerText.fontMaterial);
            timerText.fontMaterial = textMaterial;
        }

        started = true;
    }

    void Update()
    {
        if (!started) return;
        if (isGameOver) return;
        if (timerPausado) return;

        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / limitTime);

        Color currentColor = Color.Lerp(Color.green, Color.red, t);
        string hexColor = ColorUtility.ToHtmlStringRGB(currentColor);

        if (timerText != null)
            timerText.text = $"<color=#{hexColor}>{timer:F2}</color> / {limitTime:F0}";

        if (textMaterial != null)
        {
            float glowPower = Mathf.Lerp(0.1f, 0.6f, t);
            textMaterial.SetFloat("_GlowPower", glowPower);
            textMaterial.SetColor("_GlowColor", currentColor);

            if (t > 0.7f)
            {
                float pulse = Mathf.Sin(Time.time * 10f) * 0.5f + 0.5f;
                textMaterial.SetFloat("_GlowPower", Mathf.Lerp(0.3f, 0.8f, pulse));
            }
        }

        if (timer >= limitTime)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (gamePanel != null)
            gamePanel.SetActive(false);

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MenuScene");
    }
}