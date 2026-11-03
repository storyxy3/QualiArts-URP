// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[Serializable]

[ReloadGroup]
[ExcludeFromPreset]
public class VLSimpleRendererData : ScriptableRendererData
{


	[Reload("Shaders/Utils/CoreBlit.shader", ReloadAttribute.Package.Root)]
	[SerializeField]
	internal Shader coreBlitPS;


	[Reload("Shaders/Utils/BlitHDROverlay.shader", ReloadAttribute.Package.Root)]
	[SerializeField]
	internal Shader blitHDROverlay;


	[Reload("Shaders/Utils/CopyDepth.shader", ReloadAttribute.Package.Root)]
	public Shader copyDepthPS;


	protected override ScriptableRenderer Create()
	{
		return null;
	}


	public VLSimpleRendererData()
	{
	}
}

}
