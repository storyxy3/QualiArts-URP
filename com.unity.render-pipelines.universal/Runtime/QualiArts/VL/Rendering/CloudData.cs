using UnityEngine;

namespace VL.Rendering
{

[CreateAssetMenu(fileName = "CloudData", menuName = "VL/Rendering/Cloud Data", order = 0)]
public class CloudData : ScriptableObject
{
	public CloudLayer[] cloudLayers;

	public Material cloudMaterial;

	public CloudData()
	{
	}
}

}
