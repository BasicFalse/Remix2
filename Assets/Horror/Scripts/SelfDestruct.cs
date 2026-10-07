using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float lifetime = 5f;  // Time before the GameObject destroys itself

    private void Start()
    {
        // Schedule the self-destruction after the specified lifetime
        Destroy(gameObject, lifetime);
    }
}