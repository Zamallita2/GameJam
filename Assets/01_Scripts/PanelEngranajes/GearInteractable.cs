using System.Collections;
using UnityEngine;

public class GearInteractable : MonoBehaviour
{
    [HideInInspector] public int gearID;
    [HideInInspector] public GearPanel panel;

    private bool activated = false;
    private bool isAnimating = false;
    private Quaternion originalRot;
    private Vector3 originalPos;

    public float animDuration = 1.4f;

    void Start()
    {
        originalRot = transform.rotation;
        originalPos = transform.localPosition;
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
        StartCoroutine(IrAlFrente());
    }

    public void SetNormal()
    {
        activated = false;
        isAnimating = false;
        StopAllCoroutines();
        transform.rotation = originalRot;
        transform.localPosition = originalPos;
    }

    public void SetError() { }
    public bool IsActivated() => activated;

    IEnumerator IrAlFrente()
    {
        isAnimating = true;
        Vector3 target = originalPos + new Vector3(0f, 0f, -3f);
        float t = 0f;

        while (t < animDuration)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(originalPos, target, t / animDuration);
            yield return null;
        }

        isAnimating = false;
    }
}