// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class VLDecalDrawSystem
{


	public static readonly int _NormalToWorld;


	public static readonly int _DecalLayerMaskFromDecal;


	public static readonly int _Color;


	protected DecalEntityManager m_EntityManager;


	private Matrix4x4[] m_WorldToDecals;


	private Matrix4x4[] m_NormalToDecals;


	private float[] m_DecalLayerMasks;


	private float[] m_Colors;


	private ProfilingSampler m_Sampler;


	private DecalType _decalType;


	public VLDecalDrawSystem(string sampler, DecalType type, int layer, uint renderingLayerMask, DecalEntityManager entityManager)
	{
	}


	public void Execute(CommandBuffer cmd)
	{
	}


	private void Execute(CommandBuffer cmd, DecalEntityChunk decalEntityChunk, DecalCachedChunk decalCachedChunk, DecalDrawCallChunk decalDrawCallChunk)
	{
	}


	private void Draw(CommandBuffer cmd, DecalEntityChunk decalEntityChunk, MaterialPropertyBlock materialPropertyBlock, DecalDrawCallChunk decalDrawCallChunk)
	{
	}
}

}
