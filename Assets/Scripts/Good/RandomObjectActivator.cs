using System.Collections.Generic;
using UnityEngine;

public class RandomObjectActivator : RemixScript
{
    public GameObject[] screens; // Assign objects in the Inspector
    public GameObject[] Titles;
    private int randomIndex1;
    private int randomIndex2;

    public override void StartExt()
    {
        if (screens.Length == 0)
        {
            Debug.LogWarning("No screens assigned to the array.");
            return;
        }

        // Seed the random generator with the current time
        Random.InitState(System.DateTime.Now.Second);

        // Disable all screens
        for (int i = 0; i < screens.Length; i++)
        {
            if (screens[i] != null)
                screens[i].SetActive(false);
        }
        for (int i = 0; i < Titles.Length; i++)
        {
            if (Titles[i] != null)
                Titles[i].SetActive(false);
        }

        string lastScreen = PlayerPrefs.GetString("LastLoadScreen", "");
        string lastTitle = PlayerPrefs.GetString("LastLoadTitle", "");
        string selectedScreen;
        string selectedTitle;

        do
        {
            randomIndex1 = Random.Range(0, screens.Length);
            selectedScreen = screens[randomIndex1] != null ? screens[randomIndex1].name : "";
        } while (selectedScreen == lastScreen);


        Random.InitState(System.DateTime.Now.Minute);

        do
        {
            randomIndex2 = Random.Range(0, Titles.Length);
            selectedTitle = Titles[randomIndex2] != null ? Titles[randomIndex2].name : "";
        } while (selectedTitle == lastTitle);

        // Save the selected screen name
        PlayerPrefs.SetString("LastLoadScreen", selectedScreen);
        PlayerPrefs.SetString("LastLoadTitle", selectedTitle);
        PlayerPrefs.Save();

        // Activate the chosen screen
        if (screens[randomIndex1] != null)
        {
            screens[randomIndex1].SetActive(true);
        }

        if (Titles[randomIndex2] != null)
        {
            Titles[randomIndex2].SetActive(true);
        }
    }
}
