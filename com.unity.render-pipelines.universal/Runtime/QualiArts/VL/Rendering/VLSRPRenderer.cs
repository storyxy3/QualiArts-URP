// VL renderer (APK 3.5.3, VL.Rendering.VLSRPRenderer), reconstructed from the game; based on URP 14 UniversalRenderer.
namespace VL.Rendering {
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;
using VL.Rendering;
using VL.Rendering.Internal;

public sealed class VLSRPRenderer : UniversalRenderer
{
	// DMM PC allocates _CameraDepthTexture / _MotionVectorDepthTexture as D32_SFloat, the APK as D24_UNorm: UniversalRenderer's platform switch.
#if (UNITY_SWITCH || UNITY_ANDROID) && !UNITY_EDITOR
	const GraphicsFormat k_VLDepthFormat = GraphicsFormat.D24_UNorm;
#else
	const GraphicsFormat k_VLDepthFormat = GraphicsFormat.D32_SFloat;
#endif
	private VLMotionVectorRenderPass m_VLMotionVectorPass;


	private VLSkyPass m_DrawSkyboxPass;


	internal VLCameraBufferSystem m_LitColorTexture;


	internal RTHandle m_AdditionalInfoTexture;


	private Material m_SkyMaterial;


	private Material m_FogMaterial;


	private VLPostProcessPasses m_VLPostProcessPasses;


	private VLDeferredPass _vlDeferredPass;


	private VLActorForward m_ActorForward;


	private VLPreDepthIDPass _vlPreDepthIDPass;

	internal new ColorGradingLutPass colorGradingLutPass => m_VLPostProcessPasses.colorGradingLutPass;

	internal ColorGradingComputePass colorGradingComputePass => m_VLPostProcessPasses.colorGradingComputePass;

	internal new VLPostProcessPass postProcessPass => m_VLPostProcessPasses.postProcessPass;

	internal new VLPostProcessPass finalPostProcessPass => m_VLPostProcessPasses.finalPostProcessPass;

	internal new RTHandle colorGradingLut => m_VLPostProcessPasses.colorGradingLut;

	public ForwardLights forwardLights => m_ForwardLights;


	public VLSRPRenderer(VLSRPRendererData data) : base(data)
	{
		m_VLMotionVectorPass = new VLMotionVectorRenderPass((RenderPassEvent)400, m_CameraMotionVecMaterial, m_ObjectMotionVecMaterial, data.opaqueLayerMask);
		m_ActorForward = new VLActorForward(data.opaqueLayerMask);
		m_SkyMaterial = CoreUtils.CreateEngineMaterial(data.skyShader);
		m_DrawSkyboxPass = new VLSkyPass((RenderPassEvent)350, m_SkyMaterial);
		m_FogMaterial = CoreUtils.CreateEngineMaterial(data.deferredFogShader);
		m_TransparentSettingsPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
		m_RenderTransparentForwardPass = new DrawObjectsPass(URPProfileId.DrawTransparentObjects, false, RenderPassEvent.BeforeRenderingTransparents, VLRenderQueue.TransparentRange, data.transparentLayerMask, m_DefaultStencilState, data.defaultStencilState.stencilReference);
		_vlPreDepthIDPass = new VLPreDepthIDPass((RenderPassEvent)150, RenderQueueRange.opaque, data.opaqueLayerMask);
		_vlDeferredPass = new VLDeferredPass((RenderPassEvent)230, data.opaqueLayerMask, data.transparentLayerMask, m_DefaultStencilState, data.defaultStencilState.stencilReference, m_DeferredLights, m_DrawSkyboxPass, m_RenderOpaqueForwardOnlyPass, m_FogMaterial, m_ObjectMotionVecMaterial);
		PostProcessParams postProcessParams = PostProcessParams.Create();
		postProcessParams.blitMaterial = m_BlitMaterial;
		postProcessParams.requestHDRFormat = GraphicsFormat.B10G11R11_UFloatPack32;
		UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
		if (asset)
			postProcessParams.requestHDRFormat = UniversalRenderPipeline.MakeRenderTextureGraphicsFormat(asset.supportsHDR, asset.hdrColorBufferPrecision, false);
		m_VLPostProcessPasses = new VLPostProcessPasses(data.postProcessData as VLPostProcessData, ref postProcessParams);
		m_LitColorTexture = new VLCameraBufferSystem();
		m_CameraDepthAttachment = RTHandles.Alloc(BuiltinRenderTextureType.CameraTarget);
	}


	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		CoreUtils.Destroy(m_SkyMaterial);
		CoreUtils.Destroy(m_FogMaterial);
		m_VLPostProcessPasses.Dispose();
		ConstantBuffer.ReleaseAll();
		RenderingBridge.ClearEvents();
		m_DeferredLights?.DisposePlanarReflection();
	}


	internal override void ReleaseRenderTargets()
	{
		base.ReleaseRenderTargets();
		m_VLPostProcessPasses.ReleaseRenderTargets();
		m_LitColorTexture?.Release();
		m_AdditionalInfoTexture?.Release();
	}


