using UnityEngine;
using RemixNetworking;

public class ReviewReward : GorillaPressableButton
{
    public enum ButtonReviewType{Disabled,Applab,Discord,Archive,Booster,Extern}
    public string cosmeticItemId = "ReviewRewardHat";
    public string catalogVersion = "DLC";
    public string rewardStringIdentifier;
    public bool claimed;
    public Material claimedMaterial;
    public string urlRedir;
    public ButtonReviewType pressType;
    // "https://raw.githubusercontent.com/BasicFalse/RemixVersions/refs/heads/main/ApplabPage.txt"

    public void Start()
    {
        string buttonTextTrue = "ERROR";
        string buttonTextFalse = "ERROR";
        switch (pressType)
        {
            case ButtonReviewType.Applab:
                StartCoroutine(WebUtilities.DownloadTextFromURL(
                "https://raw.githubusercontent.com/BasicFalse/RemixVersions/refs/heads/main/ApplabPage.txt",
                (text) => { urlRedir = text; }
                ));
                buttonTextTrue = "CLAIMED";
                buttonTextFalse = "RATE THE GAME!";
                break;
            case ButtonReviewType.Booster:
                StartCoroutine(WebUtilities.DownloadTextFromURL(
                "https://raw.githubusercontent.com/BasicFalse/RemixVersions/refs/heads/main/GenLinks/GTHDiscordLink.txt",
                (text) => { urlRedir = text; }
                ));
                buttonTextTrue = "CLAIMED";
                buttonTextFalse = "BOOST THE DISCORD";
                break;
                case ButtonReviewType.Extern:
                buttonTextTrue = "CLAIMED";
                buttonTextFalse = "CLAIM";
                break; 
        }
        switch (pressType)
        {
            case ButtonReviewType.Applab:
                if (PlayerPrefs.HasKey(rewardStringIdentifier))
                {
                    if(PlayerPrefs.GetString(rewardStringIdentifier) == "claimed")
                    {
                        myText.text = buttonTextTrue;
                        claimed = true;
                        UpdateColor();
                    }
                }
                else
                {
                    PlayerPrefs.SetString(rewardStringIdentifier, "false");
                    PlayerPrefs.Save();
                    myText.text = buttonTextFalse;
                    claimed = false;
                }
            break;
            case ButtonReviewType.Booster:
                bool didboost = false;
                if (didboost)
                {
                    myText.text = buttonTextTrue;
                    claimed = true;
                    UpdateColor();
                }
                else
                {
                    myText.text = buttonTextFalse;
                    claimed = false;
                }
            break;
            case ButtonReviewType.Extern:
                if (PlayerPrefs.HasKey(rewardStringIdentifier))
                {
                    if(PlayerPrefs.GetString(rewardStringIdentifier) == "claimed")
                    {
                        myText.text = buttonTextTrue;
                        claimed = true;
                        UpdateColor();
                    }
                }
                else
                {
                    PlayerPrefs.SetString(rewardStringIdentifier, "false");
                    PlayerPrefs.Save();
                    myText.text = buttonTextFalse;
                    claimed = false;
                }
            break;
        }
        
    }

    public override void UpdateColor()
    {
        if (ChangeMaterial)
		{
			if (isOn)
			{
				buttonRenderer.material = pressedMaterial;
				
			}
			else
			{
				buttonRenderer.material = unpressedMaterial;
			}
		}

        if (claimed)
        {
            buttonRenderer.material = claimedMaterial;
        }
    }

    public override void ButtonActivation()
    {
        //base.ButtonActivation();
        switch (pressType)
        {
            case ButtonReviewType.Applab:
                OpenWebPage();
                GrantReward();
                break;
            case ButtonReviewType.Booster:
                OpenWebPage();
                break;
            case ButtonReviewType.Extern:
                GrantReward();
                break;
        }
    }

    private void OpenWebPage()
    {
        if(!string.IsNullOrEmpty(urlRedir)){
        Application.OpenURL(urlRedir);}
    }

    private void GrantReward()
    {
        if (string.IsNullOrEmpty(cosmeticItemId))
        {
            Debug.LogError("Cosmetic Item ID not set!");
            return;
        }
    }
}
