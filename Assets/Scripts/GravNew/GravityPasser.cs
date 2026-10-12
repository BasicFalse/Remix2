using System.Collections.Generic;
using UnityEngine;

namespace GravNew
{
    public class GravityPasser : MonoBehaviour
    {
        public bool passesGravity = true;
        public bool isInSector => currentSectors.Count > 0;
        public NormalisedGravity controlling;
        public List<GravitySector> currentSectors = new();

        public void Update()
        {
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

        void UpdateSectorState()
        {
            if (passesGravity && controlling != null)
            {
                controlling.BestGravity = GetBestSector();
                controlling.BestRotation = GetRotationSector();
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
