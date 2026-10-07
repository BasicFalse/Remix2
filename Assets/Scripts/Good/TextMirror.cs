using UnityEngine;
using UnityEngine.UI;

public class TextMirror : MonoBehaviour
{
    public Text baseText;
    public Text mirrorText;

    void Update()
    {
        if (baseText != null && mirrorText != null)
        {
            mirrorText.text = baseText.text;
        }
    }
}