	internal void SetupVLSRPFinalPassDebug(ref CameraData cameraData)
	{
		if ((DebugHandler != null) && DebugHandler.IsActiveForCamera(ref cameraData))
		{
			if (DebugHandler.TryGetFullscreenDebugMode(out DebugFullScreenMode fullScreenDebugMode, out int textureHeightPercent) &&
				(fullScreenDebugMode != DebugFullScreenMode.ReflectionProbeAtlas || m_Clustering))
			{
				Camera camera = cameraData.camera;
				float screenWidth = camera.pixelWidth;
				float screenHeight = camera.pixelHeight;
				var relativeSize = Mathf.Clamp01(textureHeightPercent / 100f);
				var height = relativeSize * screenHeight;
				var width = relativeSize * screenWidth;
				if (fullScreenDebugMode == DebugFullScreenMode.ReflectionProbeAtlas)
				{
					var texture = m_ForwardLights.reflectionProbeManager.atlasRT;
					var targetWidth = height * texture.width / texture.height;
					if (targetWidth > width)
						height = width * texture.height / texture.width;
					else
						width = targetWidth;
				}
				float normalizedSizeX = width / screenWidth;
				float normalizedSizeY = height / screenHeight;
				Rect normalizedRect = new Rect(1 - normalizedSizeX, 1 - normalizedSizeY, normalizedSizeX, normalizedSizeY);
				switch (fullScreenDebugMode)
				{
					case DebugFullScreenMode.Depth:
						DebugHandler.SetDebugRenderTarget(m_DepthTexture.nameID, normalizedRect, true);
						break;
					case DebugFullScreenMode.AdditionalLightsShadowMap:
						DebugHandler.SetDebugRenderTarget(m_AdditionalLightsShadowCasterPass.m_AdditionalLightsShadowmapHandle, normalizedRect, false);
						break;
					case DebugFullScreenMode.MainLightShadowMap:
						DebugHandler.SetDebugRenderTarget(m_MainLightShadowCasterPass.m_MainLightShadowmapTexture, normalizedRect, false);
						break;
					case DebugFullScreenMode.ReflectionProbeAtlas:
						DebugHandler.SetDebugRenderTarget(m_ForwardLights.reflectionProbeManager.atlasRT, normalizedRect, false);
						break;
					case DebugFullScreenMode.MotionVector:
						DebugHandler.SetDebugRenderTarget(m_AdditionalInfoTexture, normalizedRect, false);
						break;
				}
			}
			else
			{
				DebugHandler.ResetDebugRenderTarget();
			}
		}
	}


	public override void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		ref CameraData cameraData = ref renderingData.cameraData;
		Camera camera = cameraData.camera;
		RenderTextureDescriptor cameraTargetDescriptor = cameraData.cameraTargetDescriptor;
		var cmd = renderingData.commandBuffer;
		if (DebugHandler != null && DebugHandler.IsActiveForCamera(ref cameraData))
		{
			if (DebugHandler.WriteToDebugScreenTexture(ref cameraData))
			{
				RenderTextureDescriptor colorDesc = cameraData.cameraTargetDescriptor;
				DebugHandler.ConfigureColorDescriptorForDebugScreen(ref colorDesc, cameraData.pixelWidth, cameraData.pixelHeight);
				RenderingUtils.ReAllocateIfNeeded(ref DebugHandler.DebugScreenColorHandle, colorDesc, name: "_DebugScreenColor");
				RenderTextureDescriptor depthDesc = cameraData.cameraTargetDescriptor;
				DebugHandler.ConfigureDepthDescriptorForDebugScreen(ref depthDesc, k_DepthBufferBits, cameraData.pixelWidth, cameraData.pixelHeight);
				RenderingUtils.ReAllocateIfNeeded(ref DebugHandler.DebugScreenDepthHandle, depthDesc, name: "_DebugScreenDepth");
			}
			if (DebugHandler.HDRDebugViewIsActive(ref cameraData))
			{
				DebugHandler.hdrDebugViewPass.Setup(ref cameraData, DebugHandler.DebugDisplaySettings.lightingSettings.hdrDebugMode);
				EnqueuePass(DebugHandler.hdrDebugViewPass);
			}
		}

		if (cameraData.cameraType != CameraType.Game)
			useRenderPassEnabled = false;

		useDepthPriming = IsDepthPrimingEnabled(ref cameraData);

		if (camera.cameraType == CameraType.Game && camera.targetTexture != null)
			cameraTargetDescriptor.useDynamicScale = camera.targetTexture.useDynamicScale;

		if (IsOffscreenDepthTexture(in cameraData))
		{
			ConfigureCameraTarget(k_CameraTarget, k_CameraTarget);
			SetupRenderPasses(in renderingData);
			EnqueuePass(m_RenderOpaqueForwardPass);
			EnqueuePass(m_RenderTransparentForwardPass);
			return;
		}

		bool isPreviewCamera = cameraData.isPreviewCamera;
		var createColorTexture = ((rendererFeatures.Count != 0 && m_IntermediateTextureMode == IntermediateTextureMode.Always) && !isPreviewCamera) ||
			(Application.isEditor && m_Clustering);

		RenderPassInputSummary renderPassInputs = GetRenderPassInputs(ref renderingData);

		bool requiresRenderingLayer = RenderingLayerUtils.RequireRenderingLayers(this, rendererFeatures,
			cameraTargetDescriptor.msaaSamples,
			out var renderingLayersEvent, out var renderingLayerMaskSize);
		if (IsGLDevice())
			requiresRenderingLayer = false;

