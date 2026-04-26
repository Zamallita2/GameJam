using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    [Header("Audio Sources")]
    public AudioSource alarmSource;
    public AudioSource clockSource;

    [Header("Clips")]
    public AudioClip alarm;
    public AudioClip clock1;
    public AudioClip clock2;

    [Header("Volúmenes")]
    [Range(0f, 1f)] public float volAlarm = 0.5f;
    [Range(0f, 1f)] public float volClock1 = 0.5f;
    [Range(0f, 1f)] public float volClock2 = 0.8f;
    public float porcentajeBuscado=0.9f;

    private TimeManager timeManager;
    private bool cambiado = false;

    void Awake()
    {
        if (alarmSource == null)
            alarmSource = gameObject.AddComponent<AudioSource>();

        if (clockSource == null)
            clockSource = gameObject.AddComponent<AudioSource>();

        timeManager = GetComponent<TimeManager>();
    }

    void Start()
    {
        // 🎶 Alarm SIEMPRE
        alarmSource.clip = alarm;
        alarmSource.loop = true;
        alarmSource.volume = volAlarm;
        alarmSource.Play();

        // ⏰ Clock1 inicio
        clockSource.clip = clock1;
        clockSource.loop = true;
        clockSource.volume = volClock1;
        clockSource.Play();
    }

    void Update()
    {
        // Follow player 🐾
        if (player != null && player.gameObject.activeInHierarchy)
            transform.position = player.position;

        // Cambio al 90% ⏰⚠️
        if (timeManager != null && !cambiado)
        {
            float porcentaje = timeManager.timer / timeManager.limitTime;

            if (porcentaje >=porcentajeBuscado)
            {
                CambiarAClock2();
                cambiado = true;
            }
        }
    }

    void CambiarAClock2()
    {
        clockSource.Stop();
        clockSource.clip = clock2;
        clockSource.volume = volClock2;
        clockSource.loop = true;
        clockSource.Play();
    }
}