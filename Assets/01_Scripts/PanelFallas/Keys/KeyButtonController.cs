using System.Collections;
using UnityEngine;

public class KeyButtonController : MonoBehaviour
{
    public enum KeySymbol
    {
        A,
        S,
        D,
        F
    }

    [Header("Tecla")]
    public KeySymbol keySymbol;
    public KeyCode keyCode;

    [Header("Botón")]
    public Transform pressTarget;
    public float pressDepth = 0.01f;
    public float pressSpeed = 12f;

    [Header("Renderer")]
    public Renderer targetRenderer;
    public Material idleMaterial;
    public Material pressedMaterial;
    public Material errorMaterial;

    [Header("Luz")]
    public Light keyLight;
    public float lightDuration = 0.15f;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip pressSound;
    [Range(0f, 1f)] public float volume = 1f;

    private Vector3 initialLocalPos;
    private bool isAnimating = false;

    void Start()
    {
        if (pressTarget == null)
            pressTarget = transform;

        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        initialLocalPos = pressTarget.localPosition;

        if (keyLight != null)
            keyLight.enabled = false;

        SetIdle();
    }

    public void SetIdle()
    {
        if (targetRenderer != null && idleMaterial != null)
            targetRenderer.material = idleMaterial;
    }

    public void SetPressed()
    {
        if (targetRenderer != null && pressedMaterial != null)
            targetRenderer.material = pressedMaterial;
    }

    public void SetError()
    {
        if (targetRenderer != null && errorMaterial != null)
            targetRenderer.material = errorMaterial;
    }

    public void TriggerCorrectFeedback()
    {
        if (!isAnimating)
            StartCoroutine(PressRoutine());

        StartCoroutine(LightRoutine());
        StartCoroutine(PressedRoutine());

        if (audioSource != null && pressSound != null)
            audioSource.PlayOneShot(pressSound, volume);
    }

    public void TriggerWrongFeedback()
    {
        StartCoroutine(ErrorRoutine());
    }

    IEnumerator PressRoutine()
    {
        isAnimating = true;

        Vector3 pressedPos = initialLocalPos + new Vector3(0f, -pressDepth, 0f);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * pressSpeed;
            pressTarget.localPosition = Vector3.Lerp(initialLocalPos, pressedPos, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * pressSpeed;
            pressTarget.localPosition = Vector3.Lerp(pressedPos, initialLocalPos, t);
            yield return null;
        }

        pressTarget.localPosition = initialLocalPos;
        isAnimating = false;
    }

    IEnumerator LightRoutine()
    {
        if (keyLight == null) yield break;

        keyLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        keyLight.enabled = false;
    }

    IEnumerator PressedRoutine()
    {
        SetPressed();
        yield return new WaitForSeconds(0.12f);
        SetIdle();
    }

    IEnumerator ErrorRoutine()
    {
        SetError();

        if (keyLight != null)
            keyLight.enabled = true;

        yield return new WaitForSeconds(0.2f);

        if (keyLight != null)
            keyLight.enabled = false;

        SetIdle();
    }
}