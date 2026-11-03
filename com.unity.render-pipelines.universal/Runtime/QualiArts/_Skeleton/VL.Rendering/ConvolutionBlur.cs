// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using UnityEngine;
using UnityEngine.Rendering;

public class ConvolutionBlur
{

	public const int MaxMipCount = 6;

	public const int MinResolution = 64;


	private static readonly float[] GaussOffset;


	private static readonly int _BlurOffset;


	private static readonly int _MipLevel;


	private static readonly int[] _TempConvolutionMips;


	private static readonly ProfilingSampler _profilingSampler;


	private readonly Material _material;


	private readonly int _mipCount;


	private readonly int _blurPass;


	private RenderTextureDescriptor _descriptor;


	private RTHandle[] _tempRTs;


	public ConvolutionBlur(Material material, int blurPass = 1, int mipCount = 6)
	{
	}


	public void Dispose()
	{
	}


	public void Execute(CommandBuffer cmd, RTHandle destination, RenderTextureDescriptor cameraTextureDescriptor)
	{
	}
}

}
