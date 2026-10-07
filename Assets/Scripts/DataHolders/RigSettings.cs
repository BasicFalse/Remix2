using System;
using UnityEngine;

namespace GorillaRigging{
    
    public static class RigSettings
    {
        public static VRMapThumb GrabThumbSettings(bool left, int bodyType, RigSetup.Gorilla.FingerSetup thumb)
        {
            VRMapThumb returnThumb = new();

            switch (bodyType)
            {
                case 0:
                    returnThumb = RigDefaults.Gorilla.GetDefaultsGorillaThumb(left);
                    returnThumb.fingerBone1 = thumb.fingerBone1;
                    returnThumb.fingerBone2 = thumb.fingerBone2;
                    return returnThumb;

                case 1:
                    Debug.Log("Hakr hakr go away");
                    return null;

                default: Debug.LogError("bodyType index is invalid."); return returnThumb;
            }
        }

        public static VRMapIndex GrabIndexSettings(bool left, int bodyType, RigSetup.Gorilla.FingerSetup index)
        {
            VRMapIndex returnIndex = new();

            switch (bodyType)
            {
                case 0:
                returnIndex = RigDefaults.Gorilla.GetDefaultsGorillaIndex(left);
                returnIndex.fingerBone1 = index.fingerBone1;
                returnIndex.fingerBone2 = index.fingerBone2;
                returnIndex.fingerBone3 = index.fingerBone3;
                return returnIndex;

                case 1:
                    Debug.Log("Hakr hakr go away");
                    return null;

                default: Debug.LogError("bodyType index is invalid."); return returnIndex;
            }
        }

        public static VRMapMiddle GrabMiddleSettings(bool left, int bodyType, RigSetup.Gorilla.FingerSetup middle)
        {
            VRMapMiddle returnMiddle = new();

            switch (bodyType)
            {
                case 0:
                returnMiddle = RigDefaults.Gorilla.GetDefaultsGorillaMiddle(left);
                returnMiddle.fingerBone1 = middle.fingerBone1;
                returnMiddle.fingerBone2 = middle.fingerBone2;
                returnMiddle.fingerBone3 = middle.fingerBone3;
                return returnMiddle;

                case 1:
                    Debug.Log("Hakr hakr go away");
                    return null;

                default: Debug.LogError("bodyType index is invalid."); return returnMiddle;
            }
        }
    }

    public static class RigDefaults
    {
        public static class Gorilla
        {
            public static VRMapThumb GetDefaultsGorillaThumb(bool left)
            {
                VRMapThumb defaults = new();
                if (left)
                {
                    defaults.closedAngle1 = new Vector3(-68.8700027f,11.8579998f,-189.617004f);
                    defaults.closedAngle2 = new Vector3(1.84899998f,25.9309998f,-66.2250137f);
                    defaults.startingAngle1 = new Vector3(-53.0320015f,-58.0940018f,-21.0550003f);
                    defaults.startingAngle2 = new Vector3(1.84899998f,25.9309998f,-5.95699978f);
                }
                else
                {
                    defaults.closedAngle1 = new Vector3(-85.9770126f,10.7690001f,82.8690033f);
                    defaults.closedAngle2 = new Vector3(-1.03699994f,0.786000013f,96.1500015f);
                    defaults.startingAngle1 = new Vector3(-53.0320015f,-29.5540009f,21.0550003f);
                    defaults.startingAngle2 = new Vector3(1.84899998f,-25.9319992f,5.95699978f);
                }
                return defaults;
            }

            public static VRMapIndex GetDefaultsGorillaIndex(bool left)
            {
                VRMapIndex defaults = new();
                if (left)
                {
                    defaults.closedAngles = 90;
                    defaults.closedAngle1 = new Vector3(6.77400017f,-80.4649963f,-97.4800034f);
                    defaults.closedAngle2 = new Vector3(0.656000018f,2.54999995f,-92.5650024f);
                    defaults.closedAngle3 = new Vector3(10.0559998f,11.9610004f,-122.278f);
                    defaults.startingAngle1 = new Vector3(6.77400017f,-80.4649963f,-7.48000002f);
                    defaults.startingAngle2 = new Vector3(0.656000018f,2.54999995f,-2.56500006f);
                    defaults.startingAngle3 = new Vector3(0.653999984f,13.8559999f,-13.9029999f);
                }
                else
                {
                    defaults.closedAngles = 90;
                    defaults.closedAngle1 = new Vector3(6.66400099f,-7.19600105f,97.4729996f);
                    defaults.closedAngle2 = new Vector3(0,0,92.5670013f);
                    defaults.closedAngle3 = new Vector3(103.248001f,-83.2730026f,4.40700006f);
                    defaults.startingAngle1 = new Vector3(6.66400099f,-7.19600105f,7.473001f);
                    defaults.startingAngle2 = new Vector3(0.930000126f,-2.54299998f,2.56699991f);
                    defaults.startingAngle3 = new Vector3(13.2480001f,-83.2730026f,4.40700006f);
                }
                return defaults;
            }

            public static VRMapMiddle GetDefaultsGorillaMiddle(bool left)
            {
                VRMapMiddle defaults = new();
                if (left)
                {
                    defaults.closedAngles = 90;
                    defaults.closedAngle1 = new Vector3(-15.6029997f,-96.9120026f,-91.1520004f);
                    defaults.closedAngle2 = new Vector3(-0.718999982f,4.55900002f,-94.5559998f);
                    defaults.closedAngle3 = new Vector3(-4.0170002f,5.96199989f,-102.269997f);
                    defaults.startingAngle1 = new Vector3(-15.6029997f,-96.9120026f,-1.15199995f);
                    defaults.startingAngle2 = new Vector3(-0.718999982f,4.55900002f,-4.55600023f);
                    defaults.startingAngle3 = new Vector3(-2.54699993f,12.6190004f,-12.6520004f);
                }
                else
                {
                    defaults.closedAngles = 90;
                    defaults.closedAngle1 = new Vector3(-15.7299995f,96.9100037f,91.1460037f);
                    defaults.closedAngle2 = new Vector3(-0.439999998f,-4.54699993f,94.5479965f);
                    defaults.closedAngle3 = new Vector3(-2.69499993f,-12.6199999f,102.689003f);
                    defaults.startingAngle1 = new Vector3(-15.7299995f,96.9100037f,1.14600003f);
                    defaults.startingAngle2 = new Vector3(-0.439999998f,-4.54699993f,4.54799986f);
                    defaults.startingAngle3 = new Vector3(-2.69499993f,-12.6199999f,12.6890001f);
                }
                return defaults;
            }
        }
    }


    public class RigSetup
    {
        [Serializable]
        public class Gorilla
        {
            [Serializable]
            public class BoneContainer
            {
                public Transform headLink;
                public Transform bodyLink;
                public Transform handLeft;
                public Transform handRight;
                public FingerSetup leftThumb;
                public FingerSetup leftIndex;
                public FingerSetup leftMiddle;
                public FingerSetup rightThumb;
                public FingerSetup rightIndex;
                public FingerSetup rightMiddle;
            }
            [Serializable]
            public class FingerSetup
            {
                public Transform fingerBone1;
                public Transform fingerBone2;
                public Transform fingerBone3;
            }
        }
        
    }
}