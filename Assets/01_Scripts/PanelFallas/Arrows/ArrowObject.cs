using UnityEngine;

public class ArrowObject : MonoBehaviour
{
    public int directionId; // 0=Up,1=Right,2=Down,3=Left
    public float speed = 2f;

    private Vector3 target;

    public void Init(int dir, Vector3 targetPos)
    {
        directionId = dir;
        target = targetPos;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject); // si no lo presionaron
        }
    }
}