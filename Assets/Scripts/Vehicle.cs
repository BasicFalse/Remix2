using UnityEngine;

public class Vehicle : MonoBehaviour
{
    protected Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartEXT();
    }

    public virtual void StartEXT() {}
}
