using UnityEngine;

[RequireComponent(typeof(Light))]
public class RainbowLight : MonoBehaviour
{
    public float speed = 1f; // Speed at which the color changes
    private Light _light;
    private float _hue;

    void Start()
    {
        // Get the Light component attached to the GameObject
        _light = GetComponent<Light>();
    }

    void Update()
    {
        // Increment the hue value based on time and speed
        _hue += speed * Time.deltaTime;
        if (_hue > 1f) _hue -= 1f; // Wrap around the hue value

        // Convert HSV to RGB and apply it to the light's color
        _light.color = Color.HSVToRGB(_hue, 1f, 1f);
    }
}