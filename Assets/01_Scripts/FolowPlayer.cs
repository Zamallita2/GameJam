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
    public float porcentajeBuscado = 0.9f;

    private TimeManager timeManager;
    private bool cambiado = false;
    private bool audioDetenido = false;

    void Awake()
    {
        if (alarmSource == null)
            alarmSource = gameObject.AddComponent<AudioSource>();

        if (clockSource == null)
            clockSource = gameObject.AddComponent<AudioSource>();

        timeManager = GetComponent<TimeManager>();
        if (timeManager == null)
            timeManager = FindAnyObjectByType<TimeManager>();
    }

    void Start()
    {
        if (alarmSource != null && alarm != null)
        {
            alarmSource.clip = alarm;
            alarmSource.loop = true;
            alarmSource.volume = volAlarm;
            alarmSource.Play();
        }

        if (clockSource != null && clock1 != null)
        {
            clockSource.clip = clock1;
            clockSource.loop = true;
            clockSource.volume = volClock1;
            clockSource.Play();
        }
    }

    void Update()
    {
        if (player != null && player.gameObject.activeInHierarchy)
            transform.position = player.position;

        if (audioDetenido) return;

        if (timeManager != null && !cambiado)
        {
            float porcentaje = timeManager.timer / timeManager.limitTime;

            if (porcentaje >= porcentajeBuscado)
            {
                CambiarAClock2();
                cambiado = true;
            }
        }
    }

    void CambiarAClock2()
    {
        if (clockSource == null || clock2 == null) return;

        clockSource.Stop();
        clockSource.clip = clock2;
        clockSource.volume = volClock2;
        clockSource.loop = true;
        clockSource.Play();
    }

    public void DetenerAlarma()
    {
        audioDetenido = true;

        if (alarmSource != null)
            alarmSource.Stop();

        if (clockSource != null)
            clockSource.Stop();
    }
}