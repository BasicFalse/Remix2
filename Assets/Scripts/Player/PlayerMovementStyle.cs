using UnityEngine;

public abstract class PlayerMovementStyle : MonoBehaviour
{
    public PlayerBase @b;
    public abstract void UpdatePlayer();
}
