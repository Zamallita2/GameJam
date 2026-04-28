using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GearPanel : MonoBehaviour
{
    [Header("Engranajes")]
    public GearInteractable[] gearsInOrder;
    public int currentGear =0;
    public float tiempo=1.2f;
    private float timerWin=0;
    private bool isWon=false;
    private bool allActivated = false;

    [Header("Lamp")]
    public Renderer statusLamp;
    public Material lampNormal;
    public Material lampOk;
    public Material lampError;
    public Camera main;

    [Header("Canvas")]
    public CanvasColorPanel canvasPanel;

    private bool panelDone = false;
    public MachineInteraction machineOwner;
    private float timer=0;
    private bool isMove=false;

    public float ajustarX=-0.7f;
    public float ajustarY=-0.4f;
    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip correctSound;
    public AudioClip wrongSound;
    public AudioClip completeSound;
    private void Awake() {
        if (audioSource == null)
                audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Start()
    {
        ShuffleArray(gearsInOrder);

        for (int i = 0; i < gearsInOrder.Length; i++)
        {
            gearsInOrder[i].gearID = i;
            gearsInOrder[i].panel = this;
        }

        if (statusLamp != null && lampNormal != null)
            statusLamp.material = lampNormal;

        if (canvasPanel != null)
            canvasPanel.SetGray();
    }
    void ShuffleArray(GearInteractable[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int randIndex = Random.Range(0, i + 1);

            GearInteractable temp = array[i];
            array[i] = array[randIndex];
            array[randIndex] = temp;
        }
    }
    void Update()
    {
        if (isWon)
        {
            if (timerWin == 0)
            {
                audioSource.PlayOneShot(completeSound);
            }
            if (timerWin>=tiempo)
            {
                PanelCompleted();
            }
            else timerWin+=Time.deltaTime;
        }
        if (timer > 0.5 && !isMove)
        {
            transform.position += new Vector3(ajustarX,ajustarY, -0.11f);  
            isMove=true;
        }
        else if(!isMove)
        {
            timer+=Time.deltaTime;
        }
        if (panelDone) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Ray ray =main.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GearInteractable gear = hit.collider.GetComponentInParent<GearInteractable>();
                if (gear != null)
                    gear.OnClicked();
            }
        }
    }

    public void OnGearClicked(GearInteractable gear)
    {
        if (panelDone || gear.IsActivated()) return;

        if (gear.gearID == currentGear)
        {
            gear.SetActivo();
            currentGear++;
            audioSource.PlayOneShot(correctSound);
        }
        else
        {
            foreach (var g in gearsInOrder)
            {
                g.SetNormal();
            }
            currentGear = 0;
            audioSource.PlayOneShot(wrongSound);
        }

        // comprobar si todos están activados
        bool allActivated = true;

        foreach (var g in gearsInOrder)
        {
            if (!g.IsActivated())
            {
                allActivated = false;
                break;
            }
        }

        if (allActivated)
        {
            isWon=true;
            if (canvasPanel != null)
                canvasPanel.SetGreen();

        }
    }

    void PanelCompleted()
    {
        panelDone = true;

        if (statusLamp != null && lampOk != null)
            statusLamp.material = lampOk;

        if (canvasPanel != null)
            canvasPanel.SetGreen();

        Debug.Log("Panel Engranajes completado!");
        machineOwner.MarcarMaquinaReparada();
        machineOwner.CerrarPanelDesdeMinijuego();
    }
}