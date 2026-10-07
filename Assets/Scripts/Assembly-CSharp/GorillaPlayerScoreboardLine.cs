using System;
using ExitGames.Client.Photon;
using GorillaNetworking;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;
using UnityEngine;
using UnityEngine.UI;
using GorillaRigging;

public class GorillaPlayerScoreboardLine : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
	public Text playerName;
	public Text playerLevel;
	public Text playerMMR;
	public Image playerSwatch;
	public Player linePlayer;
	public RigController playerVRRig;
    //public event Action<Color> OnColorChanged;
	//public int currentMatIndex;
	public string playerLevelValue;
	public string playerMMRValue;
	public string playerNameValue;
	public int playerActorNumber;
	public bool initialized;
	public bool extended = false;
	public GorillaPlayerLineButton optionsButton;
	public GorillaPlayerLineButton muteButton;
	public GorillaPlayerLineButton reportButton;
	public GorillaPlayerLineButton frndButton; // Friend button
	public GameObject speakerIcon;
	public GameObject flashlightIndicator;
	public SpriteRenderer statusMon;
	public bool canPressNextButton = true;
	public Text[] texts;
	public SpriteRenderer[] sprites;
	public MeshRenderer[] meshes;
	public Image[] images;
	public bool eventMuteSet;

	// internal cached material for this Image to avoid shared-material bleed
	private Material _instancedSwatchMaterial;
	private Color _lastAppliedSwatch = Color.clear;

	public void InitializeLine()
	{
		// Ensure we have a unique material for this UI Image so changes do not affect other rows
		if (playerSwatch != null)
		{
			try
			{
				if (playerSwatch.material != null)
				{
					_instancedSwatchMaterial = new Material(playerSwatch.material);
					playerSwatch.material = _instancedSwatchMaterial;
				}
			}
			catch (Exception ex)
			{
				Debug.LogWarning($"[ScoreboardLine] Failed to instance swatch material: {ex.Message}");
			}
		}

        // instance the UI material (you already have this)
        if (playerSwatch != null && _instancedSwatchMaterial == null)
        {
            if (playerSwatch.material != null)
            {
                _instancedSwatchMaterial = new Material(playerSwatch.material);
                playerSwatch.material = _instancedSwatchMaterial;
            }
        }

        // Subscribe to rig event if rig is already assigned
        if (playerVRRig != null)
        {
            // avoid double-subscription
            playerVRRig.OnColorChanged -= HandleColorChanged;
            playerVRRig.OnColorChanged += HandleColorChanged;

            // Set initial color immediately from renderer or cached material
            Color initColor = Color.white;
            if (playerVRRig.thisGuysSkin != null && playerVRRig.thisGuysSkin.material != null)
                initColor = playerVRRig.thisGuysSkin.material.color;
            else if (playerVRRig.playerMaterial != null)
                initColor = playerVRRig.playerMaterial.color;

            HandleColorChanged(initColor); // apply right away
        }

		// Try to immediately set an initial color from the rig if available
		UpdateSwatchFromRig(force: true);
	}

	public void CleanupLine()
	{
		// Unregister any listeners in future if you add them; clean up instantiated material
		if (_instancedSwatchMaterial != null)
		{
			UnityEngine.Object.Destroy(_instancedSwatchMaterial);
			_instancedSwatchMaterial = null;
		}
	}

	public void HideShowLine(bool active)
	{
		if (playerVRRig == null)
		{
			return;
		}
		foreach (Text text in texts)
		{
			text.enabled = active;
		}
		foreach (SpriteRenderer spriteRenderer in sprites)
		{
			spriteRenderer.enabled = active;
		}
		foreach (MeshRenderer meshRenderer in meshes)
		{
			meshRenderer.enabled = active;
		}
		foreach (Image image in images)
		{
			image.enabled = active;
		}
	}

	public void Update()
	{
		GameObject networkVoiceObject = MiscGrabs.grab.networkedVC;
		
		if (playerVRRig != null)
		{
			if (!initialized && linePlayer != null)
			{
				initialized = true;
				if (linePlayer != PhotonNetwork.LocalPlayer)
				{
					int muteState = PlayerPrefs.GetInt($"{linePlayer.UserId}_mute", 0); // Get mute state
					muteButton.isOn = muteState != 0;
					muteButton.UpdateColor();
					playerVRRig.muted = muteState != 0;

					int friendState = PlayerPrefs.GetInt($"{linePlayer.UserId}_friend", 0); // Get friend state
					frndButton.isOn = friendState != 0;
					frndButton.UpdateColor();

					muteButton.gameObject.SetActive(false);
					reportButton.gameObject.SetActive(false);
					frndButton.gameObject.SetActive(false);
					optionsButton.gameObject.SetActive(true);
				}
				else
				{
					muteButton.gameObject.SetActive(false);
					reportButton.gameObject.SetActive(false);
					frndButton.gameObject.SetActive(false);
					optionsButton.gameObject.SetActive(false);
				}
				if (playerVRRig.mutedTemp)
				{
					eventMuteSet = true;
					playerVRRig.muted = true;
				}
				else if(eventMuteSet)
				{
					int muteState = PlayerPrefs.GetInt($"{linePlayer.UserId}_mute", 0); // Get mute state
					muteButton.isOn = muteState != 0;
					muteButton.UpdateColor();
					playerVRRig.muted = muteState != 0;
					eventMuteSet = false;
				}
			}

			if (linePlayer != null)
			{
				if (playerName.text != linePlayer.NickName)
				{
					playerName.text = NormalizeName(true, linePlayer.NickName);
				}
				if (playerMMRValue != playerMMR.text)
				{
					playerMMR.text = playerMMRValue;
				}
				if (playerLevelValue != playerLevel.text)
				{
					playerLevel.text = playerLevelValue;
				}

				if (GetComponentInParent<GorillaScoreBoard>().includeMMR && !playerMMR.gameObject.activeSelf)
				{
					playerMMR.gameObject.SetActive(true);
				}

				if (playerVRRig.isFlashlightOn)
				{
					flashlightIndicator.SetActive(true);
				}
				else
				{
					flashlightIndicator.SetActive(false);
				}

				if (playerVRRig.photonView.IsMine && networkVoiceObject.GetComponent<Recorder>().IsCurrentlyTransmitting)
				{
					speakerIcon.SetActive(true);
				}
				else
				{
					speakerIcon.SetActive(false);
				}

				// Update the swatch from the rig but only when it actually changes
				UpdateSwatchFromRig();

				if (playerVRRig.isGettingJumped)
				{
					statusMon.color = Color.red;
				}
				else if (playerVRRig.isNearMonster)
				{
					statusMon.color = Color.yellow;
				}
				else
				{
					statusMon.color = Color.green;
				}
			}
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void UpdateSwatchFromRig(bool force = false)
	{
		if (playerSwatch == null || playerVRRig == null)
			return;

		Color desired = Color.white;
		// Prefer actual renderer instance color if available (thisGuysSkin.material)
		if (playerVRRig.thisGuysSkin != null && playerVRRig.thisGuysSkin.material != null)
		{
			desired = playerVRRig.thisGuysSkin.material.color;
		}
		else if (playerVRRig.playerMaterial != null)
		{
			desired = playerVRRig.playerMaterial.color;
		}

		if (force || desired != _lastAppliedSwatch)
		{
			playerSwatch.color = desired;
			_lastAppliedSwatch = desired;
		}
	}

	public void PressButton(bool isOn, GorillaPlayerLineButton.ButtonType buttonType)
	{
		switch (buttonType)
		{
			case GorillaPlayerLineButton.ButtonType.Mute:
				if (linePlayer != null && playerVRRig != null)
				{
					int muteState = isOn ? 1 : 0;
					PlayerPrefs.SetInt($"{linePlayer.UserId}_mute", muteState); // Use unique key for mute
					playerVRRig.muted = muteState != 0; // Update muted state
					PlayerPrefs.Save();
					muteButton.UpdateColor();
				}
				break;

			case GorillaPlayerLineButton.ButtonType.Report:
				SetReportState(true, buttonType);
				break;

			case GorillaPlayerLineButton.ButtonType.Frnd: // Friend button case
				if (linePlayer != null)
				{
					int friendState = isOn ? 1 : 0;
					PlayerPrefs.SetInt($"{linePlayer.UserId}_friend", friendState); // Use unique key for friend
					PlayerPrefs.Save();
					frndButton.UpdateColor();
				}
				break;

			case GorillaPlayerLineButton.ButtonType.Options: // Options button case
				SetOptionsState(isOn,buttonType);
				break;

			default:
				SetReportState(false, buttonType);
				break;
		}
	}

	public void SetOptionsState(bool isOn, GorillaPlayerLineButton.ButtonType buttonType)
	{
		canPressNextButton = buttonType != GorillaPlayerLineButton.ButtonType.Options;
		extended = !extended;

		muteButton.gameObject.SetActive(extended);
		reportButton.gameObject.SetActive(false);
		frndButton.gameObject.SetActive(extended);
	}

	public void SetReportState(bool reportState, GorillaPlayerLineButton.ButtonType buttonType)
	{
		canPressNextButton = buttonType != GorillaPlayerLineButton.ButtonType.Toxicity && buttonType != GorillaPlayerLineButton.ButtonType.Report;

		if (reportState)
		{
			foreach (var button in GetComponentsInChildren<GorillaPlayerLineButton>(true))
			{
				button.gameObject.SetActive(
								button.buttonType != GorillaPlayerLineButton.ButtonType.Report ||
								button.buttonType != GorillaPlayerLineButton.ButtonType.Mute ||
								button.buttonType != GorillaPlayerLineButton.ButtonType.Frnd ||
								button.buttonType != GorillaPlayerLineButton.ButtonType.Options);
			}
		}
		else
		{
			foreach (var button in GetComponentsInChildren<GorillaPlayerLineButton>(true))
			{
				button.gameObject.SetActive(
						button.buttonType == GorillaPlayerLineButton.ButtonType.Report ||
						button.buttonType == GorillaPlayerLineButton.ButtonType.Mute ||
						button.buttonType == GorillaPlayerLineButton.ButtonType.Frnd ||
						button.buttonType == GorillaPlayerLineButton.ButtonType.Options);
			}

			if (linePlayer != null && playerVRRig != null && buttonType != GorillaPlayerLineButton.ButtonType.Cancel)
			{
				ReportPlayer(linePlayer.UserId, buttonType, linePlayer.NickName);
				reportButton.isOn = true;
				reportButton.UpdateColor();
			}
		}

		transform.parent.GetComponent<GorillaScoreBoard>().RedrawPlayerLines();
	}

	public void ReportPlayer(string PlayerID, GorillaPlayerLineButton.ButtonType buttonType, string OtherPlayerNickName)
	{
		RaiseEventOptions options = new RaiseEventOptions { Flags = new WebFlags(1) };
		PhotonNetwork.RaiseEvent(50, new object[] { PlayerID, buttonType, OtherPlayerNickName, PhotonNetwork.LocalPlayer.NickName, PhotonNetwork.CurrentRoom.IsVisible }, options, SendOptions.SendReliable);
	}

	public Player FindPlayerforVRRig(RigController vRRig)
	{
		return vRRig.photonView?.Owner;
	}

	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			text = new string(Array.FindAll(text.ToCharArray(), char.IsLetterOrDigit));
			text = text.Length > 12 ? text.Substring(0, 12) : text;
			text = text.ToUpper();
		}
		return text;
	}

    private void HandleColorChanged(Color newColor)
    {
        // Ensure we have a unique material for this UI Image (already done in InitializeLine)
        if (playerSwatch != null)
        {
            playerSwatch.color = newColor;
            _lastAppliedSwatch = newColor; // if you're using caching
        }
    }

	private void OnDestroy()
    {
        CleanupLine(); // your cleanup already destroys the instanced material
    
        if (playerVRRig != null)
        {
            playerVRRig.OnColorChanged -= HandleColorChanged;
        }
    }
}
