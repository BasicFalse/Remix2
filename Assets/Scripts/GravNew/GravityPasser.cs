using System.Collections.Generic;
using UnityEngine;

namespace GravNew
{
    public class GravityPasser : MonoBehaviour
    {
        public GravSetting gravType;
        public Vector3 V3;
        public Vector3 Rot3;
        public Transform gravity;
        public Transform rotGrav;
        public float gravStrength;
        public bool passesGravity;
        public bool isInSector;
        public NormalisedGravity controlling;

        public List<GravitySector> currentSectors = new();

        public void Update()
        {
            if (currentSectors.Count > 0)
            {
                UpdateFromSector(GetBestSector());
                UpdateRotationSector(GetRotationSector());
            }
            else
            {
                gravity = null;
                isInSector = false;
                V3 = Vector3.zero;
                Rot3 = Vector3.zero;
            }
            UpdateSectorState();
        }

        public void EnterSector(GravitySector sector)
        {
            currentSectors.Add(sector);
        }

        public void ExitSector(GravitySector sector)
        {
            currentSectors.Remove(sector);
        }

        void UpdateFromSector(GravitySector sector)
        {
            if (sector == null || !sector.active)
                return;

            gravType = sector.gravityType;
            gravity = sector.parGravity;
            gravStrength = sector.mass;
            V3 = sector.V3;
            isInSector = true;
        }

        void UpdateRotationSector(GravitySector sector)
        {
            if (sector == null || !sector.active)
                return;

            rotGrav = sector.parGravity;
            Rot3 = sector.V3;
        }

        void UpdateSectorState()
        {
            if (passesGravity && controlling != null)
            {
                controlling.gravType = gravType;
                controlling.target = gravity;
                controlling.rotTarget = rotGrav;
                controlling.gravityStrength = gravStrength;
                controlling.V3 = V3;
                controlling.Rot3 = Rot3;
                controlling.isInSector = isInSector;
            }
        }

        GravitySector GetBestSector()
        {
            GravitySector best = null;
            int bestPriority = int.MinValue;

            foreach (var sector in currentSectors)
            {
                if (sector != null && sector.active && sector.priority > bestPriority)
                {
                    best = sector;
                    bestPriority = sector.priority;
                }
            }

            return best;
        }

        GravitySector GetRotationSector()
        {
            GravitySector best = null;
            int bestPriority = int.MinValue;

            foreach (var sector in currentSectors)
            {
                if (sector != null && sector.active && sector.priority > bestPriority && sector.reorient)
                {
                    best = sector;
                    bestPriority = sector.priority;
                }
            }

            return best;
        }
    }
}
