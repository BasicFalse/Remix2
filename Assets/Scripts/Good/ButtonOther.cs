using UnityEngine;

public class ButtonOther : MonoBehaviour
{
    public GorillaPressableButton gpb;
    void OnTriggerEnter(Collider other)
    {
        gpb.TriggerFromOther(other);
    }
}
