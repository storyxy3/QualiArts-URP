// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using System.Collections.Generic;
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

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass0_0
		{


			public VLDebugDisplaySettingsData data;


			public _003C_003Ec__DisplayClass0_0()
			{
			}


			internal bool _003CCreateDisableRenderPass_003Eb__0()
			{
				return default(bool);
			}


			internal void _003CCreateDisableRenderPass_003Eb__1(bool value)
			{
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass1_0
		{


			public VLDebugDisplaySettingsData data;


			public _003C_003Ec__DisplayClass1_0()
			{
			}


			internal Vector2 _003CCreateDynamicResolution_003Eb__0()
			{
				return default(Vector2);
			}


			internal void _003CCreateDynamicResolution_003Eb__1(Vector2 value)
			{
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass2_0
		{


			public VLDebugDisplaySettingsData data;


			public _003C_003Ec__DisplayClass2_0()
			{
			}


			internal bool _003CCreateDisableDeferredLight_003Eb__0()
			{
				return default(bool);
			}


			internal void _003CCreateDisableDeferredLight_003Eb__1(bool value)
			{
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass3_0
		{


			public VLDebugDisplaySettingsData data;


			public _003C_003Ec__DisplayClass3_0()
			{
			}


			internal bool _003CCreateDisableDeferredReflection_003Eb__0()
			{
				return default(bool);
			}


			internal void _003CCreateDisableDeferredReflection_003Eb__1(bool value)
			{
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass4_0
		{


			public VLDebugDisplaySettingsData data;


			public _003C_003Ec__DisplayClass4_0()
			{
			}


			internal bool _003CCreateDisableActor_003Eb__0()
			{
				return default(bool);
			}


			internal void _003CCreateDisableActor_003Eb__1(bool value)
			{
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass5_0
		{


			public VLDebugDisplaySettingsData data;


			public _003C_003Ec__DisplayClass5_0()
			{
			}


			internal bool _003CCreateDisableDeferredFog_003Eb__0()
			{
				return default(bool);
			}


			internal void _003CCreateDisableDeferredFog_003Eb__1(bool value)
			{
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass6_0
		{


			public VLDebugDisplaySettingsData data;


			public _003C_003Ec__DisplayClass6_0()
			{
			}


			internal UnityEngine.Object _003CCreateCameraSelector_003Eb__0()
			{
				return null;
			}


			internal void _003CCreateCameraSelector_003Eb__1(UnityEngine.Object value)
			{
			}


			internal IEnumerable<UnityEngine.Object> _003CCreateCameraSelector_003Eb__2()
			{
				return null;
			}
		}

		[CompilerGenerated]
		private sealed class _003C_003Ec__DisplayClass7_0
		{


			public VLDebugDisplaySettingsData data;


			public _003C_003Ec__DisplayClass7_0()
			{
			}


			internal void _003CCreateCaptureRenderPassButton_003Eb__0()
			{
			}
		}


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


		[CompilerGenerated]
		private void _003C_002Ector_003Eb__2_0(DebugUI.Field<UnityEngine.Object> _, UnityEngine.Object __)
		{
		}
	}















	public bool disableRenderPass { get; private set; }

	public Vector2 dynamicResolution { get; private set; }

	public bool disableDeferredLight { get; private set; }

	public bool disableDeferredReflection { get; private set; }

	public bool disableActor { get; private set; }

	public bool disableDeferredFog { get; private set; }

	public VLDebugDisplaySettingsPreviewRenderPass previewRenderPass { get; }

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
