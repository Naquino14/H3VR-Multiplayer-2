using UnityEngine;

public class RayTester : MonoBehaviour
{
	private RaycastHit hit;

	public LayerMask LayerMask;

	public Texture2D MaskTexture;

	private void Start()
	{
	}

	private void Update()
	{
		if (Physics.Raycast(base.transform.position, base.transform.forward, out hit, 10f, LayerMask) && hit.collider.gameObject.tag == "MaskedTarget")
		{
			int x = Mathf.RoundToInt((float)MaskTexture.width * hit.textureCoord.x);
			int y = Mathf.RoundToInt((float)MaskTexture.width * hit.textureCoord.y);
			Color pixel = MaskTexture.GetPixel(x, y);
			Debug.Log(string.Concat("Color", pixel, " Points:", Mathf.RoundToInt(pixel.a * 10f)));
		}
	}
}
