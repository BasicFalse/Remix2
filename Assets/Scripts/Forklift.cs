using Unity.Cinemachine;
using UnityEngine;

public class Forklift : Vehicle
{
    public void FixedUpdate()
    {

        Vector3 FowardFlat = Vector3.ProjectOnPlane(transform.forward, Vector3.up);

        Vector3 VelocityFlat = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);

        
        Vector3 SideMovement = Vector3.ProjectOnPlane(VelocityFlat, FowardFlat);
        rb.AddForce(-SideMovement, ForceMode.VelocityChange);
    }
}
