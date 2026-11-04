using UnityEngine.Rendering.Universal;
namespace Campus.Rendering.Debug
{
    public class CampusRenderingDebug : ScriptableRendererFeature
    {
        private CampusRenderingDebugPass _pass;
        private CampusRenderingOutlineDebugPass _outlinePass;
        public override void Create()
        {
            _pass = new CampusRenderingDebugPass();
            _outlinePass = new CampusRenderingOutlineDebugPass();
        }
        protected override void Dispose(bool disposing) { }
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var settings = CampusDebugDisplaySettings.Instance;
            if (settings.AreAnySettingsActive)
                renderer.EnqueuePass(_pass);
            if (settings.commonData.actorDebugMaterialMode == ActorDebugMaterialMode.Outline)
                renderer.EnqueuePass(_outlinePass);
        }
    }
}
