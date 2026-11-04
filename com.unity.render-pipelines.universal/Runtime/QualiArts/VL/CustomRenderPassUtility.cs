using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace VL
{
    public static class CustomRenderPassUtility
    {
        public static void Execute(ScriptableRenderContext context, ref RenderingData renderingData, CustomRenderPassData renderPass, CustomSubPassData[] subPasses)
        {
            if (renderPass == null || renderPass.attachmentDescriptors == null || subPasses == null || renderPass.attachmentDescriptors.Length == 0 || subPasses.Length == 0)
            {
                Debug.LogError("不正な入力です");
                return;
            }
            int width = renderPass.width;
            int height = renderPass.height;
            if (width == 0) width = renderingData.cameraData.cameraTargetDescriptor.width;
            if (height == 0) height = renderingData.cameraData.cameraTargetDescriptor.height;
            var cmd = renderingData.commandBuffer;
            CoreUtils.SetKeyword(cmd, "_RENDER_PASS_ENABLED", true);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            var attachments = new NativeArray<AttachmentDescriptor>(renderPass.attachmentDescriptors, Allocator.Temp);
            using (context.BeginScopedRenderPass(width, height, renderPass.samples, attachments, renderPass.depthAttachmentIndex))
            {
                foreach (var subPass in subPasses)
                {
                    using (var colors = new NativeArray<int>(subPass.colors, Allocator.Temp))
                    {
                        if (subPass.inputs != null)
                        {
                            using (var inputs = new NativeArray<int>(subPass.inputs, Allocator.Temp))
                                context.BeginSubPass(colors, inputs, subPass.isDepthReadOnly, subPass.isStencilReadOnly);
                        }
                        else context.BeginSubPass(colors, subPass.isDepthReadOnly, subPass.isStencilReadOnly);
                        subPass.renderAction?.Invoke(context, ref renderingData);
                        context.ExecuteCommandBuffer(cmd);
                        cmd.Clear();
                        context.EndSubPass();
                    }
                }
            }
            CoreUtils.SetKeyword(cmd, "_RENDER_PASS_ENABLED", false);
            attachments.Dispose();
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }
    }
}
