using UnityEngine;

public class ResonatorTrigger : MonoBehaviour
{
    public Resonator Rs;
    public void OnTriggerEnter(Collider other)
    {
        Rs.PlayerEnterResonator();
    }
}
