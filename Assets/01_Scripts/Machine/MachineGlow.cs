using UnityEngine;

public class MachineGlow : MonoBehaviour
{
    public Color glowColor = Color.cyan;
    public float speed = 0.7f;
    public float intensity = 2f;

    private Renderer rend;
    private float t = 0f;
    private int direction = 1;
    private bool reparada = false;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();

        if (rend == null)
        {
            Debug.LogWarning("No se encontró Renderer en " + gameObject.name);
            enabled = false;
            return;
        }

        rend.material.EnableKeyword("_EMISSION");
    }

    void Update()
    {
        if (reparada) return;

        t += Time.deltaTime * (1f / speed) * direction;

        if (t >= 1f)
        {
            t = 1f;
            direction = -1;
        }

        if (t <= 0f)
        {
            t = 0f;
            direction = 1;
        }

        Color current = Color.Lerp(Color.black, glowColor * intensity, t);
        rend.material.SetColor("_EmissionColor", current);
    }

    public void StopGlow()
    {
        reparada = true;

        if (rend != null)
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", Color.black);
        }

        enabled = false;
    }

    public void StartGlow()
    {
        reparada = false;
        t = 0f;
        direction = 1;

        if (rend == null)
            rend = GetComponentInChildren<Renderer>();

        if (rend != null)
        {
            rend.material.EnableKeyword("_EMISSION");
            rend.material.SetColor("_EmissionColor", glowColor * intensity);
        }

        enabled = true;
    }
}