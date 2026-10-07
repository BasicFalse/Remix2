using UnityEngine;

namespace GorillaLocomotion{
    public class ClimbableData : MonoBehaviour
    {
        /// <summary>
        ///  0 = Static Climbable  | Stable  \
        ///  1 = Dynamic Climbable | Unstable \
        ///  2 = Spline Climbable  | Stable  
        /// </summary>
        public int ClimbableType = 1;

        public bool doletgosound;
        public bool dograbsound;
        public bool doholdsound;
        public AudioClip grabsound;
        public AudioClip letgosound;
        public AudioClip holdsound;

        public Zipline ZLSc;

        void Start()
        {
            if (ZLSc){if(ZLSc.ZipClip){
            holdsound = ZLSc.ZipClip;}}
        }
    }
}