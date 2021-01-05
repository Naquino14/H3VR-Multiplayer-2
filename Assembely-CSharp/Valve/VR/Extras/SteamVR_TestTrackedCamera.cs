using UnityEngine;

namespace Valve.VR.Extras
{
	public class SteamVR_TestTrackedCamera : MonoBehaviour
	{
		public Material material;

		public Transform target;

		public bool undistorted = true;

		public bool cropped = true;

		private void OnEnable()
		{
			SteamVR_TrackedCamera.VideoStreamTexture videoStreamTexture = SteamVR_TrackedCamera.Source(undistorted);
			videoStreamTexture.Acquire();
			if (!videoStreamTexture.hasCamera)
			{
				base.enabled = false;
			}
		}

		private void OnDisable()
		{
			material.mainTexture = null;
			SteamVR_TrackedCamera.VideoStreamTexture videoStreamTexture = SteamVR_TrackedCamera.Source(undistorted);
			videoStreamTexture.Release();
		}

		private void Update()
		{
			SteamVR_TrackedCamera.VideoStreamTexture videoStreamTexture = SteamVR_TrackedCamera.Source(undistorted);
			Texture2D texture = videoStreamTexture.texture;
			if (!(texture == null))
			{
				material.mainTexture = texture;
				float num = (float)texture.width / (float)texture.height;
				if (cropped)
				{
					VRTextureBounds_t frameBounds = videoStreamTexture.frameBounds;
					material.mainTextureOffset = new Vector2(frameBounds.uMin, frameBounds.vMin);
					float num2 = frameBounds.uMax - frameBounds.uMin;
					float num3 = frameBounds.vMax - frameBounds.vMin;
					material.mainTextureScale = new Vector2(num2, num3);
					num *= Mathf.Abs(num2 / num3);
				}
				else
				{
					material.mainTextureOffset = Vector2.zero;
					material.mainTextureScale = new Vector2(1f, -1f);
				}
				target.localScale = new Vector3(1f, 1f / num, 1f);
				if (videoStreamTexture.hasTracking)
				{
					SteamVR_Utils.RigidTransform transform = videoStreamTexture.transform;
					target.localPosition = transform.pos;
					target.localRotation = transform.rot;
				}
			}
		}
	}
}
