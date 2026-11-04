using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
namespace VL.Rendering
{
    public class VLDebugDisplaySettingsData : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
    {
        private static class Strings
        {
            public static readonly DebugUI.Widget.NameAndTooltip DisableRenderPass = new DebugUI.Widget.NameAndTooltip { name = "DisableRenderPass", tooltip = "CustomRenderPassのSubPass実行を強制的にオフにします" };
            public static readonly DebugUI.Widget.NameAndTooltip DynamicResolution = new DebugUI.Widget.NameAndTooltip { name = "Dynamic Resolution", tooltip = "Dyanamic Resolutionの数値を変更" };
            public static readonly DebugUI.Widget.NameAndTooltip DisableDeferredLight = new DebugUI.Widget.NameAndTooltip { name = "Disable Deferred Light", tooltip = "ライティングオフ" };
            public static readonly DebugUI.Widget.NameAndTooltip DisableDeferredReflection = new DebugUI.Widget.NameAndTooltip { name = "Disable Deferred Reflection", tooltip = "ライティングオフ" };
            public static readonly DebugUI.Widget.NameAndTooltip DisableActor = new DebugUI.Widget.NameAndTooltip { name = "Disable Actor", tooltip = "Actorをオフ" };
            public static readonly DebugUI.Widget.NameAndTooltip DisableDeferredFog = new DebugUI.Widget.NameAndTooltip { name = "Disable Deferred Fog", tooltip = "フォグオフ" };
        }

        internal static class WidgetFactory
        {
            internal static DebugUI.Widget CreateDisableRenderPass(VLDebugDisplaySettingsData data)
            {
                return new DebugUI.BoolField
                {
                    nameAndTooltip = Strings.DisableRenderPass,
                    getter = () => data.disableRenderPass,
                    setter = value => data.disableRenderPass = value
                };
            }

            internal static DebugUI.Widget CreateDynamicResolution(VLDebugDisplaySettingsData data)
            {
                return new DebugUI.Vector2Field
                {
                    nameAndTooltip = Strings.DynamicResolution,
                    getter = () => data.dynamicResolution,
                    setter = value =>
                    {
                        data.dynamicResolution = value;
                        ScalableBufferManager.ResizeBuffers(Mathf.Clamp(value.x, 0.1f, 1f), Mathf.Clamp(value.y, 0.1f, 1f));
                    }
                };
            }

            internal static DebugUI.Widget CreateDisableDeferredLight(VLDebugDisplaySettingsData data)
            {
                return new DebugUI.BoolField
                {
                    nameAndTooltip = Strings.DisableDeferredLight,
                    getter = () => data.disableDeferredLight,
                    setter = value => data.disableDeferredLight = value
                };
            }

            internal static DebugUI.Widget CreateDisableDeferredReflection(VLDebugDisplaySettingsData data)
            {
                return new DebugUI.BoolField
                {
                    nameAndTooltip = Strings.DisableDeferredReflection,
                    getter = () => data.disableDeferredReflection,
                    setter = value => data.disableDeferredReflection = value
                };
            }

            internal static DebugUI.Widget CreateDisableActor(VLDebugDisplaySettingsData data)
            {
                return new DebugUI.BoolField
                {
                    nameAndTooltip = Strings.DisableActor,
                    getter = () => data.disableActor,
                    setter = value => data.disableActor = value
                };
            }

            internal static DebugUI.Widget CreateDisableDeferredFog(VLDebugDisplaySettingsData data)
            {
                return new DebugUI.BoolField
                {
                    nameAndTooltip = Strings.DisableDeferredFog,
                    getter = () => data.disableDeferredFog,
                    setter = value => data.disableDeferredFog = value
                };
            }

            public static DebugUI.ObjectPopupField CreateCameraSelector(VLDebugDisplaySettingsData data, Action<DebugUI.Field<UnityEngine.Object>, UnityEngine.Object> refresh)
            {
                return new DebugUI.ObjectPopupField
                {
                    displayName = "Camera",
                    getter = () => data.previewRenderPass.selectedCamera,
                    setter = value =>
                    {
                        var cameras = data.previewRenderPass.cameras.ToArray();
                        data.previewRenderPass.selectedCameraIndex = Array.IndexOf(cameras, value as Camera);
                    },
                    getObjects = () => data.previewRenderPass.cameras,
                    onValueChanged = refresh
                };
            }

            public static DebugUI.Button CreateCaptureRenderPassButton(VLDebugDisplaySettingsData data)
            {
                return new DebugUI.Button
                {
                    displayName = "Capture",
                    action = () => data.previewRenderPass.CaptureRequest()
                };
            }
        }

        [DisplayInfo(name = "VL Utility", order = 100)]
        private class VLRenderingPanel : DebugDisplaySettingsPanel<VLDebugDisplaySettingsData>
        {
            public override DebugUI.Flags Flags => DebugUI.Flags.None;
            public VLRenderingPanel(VLDebugDisplaySettingsData data) : base(data)
            {
                var rendering = new DebugUI.Foldout
                {
                    displayName = "Rendering Debug",
                    opened = true,
                    isHeader = true,
                    children =
                    {
                        WidgetFactory.CreateDisableRenderPass(data),
                        WidgetFactory.CreateDynamicResolution(data),
                        WidgetFactory.CreateDisableDeferredLight(data),
                        WidgetFactory.CreateDisableDeferredReflection(data),
                        WidgetFactory.CreateDisableActor(data),
                        WidgetFactory.CreateDisableDeferredFog(data)
                    }
                };
                AddWidget(rendering);
                var preview = new DebugUI.Foldout
                {
                    displayName = "Preview RenderPass",
                    opened = true,
                    isHeader = true,
                    children =
                    {
                        WidgetFactory.CreateCameraSelector(data, (_, __) => Refresh()),
                        WidgetFactory.CreateCaptureRenderPassButton(data)
                    }
                };
                AddWidget(preview);
            }
            private void Refresh() { }
        }

        public bool disableRenderPass { get; private set; }
        public Vector2 dynamicResolution { get; private set; } = new Vector2(1f, 1f);
        public bool disableDeferredLight { get; private set; }
        public bool disableDeferredReflection { get; private set; }
        public bool disableActor { get; private set; }
        public bool disableDeferredFog { get; private set; }
        public VLDebugDisplaySettingsPreviewRenderPass previewRenderPass { get; } = new VLDebugDisplaySettingsPreviewRenderPass();
        public bool AreAnySettingsActive => false;
        public bool IsPostProcessingAllowed => true;
        public bool IsLightingActive => true;
        public bool TryGetScreenClearColor(ref Color _) { return false; }
        public IDebugDisplaySettingsPanelDisposable CreatePanel() { return new VLRenderingPanel(this); }
    }
}
