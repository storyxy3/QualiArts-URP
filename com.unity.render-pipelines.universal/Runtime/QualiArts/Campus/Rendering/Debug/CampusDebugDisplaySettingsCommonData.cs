using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace Campus.Rendering.Debug
{
    public class CampusDebugDisplaySettingsCommonData : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
    {
        [DisplayInfo(name = "Campus Rendering", order = 1)]
        private class RenderingPanel : DebugDisplaySettingsPanel
        {
            public override DebugUI.Flags Flags => DebugUI.Flags.FrequentlyUsed;
            public RenderingPanel(CampusDebugDisplaySettingsCommonData data)
            {
                var actor = new DebugUI.Foldout
                {
                    displayName = "Actor",
                    opened = true,
                    isHeader = true,
                    children =
                    {
                        new DebugUI.EnumField
                        {
                            displayName = "Actor Shader Debug",
                            autoEnum = typeof(ActorDebugMaterialMode),
                            getter = () => (int)data.actorDebugMaterialMode,
                            setter = index =>
                            {
                                var material = UniversalRenderPipelineDebugDisplaySettings.Instance.materialSettings;
                                if (index != 0)
                                    material.materialValidationMode = DebugMaterialValidationMode.Albedo;
                                else if (material.materialValidationMode == DebugMaterialValidationMode.Albedo)
                                    material.materialValidationMode = DebugMaterialValidationMode.None;
                            },
                            getIndex = () => (int)data.actorDebugMaterialMode,
                            setIndex = value => data.actorDebugMaterialMode = (ActorDebugMaterialMode)value
                        }
                    }
                };
                AddWidget(actor);
            }
        }
        internal ActorDebugMaterialMode actorDebugMaterialMode { get; private set; }
        public bool AreAnySettingsActive => actorDebugMaterialMode != ActorDebugMaterialMode.None;
        public bool IsPostProcessingAllowed => true;
        public bool IsLightingActive => true;
        public bool TryGetScreenClearColor(ref Color _) { return false; }
        public IDebugDisplaySettingsPanelDisposable CreatePanel() { return new RenderingPanel(this); }
    }
}
