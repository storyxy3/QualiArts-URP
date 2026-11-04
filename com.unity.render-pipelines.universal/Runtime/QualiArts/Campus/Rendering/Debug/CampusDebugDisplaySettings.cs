using UnityEngine.Rendering;
namespace Campus.Rendering.Debug
{
    public class CampusDebugDisplaySettings : DebugDisplaySettings<CampusDebugDisplaySettings>
    {
        public CampusDebugDisplaySettingsCommonData commonData { get; set; }
        public CampusDebugDisplaySettings() { Reset(); }
        public override void Reset()
        {
            base.Reset();
            commonData = Add(new CampusDebugDisplaySettingsCommonData());
        }
    }
}
