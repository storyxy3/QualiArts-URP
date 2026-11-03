// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using VL.EntitySystem;
using VL.Rendering;

[ExecuteAlways]
[AddComponentMenu("Rendering/Planar Reflection")]
public class PlanarReflection : MonoBehaviour, IEntityComponent
{

	public enum MaskType
	{

		Box,

		Mesh
	}

	public struct ShaderTagIdList
	{

		private const int kMaxShaderPasses = 15;


		public int count;


		public unsafe fixed int _tagIds[15];


		public ShaderTagIdList(string[] shaderTagNames) : this()
		{
		}


		public ReadOnlySpan<ShaderTagId> AsReadOnlySpan()
		{
			return default(ReadOnlySpan<ShaderTagId>);
		}
	}


	public MaskType maskType;


	public Vector2 size;


	public bool fullResolution;


	public Mesh mesh;


	public LayerMask layerMask;


	public int minimumRenderQueue;


	public int maximumRenderQueue;


	public uint renderingLayerMask;


	public bool additionalShaderTagId;


	public string[] shaderTagNames;


	private ShaderTagIdList _shaderTagIdList;


	private bool _isShaderTagIdListDirty;


	[CompilerGenerated]
	private Entity _003Centity_003Ek__BackingField;

	public ShaderTagIdList shaderTagIdList
	{


		get
		{
			return default(ShaderTagIdList);
		}
	}

	public Entity entity
	{


		[CompilerGenerated]
		get
		{
			return default(Entity);
		}


		[CompilerGenerated]
		set
		{
		}
	}


	public void SetDirty()
	{
	}


	private void OnEnable()
	{
	}


	private void OnDisable()
	{
	}


	private void OnValidate()
	{
	}


	public PlanarReflection()
	{
	}


	Transform VL.EntitySystem.IEntityComponent.transform
	{
		get { return null; }
	}
}

}
