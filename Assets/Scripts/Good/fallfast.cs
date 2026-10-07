using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class FallFast : MonoBehaviour
{
    public float speedX = 0.1f;
    public float speedY = 0.1f;

    private Renderer rend;
    private Vector2 offset;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        offset = rend.material.mainTextureOffset;
    }

    void Update() // use Update for visual effects
    {
        offset.x += speedX * Time.deltaTime;
        offset.y += speedY * Time.deltaTime;

        rend.material.SetTextureOffset("_MainTex", offset);
    }
}
