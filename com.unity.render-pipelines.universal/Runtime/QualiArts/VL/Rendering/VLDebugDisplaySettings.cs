using UnityEngine.Rendering;
namespace VL.Rendering
{
    public class VLDebugDisplaySettings : DebugDisplaySettings<VLDebugDisplaySettings>
    {
        public VLDebugDisplaySettingsData data { get; set; }
        public VLDebugDisplaySettings() { Reset(); }
        public override void Reset()
        {
            base.Reset();
            data = Add(new VLDebugDisplaySettingsData());
        }
    }
}
