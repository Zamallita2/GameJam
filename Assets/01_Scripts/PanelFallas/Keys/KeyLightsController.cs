using System.Collections;
using UnityEngine;

public class KeyLightsController : MonoBehaviour
{
    [Header("Luces de los botones")]
    public Light lightA;
    public Light lightS;
    public Light lightD;
    public Light lightF;

    [Header("Duración")]
    public float lightDuration = 0.15f;

    void Awake()
    {
        TurnOffAll();
    }

    public void Flash(KeySymbolType symbol)
    {
        Light selectedLight = GetLight(symbol);

        if (selectedLight != null)
            StartCoroutine(FlashRoutine(selectedLight));
    }

    public void FlashError(KeySymbolType symbol)
    {
        Light selectedLight = GetLight(symbol);

        if (selectedLight != null)
            StartCoroutine(FlashRoutine(selectedLight));
    }

    IEnumerator FlashRoutine(Light targetLight)
    {
        targetLight.enabled = true;
        yield return new WaitForSeconds(lightDuration);
        targetLight.enabled = false;
    }

    Light GetLight(KeySymbolType symbol)
    {
        switch (symbol)
        {
            case KeySymbolType.A:
                return lightA;

            case KeySymbolType.S:
                return lightS;

            case KeySymbolType.D:
                return lightD;

            case KeySymbolType.F:
                return lightF;
        }

        return null;
    }

    public void TurnOffAll()
    {
        if (lightA != null) lightA.enabled = false;
        if (lightS != null) lightS.enabled = false;
        if (lightD != null) lightD.enabled = false;
        if (lightF != null) lightF.enabled = false;
    }
}
