using UnityEngine;

namespace FistVR
{
	public class PDSysTest : MonoBehaviour
	{
		public Transform PointTest;

		public Texture2D SysTex;

		private void Start()
		{
		}

		private void Update()
		{
			PointTest.localPosition = PointTest.localPosition.normalized * 0.5f;
			Vector2 vector = WhunkSphereMapping.PositionToUVSpherical(PointTest.localPosition, Vector3.right, (Vector3.up - Vector3.forward).normalized);
			vector = new Vector2(vector.y, vector.x);
			Color pixel = SysTex.GetPixel(Mathf.FloorToInt(vector.x * (float)SysTex.width), Mathf.FloorToInt(vector.y * (float)SysTex.height));
			int num = Mathf.FloorToInt(pixel.r * 255f);
			int num2 = Mathf.FloorToInt(pixel.g * 255f);
			int num3 = Mathf.FloorToInt(pixel.b * 255f);
			Debug.Log(vector.x + " " + vector.y + " " + num + " " + num2 + " " + num3);
		}
	}
}
