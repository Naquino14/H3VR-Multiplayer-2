using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MF2_Detpack : FVRPhysicalObject, IFVRDamageable
	{
		public AudioEvent AudEvent_Beep;

		public AudioEvent AudEvent_Boop;

		public AudioEvent AudEvent_Tick;

		public List<Transform> SpawnPoints;

		public List<GameObject> SpawnOnDestroy;

		public List<GameObject> NumberDisplay;

		private float m_fuseTime = 10f;

		private bool m_isFusing;

		private bool m_hasDetonated;

		private int lastnum = 10;

		private void Detonate()
		{
			if (m_hasDetonated)
			{
				return;
			}
			m_hasDetonated = true;
			for (int i = 0; i < SpawnPoints.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(SpawnOnDestroy[i], SpawnPoints[i].position, SpawnPoints[i].rotation);
				Rigidbody component = gameObject.GetComponent<Rigidbody>();
				if (component != null)
				{
					Vector3 onUnitSphere = Random.onUnitSphere;
					component.velocity = onUnitSphere * Random.Range(0.3f, 2f);
					component.angularVelocity = Random.onUnitSphere * Random.Range(1f, 7f);
				}
			}
			Object.Destroy(base.gameObject);
		}

		public void Damage(Damage d)
		{
			if (!base.IsHeld && !(base.QuickbeltSlot != null))
			{
				m_isFusing = true;
				m_fuseTime = Mathf.Min(m_fuseTime, Random.Range(0.1f, 0.2f));
			}
		}

		public void InitiateCountDown(FVRViveHand hand)
		{
			if (!base.IsHeld || !(hand == m_hand))
			{
				if (!m_isFusing)
				{
					SM.PlayGenericSound(AudEvent_Beep, base.transform.position);
				}
				m_isFusing = true;
				m_fuseTime = Mathf.Min(m_fuseTime, 10f);
			}
		}

		public void ResetCountDown(FVRViveHand hand)
		{
			if (!base.IsHeld || !(hand == m_hand))
			{
				if (m_isFusing)
				{
					SM.PlayGenericSound(AudEvent_Boop, base.transform.position);
				}
				m_isFusing = false;
				m_fuseTime = 10f;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_isFusing)
			{
				m_fuseTime -= Time.deltaTime;
				if (m_fuseTime <= 0f)
				{
					Detonate();
				}
			}
			UpdateDisplay();
		}

		private void UpdateDisplay()
		{
			int value = Mathf.FloorToInt(m_fuseTime);
			value = Mathf.Clamp(value, 0, 10);
			if (value == lastnum)
			{
				return;
			}
			float num = 1f + Mathf.Abs((float)value * 0.1f);
			SM.PlayCoreSoundOverrides(FVRPooledAudioType.Generic, AudEvent_Tick, base.transform.position, new Vector2(1f, 1f), new Vector2(num, num));
			for (int i = 0; i < NumberDisplay.Count; i++)
			{
				if (i == value)
				{
					NumberDisplay[i].SetActive(value: true);
				}
				else
				{
					NumberDisplay[i].SetActive(value: false);
				}
			}
			lastnum = value;
		}
	}
}
