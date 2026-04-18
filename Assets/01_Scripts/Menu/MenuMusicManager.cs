using UnityEngine;

public class MenuMusicManager : MonoBehaviour
{
    public static MenuMusicManager instance;

    [Header("Musica")]
    public AudioClip menuMusic;
    [Range(0, 1)] public float volume = 0.4f;
    public bool loop = true;

    private AudioSource src;

    void Awake()
    {
        // Singleton: si ya existe uno, destruye este duplicado
        if (instance != null) { Destroy(gameObject); return; }
        instance = this;

        // No se destruye al cargar otra escena
        DontDestroyOnLoad(gameObject);

        src = GetComponent<AudioSource>();
        src.clip = menuMusic;
        src.volume = volume;
        src.loop = loop;
        src.Play();
    }

    // Llama esto desde tu script de GameOver o pantalla de juego
    public void StopMusic() => src.Stop();

    // Llama esto para subir/bajar el volumen
    public void SetVolume(float v) => src.volume = v;
}