// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace Campus.Rendering.Debug {
using System.Runtime.CompilerServices;
using Campus.Rendering.Debug;
using UnityEngine;
using UnityEngine.Rendering;

public class CampusDebugDisplaySettingsCommonData : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
{

	[DisplayInfo(name = "Campus Rendering", order = 1)]
	private class RenderingPanel : DebugDisplaySettingsPanel
	{

		public override DebugUI.Flags Flags
		{


			get
			{
				return default(DebugUI.Flags);
			}
		}


		public RenderingPanel(CampusDebugDisplaySettingsCommonData data)
		{
		}
	}


	[CompilerGenerated]
	private ActorDebugMaterialMode _003CactorDebugMaterialMode_003Ek__BackingField;

	internal ActorDebugMaterialMode actorDebugMaterialMode
	{


		[CompilerGenerated]
		get
		{
			return default(ActorDebugMaterialMode);
		}


		[CompilerGenerated]
		private set
		{
		}
	}

	public bool AreAnySettingsActive
	{


		get
		{
			return default(bool);
		}
	}

	public bool IsPostProcessingAllowed
	{


		get
		{
			return default(bool);
		}
	}

	public bool IsLightingActive
	{


		get
		{
			return default(bool);
		}
	}


	public bool TryGetScreenClearColor(ref Color _)
	{
		return default(bool);
	}


	public IDebugDisplaySettingsPanelDisposable CreatePanel()
	{
		return null;
	}


	public CampusDebugDisplaySettingsCommonData()
	{
	}
}

}
