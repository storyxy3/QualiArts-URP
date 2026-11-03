// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using VL.Rendering;

public class ConvolutionBlur
{
	[Serializable]

	[CompilerGenerated]
	private sealed class _003C_003Ec
	{


		public static readonly _003C_003Ec _003C_003E9;


		public _003C_003Ec()
		{
		}


		internal int _003C_002Ecctor_003Eb__15_0(int x)
		{
			return default(int);
		}
	}

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
