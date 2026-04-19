using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonAnimator : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    [Header("Animacion de Escala")]
    public float pressScale = 0.88f;
    public float hoverScale = 1.06f;
    public float speed = 8f;

    [Header("Sonido")]
    public AudioClip clickSound;
    public AudioClip hoverSound;
    [Range(0, 1)] public float volume = 0.5f;

    private Vector3 targetScale;
    private AudioSource src;

    void Awake()
    {
        targetScale = Vector3.one;
        src = gameObject.AddComponent<AudioSource>();
        src.playOnAwake = false;
    }

    void Update()
    {
        // Interpola suavemente hacia la escala objetivo
        transform.localScale = Vector3.Lerp(
            transform.localScale, targetScale,
            Time.deltaTime * speed
        );
    }

    public void OnPointerDown(PointerEventData e)
    {
        targetScale = Vector3.one * pressScale;
        PlaySound(clickSound);
    }

    public void OnPointerUp(PointerEventData e)
    {
        // Rebote: sube un poco mas antes de volver a 1
        targetScale = Vector3.one * 1.08f;
        StartCoroutine(Bounce());
    }

    IEnumerator Bounce()
    {
        yield return new WaitForSeconds(0.08f);
        targetScale = Vector3.one;
    }

    public void OnPointerEnter(PointerEventData e)
    {
        targetScale = Vector3.one * hoverScale;
        PlaySound(hoverSound);
    }

    public void OnPointerExit(PointerEventData e)
    {
        targetScale = Vector3.one;
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            src.clip = clip;
            src.volume = volume;
            src.Play();
        }
    }
}