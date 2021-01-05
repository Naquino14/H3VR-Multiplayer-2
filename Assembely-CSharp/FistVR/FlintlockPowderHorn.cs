using UnityEngine;

namespace FistVR
{
	public class FlintlockPowderHorn : FVRPhysicalObject, IFVRDamageable
	{
		public AudioEvent AudEvent_Tap;

		public AudioEvent AudEvent_Cap;

		public Transform PowderSpawnPoint;

		public GameObject PowderPrefab;

		public Transform Cap;

		public Transform CapPoint_On;

		public Transform CapPoint_Off;

		public FlintlockPowderHornCap MyCap;

		private bool m_isCapped = true;

		public Transform PourForward;

		public GameObject Explode;

		public bool UsesTriggerHoldDrop;

		private float timeSinceSpawn;

		private bool m_isDestroyed;

		public bool IsCapped => m_isCapped;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (!m_hasTriggeredUpSinceBegin)
			{
				return;
			}
			float num = Vector3.Angle(PourForward.forward, Vector3.up);
			if (num > 60f && !m_isCapped)
			{
				if (hand.Input.TriggerDown)
				{
					timeSinceSpawn = 0f;
					TapOut(sound: true);
				}
				else if (UsesTriggerHoldDrop && hand.Input.TriggerPressed && timeSinceSpawn > 0.04f && !MyCap.IsFull())
				{
					timeSinceSpawn = 0f;
					TapOut(sound: false);
				}
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (timeSinceSpawn < 1f)
			{
				timeSinceSpawn += Time.deltaTime;
			}
			if (!m_isCapped)
			{
				float num = Vector3.Angle(PourForward.forward, Vector3.up);
				if (num > 120f && timeSinceSpawn > 0.04f)
				{
					timeSinceSpawn = 0f;
					Object.Instantiate(PowderPrefab, PowderSpawnPoint.position, Random.rotation);
				}
			}
		}

		public void ToggleCap()
		{
			m_isCapped = !m_isCapped;
			SM.PlayGenericSound(AudEvent_Cap, base.transform.position);
			if (m_isCapped)
			{
				Cap.position = CapPoint_On.position;
				Cap.rotation = CapPoint_On.rotation;
			}
			else
			{
				Cap.position = CapPoint_Off.position;
				Cap.rotation = CapPoint_Off.rotation;
			}
		}

		public void Damage(Damage d)
		{
			if (d.Dam_Thermal > 1f && !m_isDestroyed)
			{
				m_isDestroyed = true;
				Object.Instantiate(Explode, base.transform.position, base.transform.rotation);
				Object.Destroy(base.gameObject);
			}
		}

		private void TapOut(bool sound)
		{
			if (!m_isCapped)
			{
				if (sound)
				{
					SM.PlayGenericSound(AudEvent_Tap, base.transform.position);
				}
				Object.Instantiate(PowderPrefab, PowderSpawnPoint.position, Random.rotation);
			}
		}
	}
}
