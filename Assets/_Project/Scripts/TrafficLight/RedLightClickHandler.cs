using UnityEngine;

public class RedLightClickHandler : MonoBehaviour
{
    private TrafficLight trafficLight;

    void Start()
    {
        trafficLight = GetComponentInParent<TrafficLight>();
    }

    void OnMouseDown()
    {
        trafficLight?.OnRedLightClicked();
    }
}
