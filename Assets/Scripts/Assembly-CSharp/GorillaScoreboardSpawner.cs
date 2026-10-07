using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using GorillaNetworking;

public class GorillaScoreboardSpawner : MonoBehaviourPunCallbacks
{
	public GameObject ScoreboardGeo;
	public GameObject scoreboardPrefab;

	public GameObject notInRoomText;

	public GameObject controllingParentGameObject;

	public Transform ScoreAnchor;

	public GorillaScoreBoard currentScoreboard;

	public bool IsCurrentScoreboard()
	{
		return base.gameObject.activeInHierarchy;
	}

	public override void OnJoinedRoom()
	{
		if (IsCurrentScoreboard())
		{
			notInRoomText.SetActive(value: false);
			GameObject gameObject = Object.Instantiate(scoreboardPrefab, ScoreAnchor);
			currentScoreboard = gameObject.GetComponent<GorillaScoreBoard>();
			//gameObject.transform.rotation = base.transform.rotation;
		}
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		OnLeftRoom();
	}

	public void Update()
	{
		if (!(currentScoreboard != null))
		{
			return;
		}
		bool activeSelf=true;
		if(controllingParentGameObject){ activeSelf = controllingParentGameObject.activeSelf; }
		if(ScoreboardGeo){ScoreboardGeo.SetActive(activeSelf);}
		foreach (GorillaPlayerScoreboardLine line in currentScoreboard.lines)
		{
			line.HideShowLine(activeSelf);
		}
		if (currentScoreboard.boardText.enabled != activeSelf)
		{
			currentScoreboard.boardText.enabled = activeSelf;
		}
	}

	public override void OnLeftRoom()
	{
		if (currentScoreboard != null)
		{
			Object.Destroy(currentScoreboard.gameObject);
			currentScoreboard = null;
		}
		notInRoomText.SetActive(value: true);
	}
}