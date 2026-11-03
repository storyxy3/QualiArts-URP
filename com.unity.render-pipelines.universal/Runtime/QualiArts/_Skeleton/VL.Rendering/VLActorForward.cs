// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

public class VLActorForward : ScriptableRenderPass
{
	[Serializable]

	[CompilerGenerated]
	private sealed class _003C_003Ec
	{


		public static readonly _003C_003Ec _003C_003E9;


		public static DebugHandler.DrawFunction _003C_003E9__7_0;


		public _003C_003Ec()
		{
		}


		internal void _003CExecute_003Eb__7_0(ScriptableRenderContext ctx, ref RenderingData data, ref DrawingSettings ds, ref FilteringSettings fs, ref RenderStateBlock rsb)
		{
		}
	}


	private RenderStateBlock _stateBlock;


	private FilteringSettings _filteringSettings;


	private readonly ProfilingSampler _profilingSampler;


	private readonly List<ShaderTagId> _shaderTagIds;


	private readonly List<ShaderTagId> _shaderTagIdsCapture;


	private readonly ShaderTagId _outlineTagId;


	public VLActorForward(LayerMask layerMask)
	{
	}


	public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}
}

}