		bool renderingLayerProvidesByDepthNormalPass = false;
		bool renderingLayerProvidesRenderObjectPass = false;
		if (requiresRenderingLayer && renderingModeActual != RenderingMode.Deferred)
		{
			switch (renderingLayersEvent)
			{
				case RenderingLayerUtils.Event.DepthNormalPrePass:
					renderingLayerProvidesByDepthNormalPass = true;
					break;
				case RenderingLayerUtils.Event.Opaque:
					renderingLayerProvidesRenderObjectPass = true;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		if (renderingLayerProvidesByDepthNormalPass)
			renderPassInputs.requiresNormalsTexture = true;

		if (m_DeferredLights != null)
		{
			m_DeferredLights.RenderingLayerMaskSize = renderingLayerMaskSize;
			m_DeferredLights.UseDecalLayers = requiresRenderingLayer;
			m_DeferredLights.HasNormalPrepass = renderPassInputs.requiresNormalsTexture;
			m_DeferredLights.IsOverlay = cameraData.renderType == CameraRenderType.Overlay;
		}

		bool applyPostProcessing = cameraData.postProcessEnabled && m_VLPostProcessPasses.isCreated;
		bool anyPostProcessing = renderingData.postProcessingEnabled && m_VLPostProcessPasses.isCreated;
		bool cameraHasPostProcessingWithDepth = applyPostProcessing && cameraData.postProcessingRequiresDepthTexture;
		bool generateColorGradingLUT = cameraData.postProcessEnabled && m_VLPostProcessPasses.isCreated;
		bool isSceneViewOrPreviewCamera = cameraData.isSceneViewCamera || cameraData.isPreviewCamera;
		bool requiresDepthTexture = true;
		bool isGizmosEnabled = false;

		float maxShadowDistance = cameraData.maxShadowDistance;
		float shadowDistance = (maxShadowDistance > 100f || renderingData.shadowData.mainLightShadowCascadesCount > 3) ? maxShadowDistance : 100f;
		renderingData.shadowData.mainLightShadowCascadesSplit *= maxShadowDistance / shadowDistance;

		bool mainLightShadows = m_MainLightShadowCasterPass.Setup(ref renderingData);
		bool additionalLightShadows = m_AdditionalLightsShadowCasterPass.Setup(ref renderingData, shadowDistance, 0.1f);
		bool transparentsNeedSettingsPass = m_TransparentSettingsPass.Setup();

		bool forcePrepass = (m_CopyDepthMode == CopyDepthMode.ForcePrepass);

		bool requiresDepthPrepass = (requiresDepthTexture || cameraHasPostProcessingWithDepth) && (!CanCopyDepth(ref renderingData.cameraData) || forcePrepass);
		requiresDepthPrepass |= isSceneViewOrPreviewCamera;
		requiresDepthPrepass |= isGizmosEnabled;
		requiresDepthPrepass |= isPreviewCamera;
		requiresDepthPrepass |= renderPassInputs.requiresDepthPrepass;
		requiresDepthPrepass |= renderPassInputs.requiresNormalsTexture;

		if (requiresDepthPrepass && this.renderingModeActual == RenderingMode.Deferred && !renderPassInputs.requiresNormalsTexture)
			requiresDepthPrepass = false;

		requiresDepthPrepass |= useDepthPriming;

		// APK 3.5.3 / DMM: set unconditionally, default AfterRenderingSkybox, no post-process fallback branch.
		bool copyDepthAfterTransparents = m_CopyDepthMode == CopyDepthMode.AfterTransparents;
		RenderPassEvent copyDepthPassEvent = copyDepthAfterTransparents ? RenderPassEvent.AfterRenderingTransparents : RenderPassEvent.AfterRenderingSkybox;
		if (renderPassInputs.requiresDepthTexture)
			copyDepthPassEvent = (RenderPassEvent)Mathf.Min((int)RenderPassEvent.AfterRenderingTransparents, ((int)renderPassInputs.requiresDepthTextureEarliestEvent) - 1);
		m_CopyDepthPass.renderPassEvent = copyDepthPassEvent;
		if (copyDepthPassEvent < RenderPassEvent.AfterRenderingTransparents)
		{
			m_CopyDepthPass.m_CopyResolvedDepth = false;
			m_CopyDepthMode = CopyDepthMode.AfterOpaques;
		}

		createColorTexture |= RequiresIntermediateColorTexture(ref cameraData);
		createColorTexture |= renderPassInputs.requiresColorTexture;
		createColorTexture |= renderPassInputs.requiresColorTextureCreated;
		createColorTexture &= !isPreviewCamera;

		bool createDepthTexture = true;
		createDepthTexture |= !cameraData.resolveFinalTarget;
		createDepthTexture |= (this.renderingModeActual == RenderingMode.Deferred && !useRenderPassEnabled);
		createDepthTexture |= useDepthPriming;
		createDepthTexture |= (renderingLayerProvidesRenderObjectPass);

		if (cameraData.xr.enabled)
			createColorTexture |= createDepthTexture;
		if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Vulkan || m_VulkanEnablePreTransform)
			createColorTexture |= createDepthTexture;
		if (RTHandles.rtHandleProperties.rtHandleScale.x != 1.0f || RTHandles.rtHandleProperties.rtHandleScale.y != 1.0f)
			createColorTexture |= createDepthTexture;
		if (useRenderPassEnabled || useDepthPriming)
			createColorTexture |= createDepthTexture;

		var colorDescriptor = cameraTargetDescriptor;
		colorDescriptor.useMipMap = false;
		colorDescriptor.autoGenerateMips = false;
		colorDescriptor.depthBufferBits = (int)DepthBits.None;
		m_ColorBufferSystem.SetCameraSettings(colorDescriptor, FilterMode.Bilinear);

		if (cameraData.renderType == CameraRenderType.Base)
		{
			bool sceneViewFilterEnabled = camera.sceneViewFilterMode == Camera.SceneViewFilterMode.ShowFiltered;
			bool intermediateRenderTexture = (createColorTexture || createDepthTexture) && !sceneViewFilterEnabled;
			createDepthTexture |= createColorTexture;

			RenderTargetIdentifier targetId = BuiltinRenderTextureType.CameraTarget;
			if (cameraData.xr.enabled)
				targetId = cameraData.xr.renderTarget;

			if (m_XRTargetHandleAlias == null)
				m_XRTargetHandleAlias = RTHandles.Alloc(targetId);
			else if (m_XRTargetHandleAlias.nameID != targetId)
				RTHandleStaticHelpers.SetRTHandleUserManagedWrapper(ref m_XRTargetHandleAlias, targetId);

			if (renderingModeActual == RenderingMode.Forward || cameraData.isPreviewCamera)
			{
				if (m_CameraDepthAttachment == null || m_CameraDepthAttachment.nameID == BuiltinRenderTextureType.CameraTarget)
				{
					m_CameraDepthAttachment?.Release();
					m_CameraDepthAttachment = null;
				}
			}

			if (intermediateRenderTexture)
			{
				CreateCameraRenderTarget(context, ref cameraTargetDescriptor, useDepthPriming, cmd, ref cameraData);
				if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Direct3D11)
					cmd.CopyTexture(m_CameraDepthAttachment, m_CameraDepthAttachment_D3d_11);
			}

			m_RenderOpaqueForwardPass.m_IsActiveTargetBackBuffer = !intermediateRenderTexture;
			m_RenderTransparentForwardPass.m_IsActiveTargetBackBuffer = !intermediateRenderTexture;
			m_XROcclusionMeshPass.m_IsActiveTargetBackBuffer = !intermediateRenderTexture;

			m_ActiveCameraColorAttachment = createColorTexture ? m_ColorBufferSystem.PeekBackBuffer() : m_XRTargetHandleAlias;
			m_ActiveCameraDepthAttachment = createDepthTexture ? m_CameraDepthAttachment : m_XRTargetHandleAlias;
		}
		else
		{
			cameraData.baseCamera.TryGetComponent<UniversalAdditionalCameraData>(out var baseCameraData);
			var baseRenderer = (UniversalRenderer)baseCameraData.scriptableRenderer;
			if (m_ColorBufferSystem != baseRenderer.m_ColorBufferSystem)
			{
				m_ColorBufferSystem.Dispose();
				m_ColorBufferSystem = baseRenderer.m_ColorBufferSystem;
			}
			m_ActiveCameraColorAttachment = m_ColorBufferSystem.PeekBackBuffer();
			m_ActiveCameraDepthAttachment = baseRenderer.m_ActiveCameraDepthAttachment;
			m_XRTargetHandleAlias = baseRenderer.m_XRTargetHandleAlias;
		}

