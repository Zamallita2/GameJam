using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CanvasColorPanel : MonoBehaviour
{
    public Image panelImage;
    public Color colorGray = Color.gray;
    public Color colorGreen = Color.green;
    public Color colorRed = Color.red;
    private Vector3 originalPos;

    void Start()
    {
        SetGray();
    }

    public void SetGreen() => panelImage.color = colorGreen;
    public void SetGray() => panelImage.color = colorGray;

    public void SetRedThenGray()
    {
        StopAllCoroutines();
        StartCoroutine(RedFlash());
    }

    IEnumerator RedFlash()
    {
        panelImage.color = colorRed;
        yield return new WaitForSeconds(0.5f);
        panelImage.color = colorGray;
    }
}