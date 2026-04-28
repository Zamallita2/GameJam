using UnityEngine;

public class FitObjectToCamera : MonoBehaviour
{
    public Transform target; // tu panel

    void Start()
    {
        Camera cam = GetComponent<Camera>();

        Renderer rend = target.GetComponentInChildren<Renderer>();
        Bounds bounds = rend.bounds;

        float objectSize = Mathf.Max(bounds.size.x, bounds.size.y);

        float distance = objectSize / (2f * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad));

        transform.position = bounds.center - transform.forward * distance;
    }
}