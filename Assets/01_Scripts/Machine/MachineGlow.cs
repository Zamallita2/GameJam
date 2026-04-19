using UnityEngine;

public class MachineGlow : MonoBehaviour
{
    public Color glowColor = Color.cyan;
    public float speed = 0.7f;
    public float intensity = 2f;

    private Renderer rend;
    private float t = 0f;
    private int direction = 1;

    void Start()
    {
        rend = GetComponent<Renderer>();
        rend.material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        t += Time.deltaTime * (1f / speed) * direction;

        if (t >= 1f) { t = 1f; direction = -1; }
        if (t <= 0f) { t = 0f; direction = 1; }

        Color current = Color.Lerp(Color.black, glowColor * intensity, t);
        rend.material.SetColor("_EmissionColor", current);
    }
}