using System;
using UnityEngine;

namespace RemixPortals
{
    [Serializable]
    public class PortalPair
    {
        public PortalInstance portalAlpha;
        public PortalInstance portalBeta;

        public void SetupPortalPair(Material template)
        {
            RenderTextureSet rtsa = portalAlpha.pr.SetupRenderTextures();
            RenderTextureSet rtsb = portalBeta.pr.SetupRenderTextures();

            portalAlpha.pr.SetupCameras(rtsb);
            portalBeta.pr.SetupCameras(rtsa);
        }
    }

    public class PortalRendering
    {
        public RenderTexture RTL;
        public RenderTexture RTR;

        public Camera CL;
        public Camera CR;

        public RenderTextureSet SetupRenderTextures()
        {
            return new(RTL = new(1024,1024,1,MiscGrabs.grab.RendTexTemp.format),
                    RTR = new(1024,1024,1,MiscGrabs.grab.RendTexTemp.format));
        }

        public void SetupCameras(RenderTextureSet rts)
        {
            CL.targetTexture = rts.RTL;
            CR.targetTexture = rts.RTR;
        }
    }

    public class RenderTextureSet
    {
        public RenderTexture RTL;
        public RenderTexture RTR;

        public RenderTextureSet(RenderTexture tl, RenderTexture tr)
        {
            RTL = tl;
            RTR = tr;
        }
    }
}
