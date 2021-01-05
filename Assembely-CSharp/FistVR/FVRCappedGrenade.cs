using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRCappedGrenade : FVRMeleeWeapon
	{
		public enum CappedGrenadeFuseType
		{
			Timed,
			Impact
		}

		[Header("Grenade Stuff")]
		public GameObject Cap_Primary_Prefab;

		public bool IsPrimaryCapRemoved;

		public bool UsesSecondaryCap;

		public GameObject Cap_Secondary_Prefab;

		public bool IsSecondaryCapRemoved;

		public CappedGrenadeFuseType FuseType;

		public float FuseTime = 4f;

		private float m_fuseTimeLeft = 4f;

		private bool m_IsFuseActive;

		public AudioEvent AudEvent_CapRemovePrimary;

		public AudioEvent AudEvent_CapRemoveSecondary;

		[Header("Explosion Stuff")]
		public GameObject[] SpawnOnDestruction;

		private bool m_hasExploded;

		public bool HasPopOutShell;

		public List<Transform> ShellPieces;

		public List<Vector3> ShellPoses;

		protected override void Start()
		{
			base.Start();
			m_fuseTimeLeft = FuseTime;
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (m_IsFuseActive && FuseType == CappedGrenadeFuseType.Timed)
			{
				m_fuseTimeLeft -= Time.deltaTime;
				if (m_fuseTimeLeft <= 0f)
				{
					Explode();
				}
			}
		}

		public void CapRemoved(bool isPrimary, FVRViveHand hand, FVRCappedGrenadeCap cap)
		{
			if (isPrimary)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_CapRemovePrimary, base.transform.position);
				IsPrimaryCapRemoved = true;
			}
			else
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_CapRemoveSecondary, base.transform.position);
				IsSecondaryCapRemoved = true;
			}
			if ((UsesSecondaryCap && IsPrimaryCapRemoved && IsSecondaryCapRemoved) || (!UsesSecondaryCap && IsPrimaryCapRemoved))
			{
				m_IsFuseActive = true;
			}
			GameObject gameObject = null;
			gameObject = ((!isPrimary) ? Cap_Secondary_Prefab : Cap_Primary_Prefab);
			GameObject gameObject2 = Object.Instantiate(gameObject, cap.transform.position, cap.transform.rotation);
			FVRPhysicalObject component = gameObject2.GetComponent<FVRPhysicalObject>();
			cap.EndInteraction(hand);
			hand.ForceSetInteractable(component);
			component.BeginInteraction(hand);
			Object.Destroy(cap.gameObject);
			if (HasPopOutShell)
			{
				for (int i = 0; i < ShellPieces.Count; i++)
				{
					ShellPieces[i].localPosition = ShellPoses[i];
				}
			}
		}

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			if (m_IsFuseActive && FuseType == CappedGrenadeFuseType.Impact && col.relativeVelocity.magnitude > 1.5f && !base.IsHeld)
			{
				Explode();
			}
		}

		private void Explode()
		{
			if (!m_hasExploded)
			{
				m_hasExploded = true;
				for (int i = 0; i < SpawnOnDestruction.Length; i++)
				{
					Object.Instantiate(SpawnOnDestruction[i], base.transform.position, base.transform.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}
}
