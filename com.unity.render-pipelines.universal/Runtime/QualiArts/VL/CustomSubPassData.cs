using UnityEngine.Rendering;

namespace VL
{

public class CustomSubPassData
{
	public int[] colors;

	public int[] inputs;

	public bool isDepthReadOnly;

	public bool isStencilReadOnly;

	public CustomRenderAction renderAction;

	public ProfilingSampler profilingSampler;

	public CustomSubPassData()
	{
	}
}

}
