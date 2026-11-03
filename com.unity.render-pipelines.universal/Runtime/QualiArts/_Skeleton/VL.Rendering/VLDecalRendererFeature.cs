// Generated from Gakumas 3.5.3 DummyDll; method bodies are placeholders.
namespace VL.Rendering {
using System.Runtime.CompilerServices;
using UnityEngine.Rendering.Universal;
using VL.Rendering;

[DisallowMultipleRendererFeature("VL Decal")]
internal class VLDecalRendererFeature : ScriptableRendererFeature
{

	public const float MaxDrawDistance = 100f;


	public int layer;


	[CompilerGenerated]
	private static readonly SharedDecalEntityManager _003CsharedDecalEntityManager_003Ek__BackingField;


	private bool m_RecreateSystems;


	private DecalPreviewPass m_DecalPreviewPass;


	private VLDecalPass m_LitDecalPass;


	private VLDecalPass m_ActorDecalPass;


	private DecalEntityManager m_DecalEntityManager;


	private DecalUpdateCachedSystem m_DecalUpdateCachedSystem;


	private DecalUpdateCullingGroupSystem m_DecalUpdateCullingGroupSystem;


	private DecalUpdateCulledSystem m_DecalUpdateCulledSystem;


	private DecalCreateDrawCallSystem m_DecalCreateDrawCallSystem;


	private DecalPrioritySystem m_DecalPrioritySystem;


	private VLDecalDrawSystem _vlLitDecalDrawSystem;


	private VLDecalDrawSystem _actorDecalDrawSystem;

	private static SharedDecalEntityManager sharedDecalEntityManager
	{


		[CompilerGenerated]
		get
		{
			return null;
		}
	}


	public override void Create()
	{
	}


	private void RecreateSystemsIfNeeded()
	{
	}


	public override void OnCameraPreCull(ScriptableRenderer renderer, in CameraData cameraData)
	{
	}


	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
	}


	protected override void Dispose(bool disposing)
	{
	}


	public VLDecalRendererFeature()
	{
	}
}

}
