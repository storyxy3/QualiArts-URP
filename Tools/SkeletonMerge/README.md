# Game rendering skeleton merge

Input: Gakumas APK 3.5.3 DummyDlls in `work/il2cpp-3.5.3/DummyDll`.
The initial base is fork commit `34071c8`; changes are on `live-base` only.

`python3 Tools/SkeletonMerge/regenerate.py` runs ILSpy project decompilation,
then the project's `SkeletonMetadata` helper with automatic properties/events
disabled to retain metadata fields, and merges declarations into this worktree.
The helper is built by the live project's skeleton generation script. Staging
and compiler output belong to the ignored `.qa-work` directory.

`Program.cs` inventories types/members with Roslyn, merges absent declarations,
and mechanically initializes placeholder out arguments / struct constructors.
`finish_skeleton.py` applies the shared Python attribute cleanup and required
base-constructor placeholders; these are marked TODO and are not reconstructed
behavior. `api_repairs.py` records game visibility, virtual modifiers, enum
values, field types, and two property-storage/order adjustments that cannot be
inferred merely by looking for absent source members. No game method body has
been reconstructed here.

The generated tree contains 143 URP top-level types/delegates and one Core type
(`RenderPipelineGraphicsSettingsContainer`). Compiler-generated field names are
escaped in C# and restored by the live project's IL postprocessor. Classes
already defined by the fork are skipped. Stock class additions were committed
separately; their original implementations remain intact for Task B to replace.

Compilation evidence: direct Core and URP runtime compilation succeeds with
Unity 6000.0.77's reference-project response options under both Editor and Android
player defines. Full project import and the Android IL2CPP layout report are
owned by the live project.

Metadata declaration comparison (before IL2CPP stripping) found all original
non-compiler-generated types and instance fields, with matching field types and
order after amendments. This is not an offset comparison. Stock-only fields
remain in Core's RenderGraphDebugParams,
DynamicResolutionHandler, DebugUI.EnumField, ObjectParameter<T>; and URP's
DBufferRenderPass, DecalProjector, DecalDrawSystem, DrawScreenSpaceUIPass.PassData,
UniversalRendererData.ShaderResources. They may affect the
IL2CPP report, depending on stripping. Their stock code has not been rewritten
merely to remove storage.

The `fields`, `field-types`, and `visibility` tool modes accept an input assembly
and JSON output path for declaration comparison, e.g.:

```
dotnet Tools/SkeletonMerge/bin/Debug/net8.0/SkeletonMerge.dll field-types input.dll output.json
```

Inherited package `Tests` directories were removed, and the URP/ShaderGraph
Editor asmdefs no longer infer VFX authoring availability from the installed
runtime-only VFX package. Runtime VFX integration remains enabled.

## Stock-only instance storage before IL2CPP stripping

The following is an assembly-metadata comparison using Android player compiler
defines, not the acceptance report's native offset comparison. Sources are the
original DummyDll and the newly compiled Core/URP DLLs; detailed local evidence
is `.qa-work/{Core,Universal}-{game,player}-{fields,field-types}.json`.

| Assembly | Type (namespace abbreviated) | Extra fields |
| --- | --- | --- |
| Core | RenderGraphDebugParams | `m_DebugItems`, `m_DebugPanel` |
| Core | DynamicResolutionHandler | `m_OwnerCameraWeakRef` |
| Core | DebugUI.EnumField | `m_Indexes` |
| Core | ObjectParameter<T> | `<parameters>k__BackingField` |
| Universal | DBufferRenderPass | `m_DBufferDepth`, `<depthHandle>k__BackingField` |
| Universal | DecalProjector | `m_OldDrawDistance`, `m_OldFadeScale`, `m_OldStartAngleFade`, `m_OldEndAngleFade`, `m_OldUVScale`, `m_OldUVBias`, `m_OldScaleMode`, `m_OldOffset`, `m_OldSize`, `m_OldFadeFactor` |
| Universal | DecalDrawSystem | `<overrideMaterial>k__BackingField` |
| Universal | DrawScreenSpaceUIPass.PassData | `offscreenTexture` |
| Universal | UniversalRendererData.ShaderResources | `terrainDetailLitPS`, `terrainDetailGrassPS`, `terrainDetailGrassBillboardPS` |

No missing instance fields remained among shared non-compiler-generated types.
The sole field-type difference initially found was
`UniversalAdditionalLightData.m_LightLayerMask` (stock `LightLayerEnum`, game
`uint`); the field and public property now use `uint`. The only ordering
difference among types with the same field set was ScriptableRenderer's
`_screenToWorld`; it now follows DebugHandler's backing field and precedes
supportedRenderingFeatures' backing field. DebugDisplaySettingsRendering's
missing gameViewOnly backing field was also restored.


Native-report follow-up: the first ARM64 build exposed an eight-byte inherited
shift in VLSRPRenderer caused by stock UniversalRenderer.m_ColorFrontBuffer.
This field had no usages anywhere in the universal package (declaration only)
and was removed without replacing any method behavior. A subsequent native
build must confirm the corrected offsets. UniversalRenderPipelineAsset now
uses the game's `RenderPipelineAsset<UniversalRenderPipeline>` base, and
ScriptableRenderer's constant/static declaration placement also matches the
game. Generated skeletons were regenerated with anonymous methods, anonymous
types and local-function reconstruction disabled, retaining the original
compiler-generated closure declarations for ILPP name restoration.

RenderGraphDefaultResources.m_IsValid was likewise declaration-only and removed.
DynamicResolutionHandler.cachedOriginalSize had one write and no reads; its
field and dead assignment were removed. The live weak-reference owner field
remains because removing it would require reconstructing pooling behavior.
