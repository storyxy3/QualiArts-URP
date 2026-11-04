using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace VL.Rendering
{

[DisallowMultipleRendererFeature("VL Decal")]
internal class VLDecalRendererFeature : ScriptableRendererFeature
{
	public const float MaxDrawDistance = 100f;

	public int layer;

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

	private static SharedDecalEntityManager sharedDecalEntityManager { get; } = new SharedDecalEntityManager();

	public override void Create()
	{
        m_DecalPreviewPass = new DecalPreviewPass();
        m_LitDecalPass = new VLDecalPass(RenderingBridge.EventType.AfterGBuffer);
        m_ActorDecalPass = new VLDecalPass(RenderingBridge.EventType.ActorDecal);
        m_RecreateSystems = true;
	}

	private void RecreateSystemsIfNeeded()
	{
        if (!m_RecreateSystems) return;
        if (m_DecalEntityManager == null) m_DecalEntityManager = sharedDecalEntityManager.Get();
        m_DecalUpdateCachedSystem = new DecalUpdateCachedSystem(m_DecalEntityManager);
        m_DecalUpdateCulledSystem = new DecalUpdateCulledSystem(m_DecalEntityManager);
        m_DecalCreateDrawCallSystem = new DecalCreateDrawCallSystem(m_DecalEntityManager, MaxDrawDistance);
        m_DecalPrioritySystem = new DecalPrioritySystem(m_DecalEntityManager);
        m_DecalUpdateCullingGroupSystem = new DecalUpdateCullingGroupSystem(m_DecalEntityManager, MaxDrawDistance);
        // These two constructor arguments are unused in the game binary.
        _vlLitDecalDrawSystem = new VLDecalDrawSystem("VLLitDecalSystem.Execute", (DecalType)0, layer, 0, m_DecalEntityManager);
        _actorDecalDrawSystem = new VLDecalDrawSystem("ActorDecalSystem.Execute", (DecalType)1, layer, 0, m_DecalEntityManager);
        m_LitDecalPass.SetDrawSystem(_vlLitDecalDrawSystem);
        m_ActorDecalPass.SetDrawSystem(_actorDecalDrawSystem);
        m_RecreateSystems = false;
	}

	public override void OnCameraPreCull(ScriptableRenderer renderer, in CameraData cameraData)
	{
        if (cameraData.cameraType == CameraType.Preview) return;
        RecreateSystemsIfNeeded();
        m_DecalEntityManager.Update();
        m_DecalUpdateCachedSystem.Execute();
        m_DecalUpdateCullingGroupSystem.Execute(cameraData.camera);
	}

	public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
	{
        if (renderingData.cameraData.cameraType == CameraType.Preview)
        {
            renderer.EnqueuePass(m_DecalPreviewPass);
            return;
        }
        RecreateSystemsIfNeeded();
        m_DecalUpdateCulledSystem.Execute();
        m_DecalPrioritySystem.Execute();
        m_DecalCreateDrawCallSystem.Execute();
        renderer.EnqueuePass(m_LitDecalPass);
        renderer.EnqueuePass(m_ActorDecalPass);
	}

	protected override void Dispose(bool disposing)
	{
        if (m_DecalEntityManager != null)
        {
            sharedDecalEntityManager.Release(m_DecalEntityManager);
            m_DecalEntityManager = null;
        }
	}

	public VLDecalRendererFeature()
	{
	}
}

}
