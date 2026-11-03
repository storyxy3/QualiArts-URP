#!/usr/bin/env python3
"""Mechanical C# compilability repairs; no game method behavior is supplied."""
from pathlib import Path
import re,sys
root=Path(__file__).resolve().parents[2]
sys.path.insert(0,'/data/xy/gakumas/scripts')
from skeleton_from_dummydll import clean_source
for pkg in ['core','universal']:
 for p in (root/f'com.unity.render-pipelines.{pkg}/Runtime/QualiArts/_Skeleton').rglob('*.cs'):
  s=clean_source(p.read_text())
  s=re.sub(r' : base\([^\n]*\) // TODO\(qa-recon\): original constructor chaining unavailable.','',s)
  # Dummy generic null returns are invalid without a reference type constraint.
  s=re.sub(r'return \((T\w*)\)null;',r'return default(\1);',s)
  # Metadata does not retain constructor chaining. Required base arguments use
  # defaults only; their reconstruction remains TODO(qa-recon).
  m=re.search(r'class (\w+) : VolumeParameter<(.+)>',s)
  if m:
   cls=m[1]
   s=re.sub(r'((?:public|internal) '+cls+r'\([^\n]*\))\s*\{',r'\1 : base(default, false) // TODO(qa-recon): original constructor chaining unavailable.\n\t{',s)
  for cls,baseargs in [('VLSRPRenderer','data'),('VLSimpleRenderer','data'),('VLMotionVectorRenderPass','evt, cameraMaterial, objectMaterial, opaqueLayerMask'),('VLPostProcessPass','default, default, ref postProcessParams'),('VLRenderingPanel','default')]:
   s=re.sub(r'((?:public|internal) '+cls+r'\([^\n]*\))\s*\{',r'\1 : base('+baseargs+r') // TODO(qa-recon): original constructor chaining unavailable.\n\t{',s)
  p.write_text(s)
