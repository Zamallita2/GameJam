using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float openDistance = 3f;
    public float openDuration = 1.5f;

    public void Open()
    {
        StartCoroutine(AnimateOpen());
    }

    IEnumerator AnimateOpen()
    {
        Vector3 from = transform.localPosition;
        Vector3 to = from + Vector3.up * openDistance;
        float t = 0f;

        while (t < openDuration)
        {
            t += Time.deltaTime;
            transform.localPosition = Vector3.Lerp(from, to, t / openDuration);
            yield return null;
        }

        transform.localPosition = to;
    }
}