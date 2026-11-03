// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class MipChainBuffer : IDisposable
{

	public const int MaxMipCount = 14;


	public static readonly string PrepareKernelName;


	public static readonly string MipChainKernelName;


	public static readonly int _SrcOffset;


	public static readonly int _DstOffset;


	public static readonly int _Input;


	public static readonly int _Output;


	private readonly ProfilingSampler _profilingSampler;


	private readonly ComputeShader _compute;


	private readonly GraphicsFormat _format;


	private readonly int _threadSize;


	private readonly string _bufferName;


	private readonly int _prepareKernelIndex;


	private readonly int _mipChainKernelIndex;


	private int[] _srcOffset;


	private int[] _dstOffset;


	private RTHandle _mipBuffer;


	[CompilerGenerated]
	private int _003CMipCount_003Ek__BackingField;


	[CompilerGenerated]
	private Vector2Int[] _003CMipMin_003Ek__BackingField;


	[CompilerGenerated]
	private Vector2Int[] _003CMipMax_003Ek__BackingField;


	[CompilerGenerated]
	private Vector2Int[] _003CSize_003Ek__BackingField;

	public int MipCount
	{


		[CompilerGenerated]
		get
		{
			return default(int);
		}


		[CompilerGenerated]
		private set
		{
		}
	}

	public Vector2Int[] MipMin
	{


		[CompilerGenerated]
		get
		{
			return null;
		}
	}

	public Vector2Int[] MipMax
	{


		[CompilerGenerated]
		get
		{
			return null;
		}
	}

	public Vector2Int[] Size
	{


		[CompilerGenerated]
		get
		{
			return null;
		}
	}

	public RenderTargetIdentifier nameID
	{


		get
		{
			return default(RenderTargetIdentifier);
		}
	}


	public MipChainBuffer(ComputeShader compute, GraphicsFormat format, string bufferName)
	{
	}


	public void Dispose()
	{
	}


	public void UpdateMipChainParams(int width, int height, int maxMipCount = -1)
	{
	}


	private void SetComputeOffsetParams(CommandBuffer cmd, Vector2Int srcMin, Vector2Int srcMax, Vector2Int dstMin, Vector2Int srcSize)
	{
	}


	public void Execute(CommandBuffer cmd, int width, int height, [Optional] RTHandle source)
	{
	}


	public void CopyToMipMinMax(Span<Vector4> mipMinMax)
	{
	}


	public static bool IsSupported(ComputeShader computeShader, GraphicsFormat format)
	{
		return default(bool);
	}
}

}
