using UnityEngine;

public class GetTextureSizeHack : MonoBehaviour
{
	public int Width;

	public int Height;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Width = source.width;
		Height = source.height;
	}
}
