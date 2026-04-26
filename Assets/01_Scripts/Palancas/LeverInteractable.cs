using System.Collections;
using UnityEngine;

public class LeverInteractable : MonoBehaviour
{
    [HideInInspector] public int leverID;
    [HideInInspector] public LeverPanel panel;

    public bool isActive = false;
    private bool isAnimating = false;

    private Quaternion rotOff;
    private Quaternion rotOn;
    public float animDuration = 0.4f;

    public Material matNormal;
    public Material matActivo;
    private Renderer rend;

    void Start()
    {
        rend = GetComponentInChildren<Renderer>();
        rotOff = transform.localRotation;
        rotOn = Quaternion.Euler(transform.localEulerAngles + new Vector3(-70f, 0f, 0f));
        if (matNormal != null) rend.material = matNormal;
    }

    void OnMouseDown()
    {
        if (isAnimating) return;
        Toggle();
    }

    public void Toggle()
    {
        isActive = !isActive;
        StartCoroutine(AnimateLever());
        panel.OnLeverToggled();
    }

    IEnumerator AnimateLever()
    {
        isAnimating = true;
        Quaternion from = transform.localRotation;
        Quaternion to = isActive ? rotOn : rotOff;
        float t = 0f;

        while (t < animDuration)
        {
            t += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(from, to, t / animDuration);
            yield return null;
        }

        transform.localRotation = to;

        if (rend != null)
            rend.material = isActive ? matActivo : matNormal;

        isAnimating = false;
    }
}