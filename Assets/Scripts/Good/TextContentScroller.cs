using UnityEngine;
using TMPro;

public class TextContentScroller : RemixScript
{
    public TextMeshProUGUI uiText;
    public string scrollText = " \\INSERT TEXT HERE\\";
    public int portion;
    public int jump = 1;
    public float speed;
    public int selection;
    public float timer;
    public string loopedText;
    public string currentText;

    public override void StartExt()
    {
        loopedText = scrollText + scrollText + scrollText;
        selection = 0;
    }

    void FixedUpdate()
    {
        if (timer < 10f)
        {
            timer += speed;
        }
        else
        {
            timer = 0f;
            selection+=jump;
            currentText = loopedText.Substring(selection, portion);
            if (selection > (loopedText.Length / 3))
            {
                selection = 0;
            }
        }


        uiText.text = currentText;
    }
}
