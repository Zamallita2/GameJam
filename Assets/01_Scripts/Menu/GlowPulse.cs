using UnityEngine;
using TMPro;

public class GlowPulse : MonoBehaviour
{
    public TMP_Text tmpText;
    public Color glowColor = new Color(0f, 0.9f, 1f);
    [Range(0.1f, 3f)] public float speed = 1.2f;
    [Range(0f, 1f)] public float minPow = 0.1f;
    [Range(0f, 1f)] public float maxPow = 0.7f;

    void Start()
    {
        // Toma el componente si no fue asignado en el Inspector
        if (tmpText == null)
            tmpText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) * 0.5f;
        float power = Mathf.Lerp(minPow, maxPow, t);

        tmpText.fontMaterial.SetColor(
            ShaderUtilities.ID_GlowColor, glowColor
        );
        tmpText.fontMaterial.SetFloat(
            ShaderUtilities.ID_GlowPower, power
        );
    }
}