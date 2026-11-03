// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using VL.Rendering;

public class VLCameraBufferSystem
{

	private class RenderData
	{


		private string _name;


		private RTHandle _rtHandle;


		private int _renderedFrame;

		public string name
		{


			get
			{
				return null;
			}
		}

		public RTHandle rtHandle
		{


			get
			{
				return null;
			}
		}

		public bool isRendered
		{


			get
			{
				return default(bool);
			}
		}


		public RenderData(string name)
		{
		}


		public void ReAllocateIfNeeded(in RenderTextureDescriptor descriptor, FilterMode filterMode, TextureWrapMode wrapMode, string name)
		{
		}


		public void Dispose()
		{
		}


		public void MarkRendered()
		{
		}
	}


	private Dictionary<int, RenderData> _renderData;


	public RTHandle ReAllocateIfNeeded(int cameraHash, in RenderTextureDescriptor descriptor, FilterMode filterMode, TextureWrapMode wrapMode, string name)
	{
		return null;
	}


	public RTHandle GetTexture(int cameraHash)
	{
		return null;
	}


	public void MarkAsRendered(int cameraHash)
	{
	}


	public bool IsRendered(int cameraHash)
	{
		return default(bool);
	}


	public void Release()
	{
	}


	public VLCameraBufferSystem()
	{
	}
}

}
