using System.Collections;
using UnityEngine;

public class GearInteractable : MonoBehaviour
{
    public int gearID;
    [HideInInspector] public GearPanel panel;

    private bool activated = false;
    private bool isAnimating = false;

    private Quaternion originalRot;
    private Vector3 originalScale;

    public float animDuration = 1.4f;

    void Start()
    {
        originalRot = transform.rotation;
        originalScale = transform.localScale; // guardamos escala original (3,3,3)
    }

    public void OnClicked()
    {
        if (activated || isAnimating) return;
        panel.OnGearClicked(this);
    }

    public void SetActivo()
    {
        activated = true;
        StopAllCoroutines();
        StartCoroutine(AnimarEscala());
    }

    public void SetNormal()
    {
        activated = false;
        isAnimating = false;
        StopAllCoroutines();
        transform.rotation = originalRot;
        transform.localScale = originalScale;
    }

    public void SetError() { }

    public bool IsActivated() => activated;

    IEnumerator AnimarEscala()
    {
        isAnimating = true;

        Vector3 targetScale = new Vector3(5f, 5f, 5f);
        float t = 0f;

        while (t < animDuration)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(originalScale, targetScale, t / animDuration);
            yield return null;
        }

        isAnimating = false;
    }
}