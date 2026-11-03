// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class DownsampleBlur : IDisposable
{

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass8_0
	{


		public string propertyName;


		public _003C_003Ec__DisplayClass8_0()
		{
		}


		internal string _003C_002Ector_003Eb__0(int x)
		{
			return null;
		}
	}


	private static readonly int _BlurOffset;

	private const int MaxPyramidSize = 5;


	private RTHandle[] _pyramidTextures;


	private string[] _pyramidNames;


	private string _resultName;


	private Material _blurMaterial;


	private int _downPass;


	private int _upPass;


	public DownsampleBlur(string propertyName, Material material, int downPass, int upPass)
	{
	}


	public void Dispose()
	{
	}


	public void ApplyBlur(CommandBuffer cmd, RTHandle source, in RenderTextureDescriptor sourceDescriptor, ref RTHandle target, float size)
	{
	}


	private void ApplyBlur(CommandBuffer cmd, RTHandle source, in RenderTextureDescriptor sourceDescriptor, ref RTHandle target, float size, bool writeMips, out int iterations)
	{
            iterations = default;

	}


	private void SetBlurOffset(CommandBuffer cmd, float scale)
	{
	}
}

}
