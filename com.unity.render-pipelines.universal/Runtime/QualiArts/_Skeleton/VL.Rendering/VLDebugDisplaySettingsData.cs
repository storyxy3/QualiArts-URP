// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using VL.Rendering;

public class VLDebugDisplaySettingsData : IDebugDisplaySettingsData, IDebugDisplaySettingsQuery
{

	private static class Strings
	{


		public static readonly DebugUI.Widget.NameAndTooltip DisableRenderPass;


		public static readonly DebugUI.Widget.NameAndTooltip DynamicResolution;


		public static readonly DebugUI.Widget.NameAndTooltip DisableDeferredLight;


		public static readonly DebugUI.Widget.NameAndTooltip DisableDeferredReflection;


		public static readonly DebugUI.Widget.NameAndTooltip DisableActor;


		public static readonly DebugUI.Widget.NameAndTooltip DisableDeferredFog;
	}

	internal static class WidgetFactory
	{


		internal static DebugUI.Widget CreateDisableRenderPass(VLDebugDisplaySettingsData data)
		{
			return null;
		}


		internal static DebugUI.Widget CreateDynamicResolution(VLDebugDisplaySettingsData data)
		{
			return null;
		}


		internal static DebugUI.Widget CreateDisableDeferredLight(VLDebugDisplaySettingsData data)
		{
			return null;
		}


		internal static DebugUI.Widget CreateDisableDeferredReflection(VLDebugDisplaySettingsData data)
		{
			return null;
		}


		internal static DebugUI.Widget CreateDisableActor(VLDebugDisplaySettingsData data)
		{
			return null;
		}


		internal static DebugUI.Widget CreateDisableDeferredFog(VLDebugDisplaySettingsData data)
		{
			return null;
		}


		public static DebugUI.ObjectPopupField CreateCameraSelector(VLDebugDisplaySettingsData data, Action<DebugUI.Field<UnityEngine.Object>, UnityEngine.Object> refresh)
		{
			return null;
		}


		public static DebugUI.Button CreateCaptureRenderPassButton(VLDebugDisplaySettingsData data)
		{
			return null;
		}
	}

	[DisplayInfo(name = "VL Utility", order = 100)]
	private class VLRenderingPanel : DebugDisplaySettingsPanel<VLDebugDisplaySettingsData>
	{

		public override DebugUI.Flags Flags
		{


			get
			{
				return default(DebugUI.Flags);
			}
		}


		public VLRenderingPanel(VLDebugDisplaySettingsData data) : base(default) // TODO(qa-recon): original constructor chaining unavailable.
	{
		}


		private void Refresh()
		{
		}
	}


	[CompilerGenerated]
	private bool _003CdisableRenderPass_003Ek__BackingField;


	[CompilerGenerated]
	private Vector2 _003CdynamicResolution_003Ek__BackingField;


	[CompilerGenerated]
	private bool _003CdisableDeferredLight_003Ek__BackingField;


	[CompilerGenerated]
	private bool _003CdisableDeferredReflection_003Ek__BackingField;


	[CompilerGenerated]
	private bool _003CdisableActor_003Ek__BackingField;


	[CompilerGenerated]
	private bool _003CdisableDeferredFog_003Ek__BackingField;


	[CompilerGenerated]
	private VLDebugDisplaySettingsPreviewRenderPass _003CpreviewRenderPass_003Ek__BackingField;

	public bool disableRenderPass
	{


		[CompilerGenerated]
		get
		{
			return default(bool);
		}


		[CompilerGenerated]
		private set
		{
		}
	}

	public Vector2 dynamicResolution
	{


		[CompilerGenerated]
		get
		{
			return default(Vector2);
		}


		[CompilerGenerated]
		private set
		{
		}
	}

	public bool disableDeferredLight
	{


		[CompilerGenerated]
		get
		{
			return default(bool);
		}


		[CompilerGenerated]
		private set
		{
		}
	}

	public bool disableDeferredReflection
	{


		[CompilerGenerated]
		get
		{
			return default(bool);
		}


		[CompilerGenerated]
		private set
		{
		}
	}

	public bool disableActor
	{


		[CompilerGenerated]
		get
		{
			return default(bool);
		}


		[CompilerGenerated]
		private set
		{
		}
	}

	public bool disableDeferredFog
	{


		[CompilerGenerated]
		get
		{
			return default(bool);
		}


		[CompilerGenerated]
		private set
		{
		}
	}

	public VLDebugDisplaySettingsPreviewRenderPass previewRenderPass
	{


		[CompilerGenerated]
		get
		{
			return null;
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


	public VLDebugDisplaySettingsData()
	{
	}
}

}
