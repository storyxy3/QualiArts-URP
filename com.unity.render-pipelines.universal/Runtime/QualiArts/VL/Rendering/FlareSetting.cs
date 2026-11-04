using System;
using UnityEngine;

namespace VL.Rendering
{

[Serializable]
public struct FlareSetting
{
	public FlareType type;

	public bool fixToBaseAspect;

	public Vector2 center;

	[ColorUsage(false, true)]
	public Color color0;

	[ColorUsage(false, true)]
	public Color color1;

	public Vector2 size;
}

}
