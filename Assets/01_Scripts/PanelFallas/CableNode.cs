using UnityEngine;
using UnityEngine.UI;

public class CableNode : MonoBehaviour
{
    public int cableId;
    public bool isOrigin;
    public Image image;

    private FallaCablesManager manager;

    public void Init(FallaCablesManager managerRef, int id, bool origin)
    {
        manager = managerRef;
        cableId = id;
        isOrigin = origin;
    }

    public void OnClickNode()
    {
        if (manager != null)
            manager.SelectNode(this);
    }
}