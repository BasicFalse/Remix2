using UnityEngine;

public class Leveler : MonoBehaviour
{
    public Quaternion levelRotation = Quaternion.Euler(0, 0, 0);
    void Update()
    {
        transform.rotation = levelRotation;
    }
    void FixedUpdate()
    {
        transform.rotation = levelRotation;
    }
}
