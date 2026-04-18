using UnityEngine;
using UnityEngine.UI;

public class PuzzleButton : MonoBehaviour
{
    public int buttonId;
    public Image image;
    public Color normalColor = Color.white;
    public Color activeColor = Color.yellow;

    private FallaBotonesManager manager;

    public void Init(FallaBotonesManager managerRef, int id)
    {
        manager = managerRef;
        buttonId = id;
        SetNormal();
    }

    public void OnPress()
    {
        if (manager != null)
            manager.PlayerPressed(buttonId);

        HighlightTemporary();
    }

    public void SetNormal()
    {
        if (image != null)
            image.color = normalColor;
    }

    public void SetActive()
    {
        if (image != null)
            image.color = activeColor;
    }

    public void HighlightTemporary()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        SetActive();
        yield return new WaitForSeconds(0.25f);
        SetNormal();
    }
}