using System;
using UnityEngine;

namespace GorillaRigging{
    [Serializable]
    public class RigLink
    {
        [SerializeField]
        public Transform Controlling;

        public Vector3 Position;
        public Quaternion Rotation;

        public Vector3 smoothPos;
        public Quaternion smoothRot;

        private Vector3 lastTargetPos;
        private Quaternion lastTargetRot;
        private Vector3 targetPos;
        private Quaternion targetRot;
        private float lastCorrectionDistance;
        private float interpTimer;
        private float interpDuration = 0.1f;
        private bool hasNetworkTarget = false;
        private bool initialized = false;

        public RigLink(RigLink controlling)
        {
            Controlling = controlling.Controlling;
            Position = controlling.Controlling.position;
            Rotation = controlling.Controlling.rotation;
            smoothPos = Position;
            smoothRot = Rotation;
            lastTargetPos = Position;
            lastTargetRot = Rotation;
            targetPos = Position;
            targetRot = Rotation;
            initialized = true;
        }

        public RigLink(Transform seed)
        {
            Controlling = seed;
            Position = seed.position;
            Rotation = seed.rotation;
            smoothPos = Position;
            smoothRot = Rotation;
            lastTargetPos = Position;
            lastTargetRot = Rotation;
            targetPos = Position;
            targetRot = Rotation;
            initialized = true;
        }

        public RigLink()
        {
            initialized = false;
        }

        public void SetPosAndRot(Vector3 pos, Quaternion rot)
        {
            Position = pos;
            Rotation = rot;
        }

        public void InitializeFromCurrent()
        {
            if (Controlling != null)
            {
                smoothPos = Controlling.position;
                smoothRot = Controlling.rotation;
            }
            else
            {
                smoothPos = Vector3.zero;
                smoothRot = Quaternion.identity;
            }

            lastTargetPos = smoothPos;
            lastTargetRot = smoothRot;
            targetPos = smoothPos;
            targetRot = smoothRot;
            interpTimer = 0f;
            hasNetworkTarget = false;
            initialized = true;
        }

        /// <summary>
        /// Legacy smoothing: immediately lerps the internal smooth value toward Position/Rotation using a per-frame lerp factor.
        /// Kept for Offline and OurOnline logic.
        /// </summary>
        public void ApplySmooth(float lerp)
        {
            Vector3 before = smoothPos;

            smoothPos = Vector3.Lerp(smoothPos, Position, lerp);
            smoothRot = Quaternion.Slerp(smoothRot, Rotation, lerp);

            lastCorrectionDistance = Vector3.Distance(before, smoothPos);

            if (Controlling != null)
                Controlling.SetPositionAndRotation(smoothPos, smoothRot);
        }

        /// <summary>
        /// Call this when a network update arrives. It records the current smooth pose as the 'last' and the received as 'target'.
        /// </summary>
        public void SetNetworkTarget(Vector3 pos, Quaternion rot, float duration)
        {
            lastCorrectionDistance = Vector3.Distance(smoothPos, pos);

            lastTargetPos = smoothPos;
            lastTargetRot = smoothRot;

            targetPos = pos;
            targetRot = rot;

            interpDuration = Mathf.Max(0.0001f, duration);
            interpTimer = 0f;
            hasNetworkTarget = true;

            Position = pos;
            Rotation = rot;
        }

        /// <summary>
        /// Interpolate from the previous network target to the new network target over interpDuration.
        /// Should be called every frame for remote (Online) rigs using Time.deltaTime.
        /// </summary>
        public void ApplyNetworkSmooth(float deltaTime)
        {
            if (Controlling == null)
                return;

            if (!hasNetworkTarget)
            {
                Controlling.SetPositionAndRotation(smoothPos, smoothRot);
                return;
            }

            interpTimer += deltaTime;
            float t = interpDuration <= 0f ? 1f : Mathf.Clamp01(interpTimer / interpDuration);

            smoothPos = Vector3.Lerp(lastTargetPos, targetPos, t);
            smoothRot = Quaternion.Slerp(lastTargetRot, targetRot, t);

            //lastCorrectionDistance = Vector3.Distance(targetPos, Controlling.position);

            Controlling.SetPositionAndRotation(smoothPos, smoothRot);

            if (t >= 1f)
            {
                hasNetworkTarget = false;
                smoothPos = targetPos;
                smoothRot = targetRot;
            }
        }

        /// <summary>
        /// Gets how far the last positional correction was.
        /// </summary>
        /// <returns>lastCorrectionDistance</returns>
        public float GetCorrectionDistance()
        {
            return lastCorrectionDistance;
        }
    }
}