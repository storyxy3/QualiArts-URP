using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

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
		return new VLSimpleRenderer(this);
	}


	public VLSimpleRendererData()
	{
	}
}

}
