using UnityEngine;

public class PuzzleLever : MonoBehaviour
{
    public int leverId;
    private FallaPalancasManager manager;

    public void Init(FallaPalancasManager managerRef, int id)
    {
        manager = managerRef;
        leverId = id;
    }

    public void OnPullLever()
    {
        if (manager != null)
            manager.PlayerPulledLever(leverId);
    }
}