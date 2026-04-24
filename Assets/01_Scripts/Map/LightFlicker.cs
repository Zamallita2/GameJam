using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public Light pointLight;

    public float maxIntensity = 8f;

    public float fadeDuration = 2f;
    public float offDuration = 1f;

    public bool isTurnedOff = false;

    void Start()
    {
        if (pointLight == null)
        {
            pointLight = GetComponent<Light>();
        }
    }

    void Update()
    {
        float totalCycle = (fadeDuration * 2f) + offDuration;

        float time = Time.time % totalCycle;

        float intensity = 0f;

        if (time < fadeDuration)
        {
            float t = time / fadeDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            intensity = Mathf.Lerp(0f, maxIntensity, t);
        }
        else if (time < fadeDuration * 2f)
        {
            float t = (time - fadeDuration) / fadeDuration;
            t = Mathf.SmoothStep(0f, 1f, t);

            intensity = Mathf.Lerp(maxIntensity, 0f, t);
        }

        pointLight.intensity = intensity;
    }

    public void TurnOff()
    {
        if (isTurnedOff) return;

        isTurnedOff = true;

        enabled = false;

        if (pointLight != null)
        {
            pointLight.intensity = 0f;
        }
    }
}