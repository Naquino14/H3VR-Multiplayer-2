using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class WW_Speaker : MonoBehaviour, IFVRDamageable
	{
		public WW_TeleportMaster Master;

		public AudioSource AudioSource;

		public AudioLowPassFilter LPF;

		public bool HasMessage;

		public int MessageToUnlock = 30;

		public Transform SpawnPoint;

		public List<GameObject> SpawnOnDestroy;

		private bool m_isDestroyed;

		private void Update()
		{
			float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
			if (num > 50f && AudioSource.isPlaying)
			{
				AudioSource.Stop();
			}
			else if (num < 50f)
			{
				if (!AudioSource.isPlaying)
				{
					AudioSource.Play();
				}
				Vector3 to = GM.CurrentPlayerBody.transform.position - base.transform.position;
				float value = Vector3.Angle(base.transform.forward, to);
				value = Mathf.Clamp(value, 0f, 120f);
				float num2 = value / 120f;
				float volume = 0.75f - num2 * 0.4f;
				AudioSource.volume = volume;
				float cutoffFrequency = Mathf.Lerp(22000f, 1200f, num2);
				LPF.cutoffFrequency = cutoffFrequency;
			}
		}

		public void Damage(Damage d)
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				if (HasMessage && !GM.Options.XmasFlags.MessagesAcquired[MessageToUnlock])
				{
					Master.UnlockMessage(MessageToUnlock);
				}
				for (int i = 0; i < SpawnOnDestroy.Count; i++)
				{
					Object.Instantiate(SpawnOnDestroy[i], SpawnPoint.position, Quaternion.identity);
				}
				base.gameObject.SetActive(value: false);
			}
		}
	}
}
