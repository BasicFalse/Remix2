using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using GorillaRigging;

public class GorillaParent : MonoBehaviour
{
	public MiscGrabs MiscGrabs;

	public GameObject vrrigParent;

	public static volatile GorillaParent instance;

	public List<RigController> vrrigs;

	public Dictionary<Player, RigController> vrrigDict = new Dictionary<Player, RigController>();

	private int i;

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void Start()
	{
		MiscGrabs = MiscGrabs.grab;
	}

	public void Update()
	{
		for (i = vrrigs.Count - 1; i > -1; i--)
		{
			if (vrrigs[i] == null)
			{
				vrrigs.RemoveAt(i);
			}
		}
		MiscGrabs.PlayerCameras.Clear();
		for (i = 0; i < vrrigs.Count; i++)
		{
			MiscGrabs.PlayerCameras.Add(vrrigs[i].HeadFollow);
		}
		for (i = 0; i < MiscGrabs.ExtraTargs.Count; i++)
		{
			MiscGrabs.PlayerCameras.Add(MiscGrabs.ExtraTargs[i]);
		}
	}

	public void CheckRigList(List<RigController> current)
	{
		List<RigController> rigCross = new(vrrigs);
		foreach(RigController rig in current)
		{
			if (rigCross.Contains(rig))
			{
				rigCross.Remove(rig);
			}
		}

		if(rigCross.Count == 0) return;

		foreach (RigController rig2 in rigCross)
		{
			if (vrrigs.Contains(rig2) && rig2.RigStatus != RigController.RigType.Offline && rig2.RigStatus != RigController.RigType.OurOnline)
			{
				vrrigs.Remove(rig2);
				rig2.InitiateLocalSelfDestructSequence();
			}
		}
	}
}
