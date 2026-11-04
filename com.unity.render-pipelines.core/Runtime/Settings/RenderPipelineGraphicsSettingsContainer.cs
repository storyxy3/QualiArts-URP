using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering
{
    [Serializable]
    public class RenderPipelineGraphicsSettingsContainer : ISerializationCallbackReceiver
    {
        [SerializeField]
        private RenderPipelineGraphicsSettingsCollection m_SettingsList = new RenderPipelineGraphicsSettingsCollection();

        public List<IRenderPipelineGraphicsSettings> settingsList => m_SettingsList.settingsList;
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { }
    }
}
