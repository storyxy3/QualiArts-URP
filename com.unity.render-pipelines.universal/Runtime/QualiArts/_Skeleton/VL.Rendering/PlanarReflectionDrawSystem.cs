// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

public class PlanarReflectionDrawSystem : IDisposable
{

	public const int MaxPlanarReflection = 3;

	public const int StencilSSR = 2;

	public const int Stencil0 = 4;

	public const int Stencil1 = 8;


	public static readonly int[] StencilRefs;


	public static readonly int _StencilRef;


	private readonly PlanarReflectionEntityManager _entityManager;


	private readonly ProfilingSampler _drawSampler;


	private Material[] _materials;


	private readonly RenderStateBlock[] _renderStateBlocks;


	private readonly int _stencilPass;


	private readonly int _maskPass;


	private readonly int _resetPass;


	private int _planarReflectionCount;


	public bool isSystemExecuted;

	public bool IsActive
	{


		get
		{
			return default(bool);
		}
	}


	public PlanarReflectionDrawSystem(PlanarReflectionEntityManager entityManager, Material sourceMaterial, int stencilPass, int maskPass, int resetPass)
	{
	}


	public void Dispose()
	{
	}


	public void CompleteJob()
	{
	}


	public void DrawPlanarReflections(CommandBuffer cmd, ScriptableRenderContext context, ref RenderingData renderingData)
	{
	}
}

}
