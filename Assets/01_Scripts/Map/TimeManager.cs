using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public float timer = 0;
    public float limitTime = 10;

    public GameObject gameOverPanel;
    public GameObject gamePanel;
    bool isGameOver = false;

    public TextMeshProUGUI timerText;

    // Material del texto (instancia para poder modificar glow)
    private Material textMaterial;
    public bool started=false;

    public void Iniciar()
    {
        // Crear instancia del material para no modificar el original
        textMaterial = Instantiate(timerText.fontMaterial);
        timerText.fontMaterial = textMaterial;
        started=true;
    }

    void Update()
    {
        if(!started)
            return;
        if (isGameOver) return;

        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / limitTime);

        // 🎨 Color dinámico (verde → rojo)
        Color currentColor = Color.Lerp(Color.green, Color.red, t);

        // Convertir color a HEX para usar en Rich Text
        string hexColor = ColorUtility.ToHtmlStringRGB(currentColor);

        // ✨ SOLO el tiempo cambia de color
        timerText.text = $"<color=#{hexColor}>{timer:F2}</color> / {limitTime:F0}";

        // 💡 Glow dinámico
        float glowPower = Mathf.Lerp(0.1f, 0.6f, t);
        textMaterial.SetFloat("_GlowPower", glowPower);
        textMaterial.SetColor("_GlowColor", currentColor);

        // 🔥 Pulsación cuando está por acabarse
        if (t > 0.7f)
        {
            float pulse = Mathf.Sin(Time.time * 10f) * 0.5f + 0.5f;
            textMaterial.SetFloat("_GlowPower", Mathf.Lerp(0.3f, 0.8f, pulse));
        }

        if (timer >= limitTime)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
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