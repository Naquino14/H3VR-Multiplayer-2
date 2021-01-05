using UnityEngine;

namespace FistVR
{
	public class GlowStick : MonoBehaviour
	{
		public GlowStickColor[] Colors;

		public AlloyAreaLight Light;

		public Renderer Rend;

		private void Awake()
		{
			int num = Random.Range(0, Colors.Length);
			Light.Color = Colors[num].LightColor;
			Rend.material = Colors[num].Mat;
		}
	}
}