		if (rendererFeatures.Count != 0 && !isPreviewCamera)
			ConfigureCameraColorTarget(m_ColorBufferSystem.PeekBackBuffer());

		bool copyColorPass = renderingData.cameraData.requiresOpaqueTexture || renderPassInputs.requiresColorTexture;
		copyColorPass &= !isPreviewCamera;

		ConfigureCameraTarget(m_ActiveCameraColorAttachment, m_ActiveCameraDepthAttachment);

		bool hasPassesAfterPostProcessing = activeRenderPassQueue.Find(x => x.renderPassEvent == RenderPassEvent.AfterRenderingPostProcessing) != null;

		if (mainLightShadows)
			EnqueuePass(m_MainLightShadowCasterPass);
		if (additionalLightShadows)
			EnqueuePass(m_AdditionalLightsShadowCasterPass);

		bool requiresDepthCopyPass = !requiresDepthPrepass
			&& (renderingData.cameraData.requiresDepthTexture || cameraHasPostProcessingWithDepth || renderPassInputs.requiresDepthTexture)
			&& createDepthTexture;

		bool requiresMotionVectors = cameraData.postProcessEnabled && VolumeManager.instance.stack.GetComponent<VLMotionBlur>().IsActive();
		RenderingUtility.SetupMotionVectorData(cmd, ref cameraData);
		context.ExecuteCommandBuffer(cmd);
		cmd.Clear();
		requiresMotionVectors |= renderPassInputs.requiresMotionVectors;

		if ((DebugHandler != null) && DebugHandler.IsActiveForCamera(ref cameraData))
		{
			DebugHandler.TryGetFullscreenDebugMode(out var fullScreenMode);
			if (fullScreenMode == DebugFullScreenMode.Depth)
				requiresDepthPrepass = true;
			else if (fullScreenMode == DebugFullScreenMode.MotionVector)
				requiresMotionVectors = true;

			if (!DebugHandler.IsLightingActive)
			{
				mainLightShadows = false;
				additionalLightShadows = false;
				if (!isSceneViewOrPreviewCamera)
				{
					requiresDepthPrepass = false;
					useDepthPriming = false;
					generateColorGradingLUT = false;
				}
			}

			if (useRenderPassEnabled)
				useRenderPassEnabled = DebugHandler.IsRenderPassSupported;
		}

		cameraData.renderer.useDepthPriming = useDepthPriming;

		var litColorDescriptor = cameraTargetDescriptor;
		litColorDescriptor.depthStencilFormat = GraphicsFormat.None;
		litColorDescriptor.graphicsFormat = GraphicsFormat.B10G11R11_UFloatPack32;
		m_LitColorTexture.ReAllocateIfNeeded(camera.GetHashCode(), litColorDescriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, "_LitColorTexture");

