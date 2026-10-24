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
			currentScoreboard.Awake();
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
		for(int i = 0; i < GorillaParent.instance.vrrigs.Count; i++)
		{
			bool accountedFor = false;
			for(int j = 0; j < currentScoreboard.lines.Count; j++)
			{
				if(currentScoreboard.lines[j].linePlayer != null)
					if(PhotonRoomManagerMain.instance.FindVRRigControllerForPlayer(currentScoreboard.lines[j].linePlayer) == GorillaParent.instance.vrrigs[i])
						accountedFor = true;
			}
			if (!accountedFor)
			{
				currentScoreboard.CreateNewPlayerLine(GorillaParent.instance.vrrigs[i]);
			}
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