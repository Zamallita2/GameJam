using System.Collections;
using UnityEngine;

public class FastInputButton3D : MonoBehaviour
{
    public enum ColorSymbol
    {
        Red,
        Blue,
        Green,
        Pink
    }

    [Header("Color lógico del botón")]
    public ColorSymbol symbol;

    [Header("Visual")]
    public Renderer targetRenderer;
    public Material idleMaterial;
    public Material pressedMaterial;
    public Material errorMaterial;

    [Header("Animación")]
    public Transform pressTarget;
    public float pressDepth = 0.01f;
    public float pressSpeed = 12f;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip pressSound;
    [Range(0f, 1f)] public float volume = 1f;

    private Vector3 initialLocalPos;
    private bool isAnimating = false;

    void Awake()
    {
        if (targetRenderer == null)
            targetRenderer = GetComponentInChildren<Renderer>();

        if (pressTarget == null)
            pressTarget = transform;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        initialLocalPos = pressTarget.localPosition;
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

    public void TriggerVisualPress()
    {
        if (!isAnimating)
            StartCoroutine(PressRoutine());

        if (audioSource != null && pressSound != null)
            audioSource.PlayOneShot(pressSound, volume);
    }

    public IEnumerator FlashPressed(float duration)
    {
        SetPressed();
        TriggerVisualPress();
        yield return new WaitForSeconds(duration);
        SetIdle();
    }

    public IEnumerator FlashError(float duration)
    {
        SetError();
        yield return new WaitForSeconds(duration);
        SetIdle();
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
}