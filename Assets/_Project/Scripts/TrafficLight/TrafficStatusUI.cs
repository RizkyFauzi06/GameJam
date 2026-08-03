using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrafficStatusUI : MonoBehaviour
{
    [Header("Traffic Lights")]
    public TrafficLight north;
    public TrafficLight east;
    public TrafficLight south;
    public TrafficLight west;

    [Header("Icons")]
    public Image northIcon;
    public Image eastIcon;
    public Image southIcon;
    public Image westIcon;

    [Header("Text")]
    public TMP_Text northText;
    public TMP_Text eastText;
    public TMP_Text southText;
    public TMP_Text westText;

    [Header("Sprites")]
    public Sprite redSprite;
    public Sprite yellowSprite;
    public Sprite greenSprite;

    void Update()
    {
        UpdateLamp(north, northIcon, northText, "North");
        UpdateLamp(east, eastIcon, eastText, "East");
        UpdateLamp(south, southIcon, southText, "South");
        UpdateLamp(west, westIcon, westText, "West");
    }

    void UpdateLamp(TrafficLight light, Image icon, TMP_Text text, string direction)
    {
        text.text = direction;

        switch (light.currentState)
        {
            case TrafficLight.LightState.Red:
                icon.sprite = redSprite;
                break;

            case TrafficLight.LightState.Yellow:
                icon.sprite = yellowSprite;
                break;

            case TrafficLight.LightState.Green:
                icon.sprite = greenSprite;
                break;
        }
    }
}