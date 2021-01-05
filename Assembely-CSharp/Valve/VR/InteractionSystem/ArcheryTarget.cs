using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Valve.VR.InteractionSystem
{
	public class ArcheryTarget : MonoBehaviour
	{
		public UnityEvent onTakeDamage;

		public bool onceOnly;

		public Transform targetCenter;

		public Transform baseTransform;

		public Transform fallenDownTransform;

		public float fallTime = 0.5f;

		private const float targetRadius = 0.25f;

		private bool targetEnabled = true;

		private void ApplyDamage()
		{
			OnDamageTaken();
		}

		private void FireExposure()
		{
			OnDamageTaken();
		}

		private void OnDamageTaken()
		{
			if (targetEnabled)
			{
				onTakeDamage.Invoke();
				StartCoroutine(FallDown());
				if (onceOnly)
				{
					targetEnabled = false;
				}
			}
		}

		private IEnumerator FallDown()
		{
			if ((bool)baseTransform)
			{
				Quaternion startingRot = baseTransform.rotation;
				float startTime = Time.time;
				float rotLerp = 0f;
				while (rotLerp < 1f)
				{
					rotLerp = Util.RemapNumberClamped(Time.time, startTime, startTime + fallTime, 0f, 1f);
					baseTransform.rotation = Quaternion.Lerp(startingRot, fallenDownTransform.rotation, rotLerp);
					yield return null;
				}
			}
			yield return null;
		}
	}
}
