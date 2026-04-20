using System.Collections;
using UnityEngine;

public class SimonButton3D : MonoBehaviour
{
    [Header("ID")]
    public int buttonId;

    [Header("Renderer")]
    public Renderer targetRenderer;

    [Header("Materiales")]
    public Material materialInactive;
    public Material materialActive;
    public Material materialError;

    [Header("Animación")]
    public Transform pressTarget;
    public float pressDepth = 0.02f;
    public float pressSpeed = 10f;

    [Header("Sonido")]
    public AudioSource audioSource;
    public AudioClip clickSound;
    public float clickVolume = 1f;

    private SimonButtonsPanel manager;
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
        SetInactive();
    }

    public void Init(SimonButtonsPanel panelManager, int id)
    {
        manager = panelManager;
        buttonId = id;
        SetInactive();
    }

    void OnMouseDown()
    {
        Debug.Log("Click en botón ID: " + buttonId);

        PlayClickSound();

        if (manager != null)
            manager.OnButtonPressed(this);
    }

    void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound, clickVolume);
    }

    public void SetInactive()
    {
        if (targetRenderer != null && materialInactive != null)
            targetRenderer.material = materialInactive;
    }

    public void SetActive()
    {
        if (targetRenderer != null && materialActive != null)
            targetRenderer.material = materialActive;
    }

    public void SetError()
    {
        if (targetRenderer != null && materialError != null)
            targetRenderer.material = materialError;
    }

    public void PressVisual()
    {
        if (!isAnimating)
            StartCoroutine(PressRoutine());
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

    public IEnumerator FlashActive(float duration)
    {
        SetActive();
        PressVisual();
        PlayClickSound();
        yield return new WaitForSeconds(duration);
        SetInactive();
    }

    public IEnumerator FlashError(float duration)
    {
        SetError();
        yield return new WaitForSeconds(duration);
        SetInactive();
    }
}