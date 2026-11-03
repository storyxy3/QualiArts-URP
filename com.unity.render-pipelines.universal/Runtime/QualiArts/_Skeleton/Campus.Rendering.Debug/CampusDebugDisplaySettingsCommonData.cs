// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace Campus.Rendering.Debug {
using System;
using System.Runtime.CompilerServices;
using Campus.Rendering.Debug;
using UnityEngine;
using UnityEngine.Rendering;

public class CampusDebugDisplaySettingsCommonData : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
{

	[DisplayInfo(name = "Campus Rendering", order = 1)]
	private class RenderingPanel : DebugDisplaySettingsPanel
	{
		[Serializable]

		[CompilerGenerated]
		private sealed class _003C_003Ec
		{


			public static readonly _003C_003Ec _003C_003E9;


			public static Action<int> _003C_003E9__2_1;


			public _003C_003Ec()
			{
			}


			internal void _003C_002Ector_003Eb__2_1(int index)
			{
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass2_0
		{


			public CampusDebugDisplaySettingsCommonData data;


			public _003C_003Ec__DisplayClass2_0()
			{
			}


			internal int _003C_002Ector_003Eb__0()
			{
				return default(int);
			}


			internal int _003C_002Ector_003Eb__2()
			{
				return default(int);
			}


			internal void _003C_002Ector_003Eb__3(int value)
			{
			}
		}

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
