using UnityEngine;

// Attach this to any GameObject that uses the InvertBehind shader.
// The renderer feature uses it to find and sort inverter objects.
[RequireComponent(typeof(Renderer))]
public class InvertMarker : MonoBehaviour { }