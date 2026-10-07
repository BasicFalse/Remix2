using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconSpin : MonoBehaviour
{
    [SerializeField]
    private GameObject iconObject;

    private void Update()
    {
        iconObject.transform.Rotate(0, 0, -80 * Time.deltaTime);
    }
}
