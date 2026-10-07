using UnityEngine;
using Photon.Pun;

public class GroupJoinButton : GorillaPressableButton
{
	public string gameMode;

	public GorillaFriendCollider friendCollider;

	public bool inPrivate;

	public override void ButtonActivation()
	{
		base.ButtonActivation();
		if (inPrivate)
		{
			//GorillaComputer.instance.OnGroupJoinButtonPress(gameMode, friendCollider);
			Debug.Log("This don't work");
		}
	}

	public void Update()
	{
		inPrivate = PhotonNetwork.InRoom && !PhotonNetwork.CurrentRoom.IsVisible;
		if (!inPrivate)
		{
			isOn = true;
		}
	}
}
