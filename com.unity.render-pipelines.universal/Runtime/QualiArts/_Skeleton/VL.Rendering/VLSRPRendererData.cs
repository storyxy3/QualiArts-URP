// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]

[ReloadGroup]
[ExcludeFromPreset]
public class VLSRPRendererData : UniversalRendererData
{


	public Shader deferredFogShader;


	public Shader skyShader;


	public Material defaultLitMaterial;


	public Material defaultDecalMaterial;


	protected override void OnEnable()
	{
	}


	protected override ScriptableRenderer Create()
	{
		return null;
	}


	public VLSRPRendererData()
	{
	}
}

}
