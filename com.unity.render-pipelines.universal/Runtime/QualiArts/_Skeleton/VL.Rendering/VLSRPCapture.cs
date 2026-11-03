// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

[RequireComponent(typeof(Camera))]
public class VLSRPCapture : MonoBehaviour
{

	public enum SourceType
	{

		Current,

		Depth
	}

	public struct RequestData
	{


		public RTHandle rtHandle;


		public RenderTargetIdentifier target;


		public RenderPassEvent renderPassEvent;


		public SourceType sourceType;


		public Action onCapture;


		public RequestData(RenderTargetIdentifier target, RenderPassEvent renderPassEvent, [Optional] Action onCapture) : this()
		{
		}


		public bool Equals(RequestData other)
		{
			return default(bool);
		}


		public override bool Equals(object obj)
		{
			return default(bool);
		}


		public override int GetHashCode()
		{
			return default(int);
		}
	}


	private HashSet<RequestData> _captureRequests;


	public void Add(RenderTargetIdentifier target, RenderPassEvent renderPassEvent, [Optional] Action onCapture)
	{
	}


	public void Add(RequestData requestData)
	{
	}


	public bool Remove(RenderTargetIdentifier target)
	{
		return default(bool);
	}


	public bool TryGetRequests(out HashSet<RequestData>.Enumerator requests)
	{
            requests = default;

		return default(bool);
	}


	public void Clear()
	{
	}


	public VLSRPCapture()
	{
	}
}

}
