using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialData", menuName = "GorillaLocomotion/MaterialData")]
public class MaterialDatas : ScriptableObject
{

    

    public List<MaterialData> materialData;
}

[System.Serializable]
        public struct MaterialData
        {
            public string matName;

            public bool canGrab;

            public string grabbableObject;

            public bool overrideAudio;

            public AudioClip audio;

            public bool overrideSlidePercent;

            public float slidePercent;
            public float restitution;
            public float bounce;
        }