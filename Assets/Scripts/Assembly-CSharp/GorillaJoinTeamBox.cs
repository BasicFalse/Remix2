using Photon.Pun;
using UnityEngine;

public class GorillaJoinTeamBox : GorillaTriggerBox
{
	public bool joinRedTeam;

	public override void OnBoxTriggered()
	{
		base.OnBoxTriggered();
		if (GameObject.FindGameObjectWithTag("PhotonRoomManagerMain").GetComponent<PhotonRoomManagerMain>() != null)
		{
			_ = PhotonNetwork.InRoom;
		}
	}
}
