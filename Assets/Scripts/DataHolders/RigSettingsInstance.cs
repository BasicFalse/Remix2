using UnityEngine;

namespace GorillaRigging{
    public class RigSettingsInstance : MonoBehaviour
    {
        public RigSetup.Gorilla.BoneContainer boneContainer;

        public RigController SetupRig;

        void Awake()
        {
            VRMapThumb leftThumb   = RigSettings.GrabThumbSettings(true, 0, boneContainer.leftThumb);
            VRMapIndex leftIndex   = RigSettings.GrabIndexSettings(true, 0, boneContainer.leftIndex);
            VRMapMiddle leftMiddle  = RigSettings.GrabMiddleSettings(true, 0, boneContainer.leftMiddle);
            VRMapThumb rightThumb  = RigSettings.GrabThumbSettings(false, 0, boneContainer.rightThumb);
            VRMapIndex rightIndex  = RigSettings.GrabIndexSettings(false, 0, boneContainer.rightIndex);
            VRMapMiddle rightMiddle = RigSettings.GrabMiddleSettings(false, 0, boneContainer.rightMiddle);

            SetupRigController(leftThumb, leftIndex, leftMiddle, rightThumb, rightIndex, rightMiddle);
        }

        void SetupRigController(VRMapThumb leftThumb, VRMapIndex leftIndex, VRMapMiddle leftMiddle, VRMapThumb rightThumb, VRMapIndex rightIndex, VRMapMiddle rightMiddle)
        {
            SetupRig.ApplyVRMapWeights(leftThumb, leftIndex, leftMiddle, rightThumb, rightIndex, rightMiddle);
            SetupRig.ApplyLinkWeights(boneContainer.headLink, boneContainer.bodyLink, boneContainer.handRight, boneContainer.handLeft);
        }
    }
}