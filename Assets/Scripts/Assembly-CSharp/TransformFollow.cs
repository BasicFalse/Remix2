using GorillaLocomotion;
using UnityEngine;

public class TransformFollow : MonoBehaviour
{
	public Transform transformToFollow;
	public bool synkX;
	public bool synkY;
	public bool synkZ;
	public bool synkW;
	public Transform offsetTransform;
	public bool usePosOffset;
	public Transform transformToControl;
	public bool isControlling;
	public bool followPlayer;
	public Vector3 rotOffset;
	public Vector3 offset;
	public Transform downVector;

	private void LateUpdate()
	{
		if (isControlling)
		{
			transformToControl.transform.rotation = transformToFollow.rotation;
			transformToControl.transform.position = transformToFollow.position;
		}
		else
		{
			if (usePosOffset)
			{
				float x; float y; float z;
				x = transformToFollow.position.x + offsetTransform.localPosition.x;
				y = transformToFollow.position.y + offsetTransform.localPosition.y;
				z = transformToFollow.position.z + offsetTransform.localPosition.z;
				transform.position = new Vector3(x, y, z);
				transform.rotation = new Quaternion(transform.rotation.x, -offsetTransform.rotation.y, transform.rotation.z, transform.rotation.w);
			}
			else if (followPlayer)
			{
				transform.position = PlayerBase.@i.transform.position;
			}
			else
			{
				if(downVector){
				base.transform.rotation = SyncAxesWithDown(base.transform.rotation, transformToFollow.rotation, synkX, synkY, synkZ, -downVector.transform.up);// * new Quaternion(rotOffset.x,rotOffset.y,rotOffset.z,transformToFollow.rotation.w);
				}
				else
				{
					transform.rotation = transformToFollow.rotation;
				}
				base.transform.position = transformToFollow.position + offset;//transformToFollow.rotation * offset;
			}
		}
	}

	public static Quaternion SyncAxesWithDown(Quaternion current, Quaternion target, bool syncX, bool syncY, bool syncZ, Vector3 customDown)
	{
	    // Normalize the down vector
	    customDown.Normalize();

	    // Build a reference rotation aligned to the custom down
	    Quaternion alignToDown = Quaternion.FromToRotation(Vector3.down, customDown);

	    // Convert both into this "down-aligned" space
	    Vector3 curEuler = (Quaternion.Inverse(alignToDown) * current).eulerAngles;
	    Vector3 tgtEuler = (Quaternion.Inverse(alignToDown) * target).eulerAngles;

	    // Replace only the synced axes
	    if (syncX) curEuler.x = tgtEuler.x;
	    if (syncY) curEuler.y = tgtEuler.y;
	    if (syncZ) curEuler.z = tgtEuler.z;

	    // Transform back into world space
	    return alignToDown * Quaternion.Euler(curEuler);
	}

}