		var additionalInfoDescriptor = cameraTargetDescriptor;
		additionalInfoDescriptor.depthStencilFormat = GraphicsFormat.None;
		additionalInfoDescriptor.graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;
		RenderingUtils.ReAllocateIfNeeded(ref m_AdditionalInfoTexture, additionalInfoDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_AdditionalInfoTexture");

		if ((this.renderingModeActual == RenderingMode.Deferred && !this.useRenderPassEnabled) || requiresDepthPrepass || requiresDepthCopyPass || !isPreviewCamera)
		{
			var depthDescriptor = cameraTargetDescriptor;
			depthDescriptor.graphicsFormat = GraphicsFormat.None;
			depthDescriptor.depthStencilFormat = k_VLDepthFormat;
			depthDescriptor.msaaSamples = 1;
			RenderingUtils.ReAllocateIfNeeded(ref m_DepthTexture, depthDescriptor, FilterMode.Point, wrapMode: TextureWrapMode.Clamp, name: "_CameraDepthTexture");
			cmd.SetGlobalTexture(m_DepthTexture.name, m_DepthTexture.nameID);
			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
		}

		if (requiresRenderingLayer || (renderingModeActual == RenderingMode.Deferred && m_DeferredLights.UseRenderingLayers))
		{
			ref var renderingLayersTexture = ref m_DecalLayersTexture;
			string renderingLayersTextureName = "_CameraRenderingLayersTexture";
			if (this.renderingModeActual == RenderingMode.Deferred && m_DeferredLights.UseRenderingLayers)
			{
				renderingLayersTexture = ref m_DeferredLights.GbufferAttachments[(int)m_DeferredLights.GBufferRenderingLayers];
				renderingLayersTextureName = renderingLayersTexture.name;
			}

			var renderingLayersDescriptor = cameraTargetDescriptor;
			renderingLayersDescriptor.depthBufferBits = 0;
			if (!renderingLayerProvidesRenderObjectPass)
				renderingLayersDescriptor.msaaSamples = 1;

			if (this.renderingModeActual == RenderingMode.Deferred && m_DeferredLights.UseRenderingLayers)
				renderingLayersDescriptor.graphicsFormat = m_DeferredLights.GetGBufferFormat(m_DeferredLights.GBufferRenderingLayers);
			else
				renderingLayersDescriptor.graphicsFormat = RenderingLayerUtils.GetFormat(renderingLayerMaskSize);

			if (renderingModeActual == RenderingMode.Deferred && m_DeferredLights.UseRenderingLayers)
				m_DeferredLights.ReAllocateGBufferIfNeeded(renderingLayersDescriptor, (int)m_DeferredLights.GBufferRenderingLayers);
			else
				RenderingUtils.ReAllocateIfNeeded(ref renderingLayersTexture, renderingLayersDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: renderingLayersTextureName);

			cmd.SetGlobalTexture(renderingLayersTexture.name, renderingLayersTexture.nameID);
			RenderingLayerUtils.SetupProperties(cmd, renderingLayerMaskSize);
			if (this.renderingModeActual == RenderingMode.Deferred)
				cmd.SetGlobalTexture("_CameraRenderingLayersTexture", renderingLayersTexture.nameID);

			context.ExecuteCommandBuffer(cmd);
			cmd.Clear();
		}

		if (!isPreviewCamera)
		{
			var normalDescriptor = cameraTargetDescriptor;
			normalDescriptor.depthStencilFormat = GraphicsFormat.None;
			normalDescriptor.graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm;
			RenderingUtils.ReAllocateIfNeeded(ref m_NormalsTexture, normalDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_CameraNormalsTexture");
			_vlPreDepthIDPass.Setup(m_DepthTexture, m_NormalsTexture);
			EnqueuePass(_vlPreDepthIDPass);
		}
		else if (requiresDepthPrepass)
		{
			if (renderPassInputs.requiresNormalsTexture)
			{
				if (this.renderingModeActual == RenderingMode.Deferred)
				{
					int gbufferNormalIndex = m_DeferredLights.GBufferNormalSmoothnessIndex;
					if (m_DeferredLights.UseRenderingLayers)
						m_DepthNormalPrepass.Setup(m_ActiveCameraDepthAttachment, m_DeferredLights.GbufferAttachments[gbufferNormalIndex], m_DeferredLights.GbufferAttachments[m_DeferredLights.GBufferRenderingLayers]);
					else if (renderingLayerProvidesByDepthNormalPass)
						m_DepthNormalPrepass.Setup(m_ActiveCameraDepthAttachment, m_DeferredLights.GbufferAttachments[gbufferNormalIndex], m_DecalLayersTexture);
					else
						m_DepthNormalPrepass.Setup(m_ActiveCameraDepthAttachment, m_DeferredLights.GbufferAttachments[gbufferNormalIndex]);

					if (RenderPassEvent.AfterRenderingGbuffer <= renderPassInputs.requiresDepthNormalAtEvent &&
						renderPassInputs.requiresDepthNormalAtEvent <= RenderPassEvent.BeforeRenderingOpaques)
						m_DepthNormalPrepass.shaderTagIds = k_DepthNormalsOnly;
				}
				else
				{
					if (renderingLayerProvidesByDepthNormalPass)
						m_DepthNormalPrepass.Setup(m_DepthTexture, m_NormalsTexture, m_DecalLayersTexture);
					else
						m_DepthNormalPrepass.Setup(m_DepthTexture, m_NormalsTexture);
				}
				EnqueuePass(m_DepthNormalPrepass);
			}
			else if (this.renderingModeActual != RenderingMode.Deferred)
			{
				m_DepthPrepass.Setup(cameraTargetDescriptor, m_DepthTexture);
				EnqueuePass(m_DepthPrepass);
			}
		}

		if (useDepthPriming)
		{
			m_PrimedDepthCopyPass.Setup(m_ActiveCameraDepthAttachment, m_DepthTexture);
			EnqueuePass(m_PrimedDepthCopyPass);
		}

		if (generateColorGradingLUT)
		{
			if (postProcessPass.UseComputeGrading)
			{
				colorGradingComputePass.ConfigureDescriptor(in renderingData.postProcessingData, out var desc, out var filterMode);
				RenderingUtils.ReAllocateIfNeeded(ref m_VLPostProcessPasses.m_ColorGradingLut, desc, filterMode, TextureWrapMode.Clamp, anisoLevel: 0, name: "_InternalGradingLut");
				colorGradingComputePass.Setup(m_VLPostProcessPasses.m_ColorGradingLut);
				EnqueuePass(colorGradingComputePass);
			}
			else
			{
				colorGradingLutPass.ConfigureDescriptor(in renderingData.postProcessingData, out var desc, out var filterMode);
				RenderingUtils.ReAllocateIfNeeded(ref m_VLPostProcessPasses.m_ColorGradingLut, desc, filterMode, TextureWrapMode.Clamp, anisoLevel: 0, name: "_InternalGradingLut");
				colorGradingLutPass.Setup(colorGradingLut);
				EnqueuePass(colorGradingLutPass);
			}
		}

		if (cameraData.xr.hasValidOcclusionMesh)
			EnqueuePass(m_XROcclusionMeshPass);

		bool lastCameraInTheStack = cameraData.resolveFinalTarget;
		bool forwardRendering = this.renderingModeActual == RenderingMode.Forward || isPreviewCamera;

		if (forwardRendering)
		{
			RenderBufferStoreAction opaquePassColorStoreAction = RenderBufferStoreAction.Store;
			if (cameraTargetDescriptor.msaaSamples > 1)
				opaquePassColorStoreAction = copyColorPass ? RenderBufferStoreAction.StoreAndResolve : RenderBufferStoreAction.Store;

			RenderBufferStoreAction opaquePassDepthStoreAction = (copyColorPass || requiresDepthCopyPass || !lastCameraInTheStack) ? RenderBufferStoreAction.Store : RenderBufferStoreAction.DontCare;
			if (cameraData.xr.enabled && cameraData.xr.copyDepth)
				opaquePassDepthStoreAction = RenderBufferStoreAction.Store;

			if (requiresDepthCopyPass && cameraTargetDescriptor.msaaSamples > 1 && RenderingUtils.MultisampleDepthResolveSupported())
			{
				bool isCopyDepthAfterTransparent = m_CopyDepthPass.renderPassEvent == RenderPassEvent.AfterRenderingTransparents;
				if (isCopyDepthAfterTransparent && !copyColorPass)
				{
					if (opaquePassDepthStoreAction == RenderBufferStoreAction.Store)
						opaquePassDepthStoreAction = RenderBufferStoreAction.StoreAndResolve;
					else if (opaquePassDepthStoreAction == RenderBufferStoreAction.DontCare)
						opaquePassDepthStoreAction = RenderBufferStoreAction.Resolve;
				}
			}

			DrawObjectsPass renderOpaqueForwardPass = null;
			if (renderingLayerProvidesRenderObjectPass)
			{
				renderOpaqueForwardPass = m_RenderOpaqueForwardWithRenderingLayersPass;
				m_RenderOpaqueForwardWithRenderingLayersPass.Setup(m_ActiveCameraColorAttachment, m_DecalLayersTexture, m_ActiveCameraDepthAttachment);
			}
			else
				renderOpaqueForwardPass = m_RenderOpaqueForwardPass;

			renderOpaqueForwardPass.ConfigureColorStoreAction(opaquePassColorStoreAction);
			renderOpaqueForwardPass.ConfigureDepthStoreAction(opaquePassDepthStoreAction);

			bool hasPassesBeforeOpaque = activeRenderPassQueue.Find(x => (x.renderPassEvent <= RenderPassEvent.BeforeRenderingOpaques) && !x.overrideCameraTarget) != null;
			ClearFlag opaqueForwardPassClearFlag = (hasPassesBeforeOpaque || cameraData.renderType != CameraRenderType.Base)
				? ClearFlag.None
				: ClearFlag.Color;
			if (SystemInfo.usesLoadStoreActions)
				renderOpaqueForwardPass.ConfigureClear(opaqueForwardPassClearFlag, Color.black);

			EnqueuePass(renderOpaqueForwardPass);
			EnqueuePass(m_ActorForward);

			if (camera.clearFlags == CameraClearFlags.Skybox && cameraData.renderType != CameraRenderType.Overlay)
			{
				if (RenderSettings.skybox != null || (camera.TryGetComponent(out Skybox cameraSkybox) && cameraSkybox.material != null))
					EnqueuePass(m_DrawSkyboxPass);
			}
		}
		else
		{
			EnqueueDeferred(ref renderingData, false, false, false, false, additionalLightShadows);
		}

		if (cameraData.renderType == CameraRenderType.Base && !requiresDepthPrepass && !requiresDepthCopyPass)
			Shader.SetGlobalTexture("_CameraDepthTexture", SystemInfo.usesReversedZBuffer ? Texture2D.blackTexture : Texture2D.whiteTexture);

		if (requiresMotionVectors && forwardRendering)
		{
			var depthDescriptor = cameraTargetDescriptor;
			depthDescriptor.graphicsFormat = GraphicsFormat.None;
			depthDescriptor.depthStencilFormat = k_VLDepthFormat;
			depthDescriptor.msaaSamples = 1;
			RenderingUtils.ReAllocateIfNeeded(ref m_MotionVectorDepth, depthDescriptor, FilterMode.Point, TextureWrapMode.Clamp, name: "_MotionVectorDepthTexture");
			m_VLMotionVectorPass.Setup(m_AdditionalInfoTexture, m_MotionVectorDepth);
			m_VLMotionVectorPass.renderPassEvent = (RenderPassEvent)549;
			EnqueuePass(m_VLMotionVectorPass);
		}

		if (transparentsNeedSettingsPass)
			EnqueuePass(m_TransparentSettingsPass);

		if (forwardRendering)
		{
			RenderBufferStoreAction transparentPassColorStoreAction = cameraTargetDescriptor.msaaSamples > 1 && lastCameraInTheStack ? RenderBufferStoreAction.Resolve : RenderBufferStoreAction.Store;
			RenderBufferStoreAction transparentPassDepthStoreAction = lastCameraInTheStack ? RenderBufferStoreAction.DontCare : RenderBufferStoreAction.Store;
			if (requiresDepthCopyPass && m_CopyDepthPass.renderPassEvent >= RenderPassEvent.AfterRenderingTransparents)
			{
				transparentPassDepthStoreAction = RenderBufferStoreAction.Store;
				if (cameraTargetDescriptor.msaaSamples > 1 && RenderingUtils.MultisampleDepthResolveSupported())
					transparentPassDepthStoreAction = RenderBufferStoreAction.Resolve;
			}
			m_RenderTransparentForwardPass.ConfigureColorStoreAction(transparentPassColorStoreAction);
			m_RenderTransparentForwardPass.ConfigureDepthStoreAction(transparentPassDepthStoreAction);
			EnqueuePass(m_RenderTransparentForwardPass);
		}
		EnqueuePass(m_OnRenderObjectCallbackPass);

		bool shouldRenderUI = cameraData.rendersOverlayUI;
		bool outputToHDR = cameraData.isHDROutputActive;
		if (shouldRenderUI && outputToHDR)
		{
			m_DrawOffscreenUIPass.Setup(ref cameraData, k_DepthBufferBits);
			EnqueuePass(m_DrawOffscreenUIPass);
		}

		bool hasCaptureActions = renderingData.cameraData.captureActions != null && lastCameraInTheStack;

		bool applyFinalPostProcessing = anyPostProcessing && lastCameraInTheStack &&
			((renderingData.cameraData.antialiasing == AntialiasingMode.FastApproximateAntialiasing) ||
			 !Mathf.Approximately(renderingData.cameraData.renderScale, 1.0f) ||
			 ((renderingData.cameraData.imageScalingMode == ImageScalingMode.Upscaling) && (renderingData.cameraData.upscalingFilter != ImageUpscalingFilter.Linear)) ||
			 (renderingData.cameraData.IsTemporalAAEnabled() && renderingData.cameraData.taaSettings.contrastAdaptiveSharpening > 0.0f));

		if (DebugHandler != null)
		{
			var renderingSettings = DebugHandler.DebugDisplaySettings.renderingSettings;
			if (renderingSettings.overdrawMode != DebugOverdrawMode.None || renderingSettings.fullScreenDebugMode != DebugFullScreenMode.None || renderingSettings.validationMode != DebugValidationMode.None)
				applyFinalPostProcessing = true;
		}

		bool resolvePostProcessingToCameraTarget = !hasCaptureActions && !hasPassesAfterPostProcessing && !applyFinalPostProcessing;
		bool needsColorEncoding = DebugHandler == null || !DebugHandler.HDRDebugViewIsActive(ref cameraData);

		if (applyPostProcessing)
		{
			var desc = PostProcessPass.GetCompatibleDescriptor(cameraTargetDescriptor, cameraTargetDescriptor.width, cameraTargetDescriptor.height, cameraTargetDescriptor.graphicsFormat, DepthBits.None);
			RenderingUtils.ReAllocateIfNeeded(ref m_VLPostProcessPasses.m_AfterPostProcessColor, desc, FilterMode.Point, TextureWrapMode.Clamp, name: "_AfterPostProcessTexture");
			if (!lastCameraInTheStack)
			{
				postProcessPass.Setup(cameraTargetDescriptor, m_ActiveCameraColorAttachment, false, m_ActiveCameraDepthAttachment, colorGradingLut, m_MotionVectorColor, false, false);
				EnqueuePass(postProcessPass);
				return;
			}
			SetupVLSRPFinalPassDebug(ref cameraData);
			bool doSRGBEncoding = resolvePostProcessingToCameraTarget && needsColorEncoding;
			postProcessPass.Setup(cameraTargetDescriptor, m_ActiveCameraColorAttachment, resolvePostProcessingToCameraTarget, m_ActiveCameraDepthAttachment, colorGradingLut, m_AdditionalInfoTexture, applyFinalPostProcessing, doSRGBEncoding);
			EnqueuePass(postProcessPass);
		}
		else
		{
			if (!lastCameraInTheStack)
				return;
			SetupVLSRPFinalPassDebug(ref cameraData);
		}

		var sourceForFinalPass = m_ActiveCameraColorAttachment;
		if (applyFinalPostProcessing)
		{
			finalPostProcessPass.SetupFinalPass(sourceForFinalPass, true, needsColorEncoding);
			EnqueuePass(finalPostProcessPass);
		}

		if (renderingData.cameraData.captureActions != null)
			EnqueuePass(m_CapturePass);

		if (camera.TryGetComponent(out VLSRPCapture capture))
		{
			if (capture.TryGetRequests(out var requests))
			{
				while (requests.MoveNext())
				{
					var request = requests.Current;
					if (request.sourceType == VLSRPCapture.SourceType.Depth)
					{
						if (request.rtHandle == null)
						{
							Debug.LogError("Depth CaptureはRTHandleで指定する必要があります");
							continue;
						}
						var copyDepthPass = new CopyDepthPass(request.renderPassEvent, m_CopyDepthMaterial);
						copyDepthPass.Setup(m_ActiveCameraDepthAttachment, request.rtHandle);
						EnqueuePass(copyDepthPass);
					}
					else if (request.sourceType == VLSRPCapture.SourceType.Current)
					{
						EnqueuePass(new VLCapturePass(request));
					}
				}
			}
			capture.Clear();
		}

		bool cameraTargetResolved =
			applyFinalPostProcessing ||
			(applyPostProcessing && !hasPassesAfterPostProcessing && !hasCaptureActions) ||
			m_ActiveCameraColorAttachment.nameID == m_XRTargetHandleAlias.nameID;

		if (!cameraTargetResolved)
		{
			m_FinalBlitPass.Setup(cameraTargetDescriptor, sourceForFinalPass);
			EnqueuePass(m_FinalBlitPass);
		}

		if (shouldRenderUI && !outputToHDR)
			EnqueuePass(m_DrawOverlayUIPass);

		if (cameraData.xr.enabled)
		{
			bool depthTargetResolved = m_ActiveCameraDepthAttachment.nameID == cameraData.xr.renderTarget;
			if (!depthTargetResolved && cameraData.xr.copyDepth)
			{
				m_XRCopyDepthPass.Setup(m_ActiveCameraDepthAttachment, m_XRTargetHandleAlias);
				m_XRCopyDepthPass.CopyToDepth = true;
				EnqueuePass(m_XRCopyDepthPass);
			}
		}
	}


	public override void SetupLights(ScriptableRenderContext context, ref RenderingData renderingData)
	{
		m_ForwardLights.Setup(context, ref renderingData);
		m_DeferredLights.SetupLights(context, ref renderingData);
	}


	public override void SetupCullingParameters(ref ScriptableCullingParameters cullingParameters, ref CameraData cameraData)
	{
		if (renderingModeActual != RenderingMode.Forward)
			cullingParameters.cullingOptions |= CullingOptions.DisablePerObjectCulling;
		bool isShadowCastingDisabled = !UniversalRenderPipeline.asset.supportsMainLightShadows && !UniversalRenderPipeline.asset.supportsAdditionalLightShadows;
		bool isShadowDistanceZero = Mathf.Approximately(cameraData.maxShadowDistance, 0.0f);
		if (isShadowCastingDisabled || isShadowDistanceZero)
			cullingParameters.cullingOptions &= ~CullingOptions.ShadowCasters;
		if (renderingModeActual != RenderingMode.Forward)
		{
			cullingParameters.maximumVisibleLights = UniversalRenderPipeline.maxVisibleAdditionalLights;
			cullingParameters.reflectionProbeSortingCriteria = ReflectionProbeSortingCriteria.None;
		}
		else
		{
			cullingParameters.maximumVisibleLights = UniversalRenderPipeline.maxVisibleAdditionalLights + 1;
		}
		cullingParameters.shadowDistance = Mathf.Max(cameraData.maxShadowDistance, 100f);
		cullingParameters.conservativeEnclosingSphere = UniversalRenderPipeline.asset.conservativeEnclosingSphere;
		cullingParameters.numIterationsEnclosingSphere = UniversalRenderPipeline.asset.numIterationsEnclosingSphere;
	}


	public override void FinishRendering(CommandBuffer cmd)
	{
		base.FinishRendering(cmd);
		if (m_DeferredLights != null)
			m_DeferredLights.storeLitColor = false;
		RenderingBridge.ClearEvents();
	}


	private void EnqueueDeferred(ref RenderingData renderingData, bool hasDepthPrepass, bool hasNormalPrepass, bool hasRenderingLayerPrepass, bool applyMainShadow, bool applyAdditionalShadow)
	{
		m_DeferredLights.Setup(applyAdditionalShadow ? m_AdditionalLightsShadowCasterPass : null);
		_vlDeferredPass.Setup(m_DepthTexture, m_NormalsTexture, m_AdditionalInfoTexture);
		EnqueuePass(_vlDeferredPass);
	}


	public bool IsLitColorRendered(int cameraHash)
	{
		return m_LitColorTexture != null && m_LitColorTexture.IsRendered(cameraHash);
	}


	public RTHandle GetLitColorTexture(int cameraHash)
	{
		return m_LitColorTexture?.GetTexture(cameraHash);
	}


	public void MarkLitColorTextureAsRendered(int cameraHash)
	{
		m_LitColorTexture?.MarkAsRendered(cameraHash);
	}


	public int GetLightCookieShaderDataIndex(int visibleLightIndex)
	{
		if (m_LightCookieManager != null)
			return m_LightCookieManager.GetLightCookieShaderDataIndex(visibleLightIndex);
		return -1;
	}
}

}
