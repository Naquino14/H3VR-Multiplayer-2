using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_Fade : MonoBehaviour
	{
		private Color currentColor = new Color(0f, 0f, 0f, 0f);

		private Color targetColor = new Color(0f, 0f, 0f, 0f);

		private Color deltaColor = new Color(0f, 0f, 0f, 0f);

		private bool fadeOverlay;

		private static Material fadeMaterial;

		private static int fadeMaterialColorID = -1;

		public static void Start(Color newColor, float duration, bool fadeOverlay = false)
		{
			SteamVR_Events.Fade.Send(newColor, duration, fadeOverlay);
		}

		public static void View(Color newColor, float duration)
		{
			OpenVR.Compositor?.FadeToColor(duration, newColor.r, newColor.g, newColor.b, newColor.a, bBackground: false);
		}

		public void OnStartFade(Color newColor, float duration, bool fadeOverlay)
		{
			if (duration > 0f)
			{
				targetColor = newColor;
				deltaColor = (targetColor - currentColor) / duration;
			}
			else
			{
				currentColor = newColor;
			}
		}

		private void OnEnable()
		{
			if (fadeMaterial == null)
			{
				fadeMaterial = new Material(Shader.Find("Custom/SteamVR_Fade"));
				fadeMaterialColorID = Shader.PropertyToID("fadeColor");
			}
			SteamVR_Events.Fade.Listen(OnStartFade);
			SteamVR_Events.FadeReady.Send();
		}

		private void OnDisable()
		{
			SteamVR_Events.Fade.Remove(OnStartFade);
		}

		private void OnPostRender()
		{
			if (currentColor != targetColor)
			{
				if (Mathf.Abs(currentColor.a - targetColor.a) < Mathf.Abs(deltaColor.a) * Time.deltaTime)
				{
					currentColor = targetColor;
					deltaColor = new Color(0f, 0f, 0f, 0f);
				}
				else
				{
					currentColor += deltaColor * Time.deltaTime;
				}
				if (fadeOverlay)
				{
					SteamVR_Overlay instance = SteamVR_Overlay.instance;
					if (instance != null)
					{
						instance.alpha = 1f - currentColor.a;
					}
				}
			}
			if (currentColor.a > 0f && (bool)fadeMaterial)
			{
				fadeMaterial.SetColor(fadeMaterialColorID, currentColor);
				fadeMaterial.SetPass(0);
				GL.Begin(7);
				GL.Vertex3(-1f, -1f, 0f);
				GL.Vertex3(1f, -1f, 0f);
				GL.Vertex3(1f, 1f, 0f);
				GL.Vertex3(-1f, 1f, 0f);
				GL.End();
			}
		}
	}
}
