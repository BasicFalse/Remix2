// Patched GorillaScoreBoard.cs
using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using GorillaRigging;

namespace GorillaNetworking
{
public class GorillaScoreBoard : MonoBehaviourPunCallbacks, IInRoomCallbacks, IOnEventCallback
{
	public GameObject scoreBoardLinePrefab;

	public int startingYValue;

	public int lineHeight;

	public bool includeMMR;

	public List<GorillaPlayerScoreboardLine> lines;

	public Text boardText;

	private int i;

	public void Awake()
	{
		PhotonNetwork.AddCallbackTarget(this);
		if (PhotonNetwork.InRoom)
		{
			Debug.Log("Weareinaroomwitharoommanagerinstanceolkorrect");
			boardText.text = GetBeginningString();
		}
	}

	public string GetBeginningString()
	{
		if (PhotonRoomManagerMain.instance != null)
		{
			return "ROOM ID: " + ((!PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE- GAME MODE: " : (PhotonNetwork.CurrentRoom.Name + "    GAME MODE: ")) + PhotonRoomManagerMain.instance.GameMode() + "\n   PLAYER     COLOR  OPTIONS";
		}
		return "ROOM ID: " + ((!PhotonNetwork.CurrentRoom.IsVisible) ? "-PRIVATE-" : PhotonNetwork.CurrentRoom.Name) + "\n   PLAYER     COLOR  OPTIONS";
	}

	public void Update()
	{
		for (i = lines.Count - 1; i > -1; i--)
		{
			if (lines[i] == null)
			{
				lines.RemoveAt(i);
			}
		}
		if (PhotonNetwork.CurrentRoom == null)
			return;
		if (PhotonRoomManagerMain.instance == null)
			return;
		if(lines.Count == PhotonRoomManagerMain.instance.rigs.Count)
			return;
		foreach (RigController Rig in PhotonRoomManagerMain.instance.rigs)
		{
			Player player = FindPlayerforVRRig(Rig);
			if (player == null)
			{
				continue;
			}
			bool flag = false;
			foreach (GorillaPlayerScoreboardLine line in lines)
			{
				if (line.playerActorNumber == player.ActorNumber)
				{
					flag = true;
				}
			}
			if (!flag)
			{
				CreateNewPlayerLine(player, Rig);
			}
		}
	}

		public void CreateNewPlayerLine(Player player, RigController rig)
		{
				GameObject gameObject = UnityEngine.Object.Instantiate(scoreBoardLinePrefab, base.transform);
				GorillaPlayerScoreboardLine lineComponent = gameObject.GetComponent<GorillaPlayerScoreboardLine>();
				lines.Add(lineComponent);
				if (includeMMR)
				{
					lineComponent.playerMMR.gameObject.SetActive(value: true);
				}
				lineComponent.playerActorNumber = player.ActorNumber;
				lineComponent.linePlayer = player;
				lineComponent.playerVRRig = rig;
				lineComponent.playerNameValue = player.NickName;
				lineComponent.InitializeLine();
				RedrawPlayerLines();
		}

		public void CreateNewPlayerLine(RigController rig)
		{
			Player solvedPlayer = GetPlayerForRigControllerExternal(rig);
			if(solvedPlayer == null)
			{
				Debug.Log("Cannot find player for RigController! Escalating!", rig);
				PhotonRoomManagerMain.instance.DealWithNullPlayerRig(rig);
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(scoreBoardLinePrefab, base.transform);
			GorillaPlayerScoreboardLine lineComponent = gameObject.GetComponent<GorillaPlayerScoreboardLine>();
			lines.Add(lineComponent);
			if (includeMMR)
			{
				lineComponent.playerMMR.gameObject.SetActive(value: true);
			}
			lineComponent.playerActorNumber = solvedPlayer.ActorNumber;
			lineComponent.linePlayer = solvedPlayer;
			lineComponent.playerVRRig = rig;
			lineComponent.playerNameValue = solvedPlayer.NickName;
			lineComponent.InitializeLine();
			RedrawPlayerLines();
		}

		public Player GetPlayerForRigControllerExternal(RigController rig)
		{
			if(GorillaComputer.instance.isInRoom)
				for(int i = 0; i < PhotonRoomManagerMain.instance.currentPlayerArray.Length; i++)
				{
					if(PhotonRoomManagerMain.instance.FindVRRigControllerForPlayer(PhotonRoomManagerMain.instance.currentPlayerArray[i]) == rig)
						return PhotonRoomManagerMain.instance.currentPlayerArray[i];
				}
			return null;
		}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		foreach (GorillaPlayerScoreboardLine line in lines)
		{
			if (line.playerActorNumber == otherPlayer.ActorNumber)
			{
				Debug.Log("destroying this", line.gameObject);
				line.CleanupLine();
				line.enabled = false;
				lines.Remove(line);
				UnityEngine.Object.Destroy(line.gameObject);
				break;
			}
		}
		RedrawPlayerLines();
	}

	public void RedrawPlayerLines()
	{
		lines.Sort((line1, line2) => line1.playerActorNumber.CompareTo(line2.playerActorNumber));
		boardText.text = GetBeginningString();
		for (int i = 0; i < lines.Count; i++)
		{
			lines[i].gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0f, startingYValue - lineHeight * i, 0f);
			Text text = boardText;
			text.text = text.text + "\n " + NormalizeName(doIt: true, lines[i].linePlayer.NickName);
		}
	}

	public Player FindPlayerforVRRig(RigController Rig)
	{
		if (Rig != null && Rig.photonView != null && Rig.photonView.Owner != null)
		{
			return Rig.photonView.Owner;
		}
		return GetPlayerForRigControllerExternal(Rig);
	}

	void IOnEventCallback.OnEvent(EventData photonEvent)
	{
		if (photonEvent.Code == 1 || photonEvent.Code == 2 || photonEvent.Code == 3)
		{
			object[] array = (object[])photonEvent.CustomData;
			StartCoroutine(RefreshData(GetActorIDFromUserID((string)array[0]), GetActorIDFromUserID((string)array[1])));
		}
	}

	public IEnumerator RefreshData(int actorNumber1, int actorNumber2)
	{
		yield return new WaitForSeconds(1f);
		foreach (GorillaPlayerScoreboardLine line in lines)
		{
			if (line.playerActorNumber != actorNumber1)
			{
				_ = line.playerActorNumber;
				_ = actorNumber2;
			}
		}
	}

	private int GetActorIDFromUserID(string userID)
	{
		Player[] playerList = PhotonNetwork.PlayerList;
		foreach (Player player in playerList)
		{
			if (player.UserId == userID)
			{
				return player.ActorNumber;
			}
		}
		return -1;
	}

	public string NormalizeName(bool doIt, string text)
	{
		if (doIt)
		{
			text = new string(Array.FindAll(text.ToCharArray(), (char c) => char.IsLetterOrDigit(c)));
			if (text.Length > 12)
			{
				text = text[..10];
			}
			text = text.ToUpper();
		}
		return text;
	}
}
}