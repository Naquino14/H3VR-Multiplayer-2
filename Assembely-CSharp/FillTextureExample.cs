using UnityEngine;

public class FillTextureExample : MonoBehaviour
{
	public Texture2D myTex;

	public Color32[] colors;

	private void Update()
	{
		int num = myTex.width * myTex.height;
		if (num != colors.Length)
		{
			colors = new Color32[num];
		}
		myTex.SetPixels32(colors);
		myTex.Apply();
	}
}
