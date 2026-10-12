using UnityEngine;

public class JumpscareCaller : MonoBehaviour
{
    public string jumpscareID;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("GorillaPlayer"))
        {
            JumpscareVisuals.i.CallJumpscare(jumpscareID);
        }
    }
}
