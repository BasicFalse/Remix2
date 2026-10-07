using UnityEngine;

namespace GorillaNetworking
{
	public class GorillaNetworkJoinTrigger : GorillaTriggerBox
	{
		public string mapToJoin;

		public override void OnBoxTriggered()
		{
			base.OnBoxTriggered();
			PhotonNetworkController.instance.AttemptToJoinPublicRoom(this);
		}
	}
}
