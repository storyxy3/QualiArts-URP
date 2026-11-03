#!/usr/bin/env python3
"""Regenerate owned fork skeletons from the game's DummyDlls. No tests."""
from pathlib import Path
import subprocess,os
root=Path(__file__).resolve().parents[2]
dotnet='/data/xy/.toolchains/dotnet-8/dotnet'
helper='/data/xy/gakumas/live-urp/.recon/metadata-tool/SkeletonMetadata.dll'
env=dict(os.environ,DOTNET_ROOT='/data/xy/.toolchains/dotnet-8',DOTNET_ROLL_FORWARD='Major')
for stage,assembly in [('urp','Unity.RenderPipelines.Universal.Runtime'),('core','Unity.RenderPipelines.Core.Runtime')]:
 dll=Path('/data/xy/gakumas/work/il2cpp-3.5.3/DummyDll')/(assembly+'.dll')
 raw=root/'.qa-work'/stage;raw.mkdir(parents=True,exist_ok=True)
 subprocess.run(['/data/xy/.toolchains/ilspycmd/ilspycmd','--disable-updatecheck','-lv','CSharp9_0','-p','-o',str(raw),str(dll)],check=True,env=env)
 subprocess.run([dotnet,helper,str(dll),str(root/'.qa-work'/(stage+'-fields'))],check=True)
subprocess.run([dotnet,'build',str(root/'Tools/SkeletonMerge'),'--nologo','-v:q'],check=True)
merger=[dotnet,str(root/'Tools/SkeletonMerge/bin/Debug/net8.0/SkeletonMerge.dll'),str(root)]
subprocess.run(merger+['generate-fields'],check=True)
subprocess.run(merger+['merge'],check=True)
subprocess.run(['python3',str(root/'Tools/SkeletonMerge/finish_skeleton.py')],check=True)
subprocess.run(['python3',str(root/'Tools/SkeletonMerge/api_repairs.py')],check=True)
subprocess.run(merger+['repair'],check=True)
