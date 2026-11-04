using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace UnityEngine.Rendering
{
    /// <summary>
    /// Debug Dispaly Settings Volume
    /// </summary>
    public class DebugDisplaySettingsVolume : IDebugDisplaySettingsData
    {
        /// <summary>Current volume debug settings.</summary>
        public IVolumeDebugSettings2 volumeDebugSettings { get; }

        /// <summary>
        /// Constructor with the settings
        /// </summary>
        /// <param name="volumeDebugSettings"></param>
        public DebugDisplaySettingsVolume(IVolumeDebugSettings2 volumeDebugSettings)
        {
            this.volumeDebugSettings = volumeDebugSettings;
        }

        internal int volumeComponentEnumIndex;

        static class Styles
        {
            public static readonly GUIContent none = new GUIContent("None");
            public static readonly GUIContent editorCamera = new GUIContent("Editor Camera");
        }

        static class Strings
        {
            public static readonly string none = "None";
            public static readonly string camera = "Camera";
            public static readonly string parameter = "Parameter";
            public static readonly string component = "Component";
            public static readonly string debugViewNotSupported = "Debug view not supported";
            public static readonly string volumeInfo = "Volume Info";
            public static readonly string interpolatedValue = "Interpolated Value";
            public static readonly string defaultValue = "Default Value";
            public static readonly string global = "Global";
            public static readonly string local = "Local";
        }

        internal static class WidgetFactory
        {
            public static DebugUI.EnumField CreateComponentSelector(SettingsPanel panel, Action<DebugUI.Field<int>, int> refresh)
            {
                int componentIndex = 0;
                var componentNames = new List<GUIContent>() { Styles.none };
                var componentValues = new List<int>() { componentIndex++ };

                foreach (var type in panel.data.volumeDebugSettings.volumeComponentsPathAndType)
                {
                    componentNames.Add(new GUIContent() { text = type.Item1 });
                    componentValues.Add(componentIndex++);
                }

                return new DebugUI.EnumField
                {
                    displayName = Strings.component,
                    getter = () => panel.data.volumeDebugSettings.selectedComponent,
                    setter = value => panel.data.volumeDebugSettings.selectedComponent = value,
                    enumNames = componentNames.ToArray(),
                    enumValues = componentValues.ToArray(),
                    getIndex = () => panel.data.volumeComponentEnumIndex,
                    setIndex = value => { panel.data.volumeComponentEnumIndex = value; },
                    onValueChanged = refresh
                };
            }

            public static DebugUI.ObjectPopupField CreateCameraSelector(SettingsPanel panel, Action<DebugUI.Field<Object>, Object> refresh)
            {
                return new DebugUI.ObjectPopupField
                {
                    displayName = Strings.camera,
                    getter = () => panel.data.volumeDebugSettings.selectedCamera,
                    setter = value =>
                    {
                        var c = panel.data.volumeDebugSettings.cameras.ToArray();
                        panel.data.volumeDebugSettings.selectedCameraIndex = Array.IndexOf(c, value as Camera);
                    },
                    getObjects = () => panel.data.volumeDebugSettings.cameras,
                    onValueChanged = refresh
                };
            }

            static DebugUI.Widget CreateVolumeParameterWidget(string name, VolumeParameter param, Func<bool> isHiddenCallback = null)
            {
                if (param == null)
                    return new DebugUI.Value { displayName = name, getter = () => "-" };
                var parameterType = param.GetType();
                if (parameterType == typeof(ColorParameter))
                {
                    var color = (ColorParameter)param;
                    return new DebugUI.ColorField
                    {
                        displayName = name,
                        hdr = color.hdr,
                        showAlpha = color.showAlpha,
                        getter = () => color.value,
                        setter = value => color.value = value,
                        isHiddenCallback = isHiddenCallback
                    };
                }
                if (parameterType == typeof(BoolParameter))
                {
                    var boolean = (BoolParameter)param;
                    return new DebugUI.BoolField
                    {
                        displayName = name,
                        getter = () => boolean.value,
                        setter = value => boolean.value = value,
                        isHiddenCallback = isHiddenCallback
                    };
                }
                var genericArguments = parameterType.GetTypeInfo().BaseType.GenericTypeArguments;
                if (genericArguments.Length > 0 && genericArguments[0].IsArray)
                {
                    return new DebugUI.ObjectListField
                    {
                        displayName = name,
                        getter = () => (Object[])parameterType.GetProperty("value").GetValue(param, null),
                        type = parameterType
                    };
                }
                var property = param.GetType().GetProperty("value");
                var toString = property.PropertyType.GetMethod("ToString", Type.EmptyTypes);
                if (toString == null || toString.DeclaringType == typeof(object) || toString.DeclaringType == typeof(Object))
                {
                    var nameProperty = property.PropertyType.GetProperty("name");
                    if (nameProperty == null)
                        return new DebugUI.Value { displayName = name, getter = () => Strings.debugViewNotSupported };
                    return new DebugUI.Value
                    {
                        displayName = name,
                        getter = () =>
                        {
                            var value = property.GetValue(param);
                            if (value == null || value.Equals(null))
                                return Strings.none;
                            return nameProperty.GetValue(value) ?? Strings.none;
                        },
                        isHiddenCallback = isHiddenCallback
                    };
                }
                return new DebugUI.Value
                {
                    displayName = name,
                    getter = () => property.GetValue(param)?.ToString() ?? Strings.none,
                    isHiddenCallback = isHiddenCallback
                };
            }

            public static DebugUI.Table CreateVolumeTable(DebugDisplaySettingsVolume data)
            {
                var table = new DebugUI.Table { displayName = Strings.parameter, isReadOnly = true };
                Type selectedType = data.volumeDebugSettings.selectedComponentType;
                if (selectedType == null)
                    return table;
                VolumeComponent stackComponent = (data.volumeDebugSettings.selectedCameraVolumeStack ?? VolumeManager.instance.stack).GetComponent(selectedType);
                if (stackComponent == null)
                    return table;
                Volume[] volumes = data.volumeDebugSettings.GetVolumes();
                var inst = (VolumeComponent)ScriptableObject.CreateInstance(selectedType);
                var row = new DebugUI.Table.Row
                {
                    displayName = Strings.volumeInfo,
                    opened = true,
                    children = { new DebugUI.Value { displayName = Strings.interpolatedValue, getter = () => string.Empty } }
                };
                var gameObjectRow = new DebugUI.Table.Row
                {
                    displayName = "GameObject",
                    children = { new DebugUI.Value { getter = () => string.Empty } }
                };
                foreach (var volume in volumes)
                {
                    var profile = volume.HasInstantiatedProfile() ? volume.profile : volume.sharedProfile;
                    row.children.Add(new DebugUI.Value
                    {
                        displayName = profile.name,
                        getter = () => (volume.isGlobal ? Strings.global : Strings.local) + " (" + (data.volumeDebugSettings.GetVolumeWeight(volume) * 100f).ToString() + "%)"
                    });
                    gameObjectRow.children.Add(new DebugUI.ObjectField { displayName = profile.name, getter = () => volume.gameObject });
                }
                row.children.Add(new DebugUI.Value { displayName = Strings.defaultValue, getter = () => string.Empty });
                table.children.Add(row);
                gameObjectRow.children.Add(new DebugUI.Value { getter = () => string.Empty });
                table.children.Add(gameObjectRow);
                var rows = new List<DebugUI.Table.Row>();
                AddParameterRows(selectedType, null);
                foreach (var parameterRow in rows.OrderBy(r => r.displayName))
                    table.children.Add(parameterRow);
                data.volumeDebugSettings.RefreshVolumes(volumes);
                for (int i = 0; i < volumes.Length; i++)
                    table.SetColumnVisibility(i + 1, data.volumeDebugSettings.VolumeHasInfluence(volumes[i]));
                float timer = 0f;
                float refreshRate = 0.2f;
                table.isHiddenCallback = () =>
                {
                    timer += Time.deltaTime;
                    if (timer < refreshRate)
                        return false;
                    if (data.volumeDebugSettings.selectedCamera != null)
                    {
                        var newVolumes = data.volumeDebugSettings.GetVolumes();
                        if (!data.volumeDebugSettings.RefreshVolumes(newVolumes))
                        {
                            for (int i = 0; i < newVolumes.Length; i++)
                                table.SetColumnVisibility(i + 1, data.volumeDebugSettings.VolumeHasInfluence(newVolumes[i]));
                        }
                        if (!volumes.SequenceEqual(newVolumes))
                        {
                            volumes = newVolumes;
                            DebugManager.instance.ReDrawOnScreenDebug();
                        }
                    }
                    timer = 0f;
                    return false;
                };
                return table;

                int AddParameterRows(Type type, string baseName, int skip = 0)
                {
                    foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).OrderBy(f => f.MetadataToken))
                    {
                        if (field.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length != 0)
                        {
                            skip++;
                            continue;
                        }
                        Type fieldType = field.FieldType;
                        if (fieldType.IsSubclassOf(typeof(VolumeParameter)))
                            AddRow(field, baseName ?? string.Empty, skip);
                        else if (!fieldType.IsArray && fieldType.IsClass)
                            skip += AddParameterRows(fieldType, baseName ?? field.Name + " ", skip);
                    }
                    return skip;
                }

                void AddRow(FieldInfo field, string prefix, int skip)
                {
                    string displayName = prefix + field.Name;
                    var displayInfo = (DisplayInfoAttribute[])field.GetCustomAttributes(typeof(DisplayInfoAttribute), true);
                    if (displayInfo.Length != 0)
                        displayName = prefix + displayInfo[0].name;
                    int currentParam = rows.Count + skip;
                    row = new DebugUI.Table.Row
                    {
                        displayName = displayName,
                        children = { CreateVolumeParameterWidget(Strings.interpolatedValue, stackComponent.parameterList[currentParam]) }
                    };
                    foreach (var volume in volumes)
                    {
                        var profile = volume.HasInstantiatedProfile() ? volume.profile : volume.sharedProfile;
                        VolumeParameter parameter = profile.TryGet(selectedType, out VolumeComponent component)
                            ? component.parameterList[currentParam] : null;
                        row.children.Add(CreateVolumeParameterWidget(volume.name + " (" + profile.name + ")", parameter,
                            () => !component.parameterList[currentParam].overrideState));
                    }
                    row.children.Add(CreateVolumeParameterWidget(Strings.defaultValue, inst.parameterList[currentParam]));
                    rows.Add(row);
                }
            }
        }

        [DisplayInfo(name = "Volume", order = int.MaxValue)]
        internal class SettingsPanel : DebugDisplaySettingsPanel<DebugDisplaySettingsVolume>
        {
            public SettingsPanel(DebugDisplaySettingsVolume data)
                : base(data)
            {
                AddWidget(WidgetFactory.CreateComponentSelector(this, (_, __) => Refresh()));
                AddWidget(WidgetFactory.CreateCameraSelector(this, (_, __) => Refresh()));
            }

            DebugUI.Table m_VolumeTable = null;
            void Refresh()
            {
                var panel = DebugManager.instance.GetPanel(PanelName);
                if (panel == null)
                    return;

                bool needsRefresh = false;
                if (m_VolumeTable != null)
                {
                    needsRefresh = true;
                    panel.children.Remove(m_VolumeTable);
                }

                if (m_Data.volumeDebugSettings.selectedComponent > 0 && m_Data.volumeDebugSettings.selectedCamera != null)
                {
                    needsRefresh = true;
                    m_VolumeTable = WidgetFactory.CreateVolumeTable(m_Data);
                    AddWidget(m_VolumeTable);
                    panel.children.Add(m_VolumeTable);
                }

                if (needsRefresh)
                    DebugManager.instance.ReDrawOnScreenDebug();
            }
        }

        #region IDebugDisplaySettingsData
        /// <summary>
        /// Checks whether ANY of the debug settings are currently active.
        /// </summary>
        public bool AreAnySettingsActive => false; // Volume Debug Panel doesn't need to modify the renderer data, therefore this property returns false
        /// <summary>
        /// Checks whether the current state of these settings allows post-processing.
        /// </summary>
        public bool IsPostProcessingAllowed => true;
        /// <summary>
        /// Checks whether lighting is active for these settings.
        /// </summary>
        public bool IsLightingActive => true;

        /// <summary>
        /// Attempts to get the color used to clear the screen for this debug setting.
        /// </summary>
        /// <param name="color">A reference to the screen clear color to use.</param>
        /// <returns>"true" if we updated the color, "false" if we didn't change anything.</returns>
        public bool TryGetScreenClearColor(ref Color color)
        {
            return false;
        }

        /// <summary>
        /// Creates the panel
        /// </summary>
        /// <returns>The panel</returns>
        public IDebugDisplaySettingsPanelDisposable CreatePanel()
        {
            return new SettingsPanel(this);
        }

        #endregion
    }
}
