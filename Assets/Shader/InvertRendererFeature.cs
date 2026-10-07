using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class InvertRendererFeature : ScriptableRendererFeature
{
    class InvertPass : ScriptableRenderPass
    {
        const string PassName = "InvertBehind";

        public InvertPass()
        {
            renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
            // Tell URP we need the opaque texture as input
            requiresIntermediateTexture = true;
        }

        // Render Graph path (Unity 6 / URP 17+)
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            var resourceData = frameData.Get<UniversalResourceData>();

            var inverters = Object.FindObjectsByType<InvertMarker>();
            if (inverters.Length == 0) return;

            // Sort front-to-back
            var cam = cameraData.camera;
            System.Array.Sort(inverters, (a, b) =>
            {
                float da = (cam.transform.position - a.transform.position).sqrMagnitude;
                float db = (cam.transform.position - b.transform.position).sqrMagnitude;
                return da.CompareTo(db);
            });

            foreach (var inv in inverters)
            {
                var rend = inv.GetComponent<Renderer>();
                if (rend == null || !rend.enabled) continue;

                using (var builder = renderGraph.AddUnsafePass<PassData>(PassName, out var passData))
                {
                    passData.renderer = rend;
                    passData.colorTarget = resourceData.activeColorTexture;

                    builder.UseTexture(resourceData.activeColorTexture, AccessFlags.ReadWrite);
                    builder.AllowPassCulling(false);

                    builder.SetRenderFunc((PassData data, UnsafeGraphContext ctx) =>
                    {
                        var cmd = CommandBufferHelpers.GetNativeCommandBuffer(ctx.cmd);

                        // Copy current frame into _CameraOpaqueTexture so the shader sees
                        // what's been rendered so far (including previous inverter instances)
                        cmd.SetGlobalTexture("_CameraOpaqueTexture", data.colorTarget);
                        cmd.DrawRenderer(data.renderer, data.renderer.sharedMaterial);
                    });
                }
            }
        }
    }

    class PassData
    {
        public Renderer renderer;
        public TextureHandle colorTarget;
    }

    InvertPass pass;

    public override void Create()
    {
        pass = new InvertPass();
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}