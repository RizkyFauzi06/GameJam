using UnityEngine;
using UnityEngine.Events;

public class TrafficLight : MonoBehaviour
{
    public enum LightState { Red, Yellow, Green }

    [Header("State")]
    public LightState currentState = LightState.Red;

    // Internal step buat nentuin urutan siklus: 
    // 0 = Merah, 1 = Kuning (menuju Hijau), 2 = Hijau, 3 = Kuning (menuju Merah)
    private int cycleStep = 0;

    [Header("Renderers")]
    [SerializeField] private Renderer redLight;
    [SerializeField] private Renderer yellowLight;
    [SerializeField] private Renderer greenLight;

    [Header("Materials")]
    [SerializeField] private Material redOn;
    [SerializeField] private Material yellowOn;
    [SerializeField] private Material greenOn;
    [SerializeField] private Material lightOff;

    [Header("Debug")]
    [SerializeField] private bool allowSpaceKeyDebug = true;

    public UnityEvent<LightState> OnStateChanged;

    void Start()
    {
        UpdateLights();
    }

    void Update()
    {
        if (allowSpaceKeyDebug && Input.GetKeyDown(KeyCode.Space))
        {
            NextState();
        }
    }

    void OnMouseDown()
    {
        NextState();
    }

    public void NextState()
    {
        cycleStep = (cycleStep + 1) % 4;

        currentState = cycleStep switch
        {
            0 => LightState.Red,
            1 => LightState.Yellow,
            2 => LightState.Green,
            3 => LightState.Yellow,
            _ => LightState.Red
        };

        UpdateLights();
        OnStateChanged?.Invoke(currentState);
    }

    void UpdateLights()
    {
        redLight.sharedMaterial = lightOff;
        yellowLight.sharedMaterial = lightOff;
        greenLight.sharedMaterial = lightOff;

        switch (currentState)
        {
            case LightState.Red:
                redLight.sharedMaterial = redOn;
                break;
            case LightState.Yellow:
                yellowLight.sharedMaterial = yellowOn;
                break;
            case LightState.Green:
                greenLight.sharedMaterial = greenOn;
                break;
        }
    }

    public bool BolehJalan() => currentState == LightState.Green;
    public bool WajibBerhenti() => currentState == LightState.Red;

    public void OnRedLightClicked()
    {
        NextState();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
            UpdateLights();
    }
}