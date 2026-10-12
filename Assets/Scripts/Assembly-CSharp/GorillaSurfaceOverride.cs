using GorillaLocomotion;
using Unity.Mathematics;
using UnityEngine;

public class GorillaSurfaceOverride : MonoBehaviour
{
	public enum HitType{Set, Crystal, Pumpkin, Soft}
	public HitType SoundSet;
	public int overrideIndex;

    void Start()
    {
		switch (SoundSet)
		{
			case HitType.Set:
				// meh
				break;
			case HitType.Crystal:
				int2 w;
				MiscGrabs.grab.GetOverrideRange(SoundSet, out w);
				overrideIndex = UnityEngine.Random.Range(w.x,w.y);
				break;
			case HitType.Pumpkin:
				overrideIndex = 95;
				break;
			case HitType.Soft:
				overrideIndex = 5;
				break;
		}
    }

    [ContextMenu("TestIt")]
	public void TestHit()
	{
		MaterialDatas MD = PlayerBase.@i.resources.materialData;
		AudioClip AC = MD.materialData[overrideIndex].audio;
		
		if (GetComponent<AudioSource>()) { AudioSource AS = GetComponent<AudioSource>(); AS.PlayOneShot(AC); }
		else { AudioSource AS = gameObject.AddComponent<AudioSource>(); AS.PlayOneShot(AC); }
		
		Debug.Log("played " + AC.name + " with name " + MD.materialData[overrideIndex].matName + " at index " + overrideIndex);
	}
}
