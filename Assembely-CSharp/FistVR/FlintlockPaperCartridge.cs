using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FlintlockPaperCartridge : FVRPhysicalObject, IFVRDamageable
	{
		public enum CartridgeState
		{
			Whole,
			BitOpen,
			Jammed
		}

		public List<Renderer> Rends;

		public CartridgeState CState;

		public int numPowderChunksLeft = 20;

		public AudioEvent AudEvent_Bite;

		public AudioEvent AudEvent_Tap;

		public Transform PowderSpawnPoint;

		public GameObject PowderPrefab;

		public GameObject BitPart;

		public AudioEvent AudEvent_Spit;

		public GameObject Splode;

		private bool m_isDestroyed;

		private float m_tickDownToSpit = 0.2f;

		private bool m_tickingDownToSpit;

		private float timeSinceSpawn = 1f;

		private void SetRenderer(CartridgeState s)
		{
			for (int i = 0; i < Rends.Count; i++)
			{
				if (s == (CartridgeState)i)
				{
					Rends[i].enabled = true;
				}
				else
				{
					Rends[i].enabled = false;
				}
			}
		}

		protected override void Awake()
		{
			base.Awake();
			SetRenderer(CState);
		}

		public void Damage(Damage d)
		{
			if (numPowderChunksLeft > 0 && !m_isDestroyed)
			{
				m_isDestroyed = true;
				Object.Instantiate(Splode, base.transform.position, base.transform.rotation);
				Object.Destroy(base.gameObject);
			}
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (m_hasTriggeredUpSinceBegin)
			{
				float num = Vector3.Angle(-base.transform.forward, Vector3.up);
				if (num > 60f && hand.Input.TriggerDown && CState != 0)
				{
					TapOut();
				}
			}
			if (CState == CartridgeState.Whole)
			{
				Vector3 b = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f;
				if (Vector3.Distance(base.transform.position, b) < 0.15f)
				{
					SM.PlayGenericSound(AudEvent_Bite, base.transform.position);
					CState = CartridgeState.BitOpen;
					SetRenderer(CState);
					m_tickingDownToSpit = true;
					m_tickDownToSpit = Random.Range(0.3f, 0.6f);
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
			if (m_tickingDownToSpit)
			{
				m_tickDownToSpit -= Time.deltaTime;
				if (m_tickDownToSpit <= 0f)
				{
					m_tickingDownToSpit = false;
					Vector3 vector = GM.CurrentPlayerBody.Head.transform.position + GM.CurrentPlayerBody.Head.transform.up * -0.2f;
					SM.PlayGenericSound(AudEvent_Spit, vector);
					GameObject gameObject = Object.Instantiate(BitPart, vector, Random.rotation);
					Rigidbody component = gameObject.GetComponent<Rigidbody>();
					component.velocity = GM.CurrentPlayerBody.Head.forward * Random.Range(2f, 4f) + Random.onUnitSphere;
					component.angularVelocity = Random.onUnitSphere * Random.Range(1f, 5f);
				}
			}
			if (CState == CartridgeState.BitOpen)
			{
				float num = Vector3.Angle(-base.transform.forward, Vector3.up);
				if (num > 120f && numPowderChunksLeft > 0 && timeSinceSpawn > 0.04f)
				{
					numPowderChunksLeft--;
					timeSinceSpawn = 0f;
					Object.Instantiate(PowderPrefab, PowderSpawnPoint.position, Random.rotation);
				}
			}
		}

		private void TapOut()
		{
			SM.PlayGenericSound(AudEvent_Tap, base.transform.position);
			if (numPowderChunksLeft > 0)
			{
				numPowderChunksLeft--;
				timeSinceSpawn = 0f;
				Object.Instantiate(PowderPrefab, PowderSpawnPoint.position, Random.rotation);
			}
		}
	}
}
