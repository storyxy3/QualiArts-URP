using UnityEngine.Rendering;

namespace VL
{

public class CustomRenderPassData
{
	public int width;

	public int height;

	public int samples = 1;

	public AttachmentDescriptor[] attachmentDescriptors;

	public int depthAttachmentIndex = -1;

	public bool[] memoryless;

	public CustomRenderPassData()
	{
	}
}

}
