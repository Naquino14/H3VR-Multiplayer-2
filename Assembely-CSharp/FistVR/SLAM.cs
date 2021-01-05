using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SLAM : FVRPhysicalObject, IFVRDamageable
	{
		public enum SLAMMode
		{
			Disabled,
			LaserPlace,
			LaserArmed,
			ThrownArmed
		}

		[Header("ModalStuff")]
		public SLAMMode Mode;

		[Header("Components")]
		public Transform FlipPiece;

		public Vector2 FlipPieceRots;

		public GameObject Light_Red;

		public GameObject Light_Green;

		[Header("LaserShit")]
		public GameObject Laser_Root;

		public Transform Laser_Beam;

		public LayerMask LM_Beam;

		private float m_storedLaserLength = -1f;

		private RaycastHit m_hit;

		private bool m_isPriming;

		private float m_primeLerp;

		private bool m_isPrimed;

		[Header("SpawnOnDestroy")]
		public List<GameObject> SpawnOnDestroy;

		public Transform SpawnPoint;

		private bool m_hasDetonated;

		public List<GameObject> SpawnOnDestroy_Broad;

		public Transform SpawnPoint_Broad;

		[Header("Audio")]
		public AudioEvent AudEvent_Flip;

		public AudioEvent AudEvent_ArmButtonClick;

		public AudioEvent AudEvent_Priming;

		public AudioEvent AudEvent_Armed;

		[Header("AttachSensors")]
		public LayerMask LM_Attach;

		public override bool IsInteractable()
		{
			if (Mode == SLAMMode.LaserArmed)
			{
				return false;
			}
			return base.IsInteractable();
		}

		public override bool IsDistantGrabbable()
		{
			if (Mode == SLAMMode.LaserArmed)
			{
				return false;
			}
			return base.IsDistantGrabbable();
		}

		protected override void Start()
		{
			base.Start();
			FXM.RegisterSLAM(this);
		}

		public void SetMode(SLAMMode m)
		{
			Mode = m;
			switch (Mode)
			{
			case SLAMMode.Disabled:
				FlipPiece.localEulerAngles = new Vector3(FlipPieceRots.x, 0f, 0f);
				Light_Red.SetActive(value: false);
				Light_Green.SetActive(value: true);
				Laser_Root.SetActive(value: false);
				Size = FVRPhysicalObjectSize.Medium;
				break;
			case SLAMMode.LaserPlace:
				FlipPiece.localEulerAngles = new Vector3(FlipPieceRots.y, 0f, 0f);
				Light_Red.SetActive(value: false);
				Light_Green.SetActive(value: true);
				Size = FVRPhysicalObjectSize.CantCarryBig;
				break;
			case SLAMMode.LaserArmed:
				FlipPiece.localEulerAngles = new Vector3(FlipPieceRots.y, 0f, 0f);
				Light_Red.SetActive(value: true);
				Light_Green.SetActive(value: false);
				Laser_Root.SetActive(value: true);
				PrimeLaser();
				Size = FVRPhysicalObjectSize.CantCarryBig;
				break;
			case SLAMMode.ThrownArmed:
				FlipPiece.localEulerAngles = new Vector3(FlipPieceRots.x, 0f, 0f);
				Light_Red.SetActive(value: true);
				Light_Green.SetActive(value: false);
				Laser_Root.SetActive(value: false);
				Size = FVRPhysicalObjectSize.Medium;
				break;
			}
		}

		public void TriggerFlipped(bool isFlip)
		{
			if (base.QuickbeltSlot != null)
			{
				return;
			}
			switch (Mode)
			{
			case SLAMMode.Disabled:
				if (isFlip)
				{
					SetMode(SLAMMode.LaserPlace);
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Flip, base.transform.position);
				}
				else
				{
					SetMode(SLAMMode.ThrownArmed);
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_ArmButtonClick, base.transform.position);
				}
				break;
			case SLAMMode.LaserPlace:
				if (isFlip)
				{
					SetMode(SLAMMode.Disabled);
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Flip, base.transform.position);
				}
				break;
			case SLAMMode.LaserArmed:
				if (!isFlip)
				{
					DetachAndDisarm();
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_ArmButtonClick, base.transform.position);
				}
				break;
			case SLAMMode.ThrownArmed:
				if (!isFlip)
				{
					SetMode(SLAMMode.Disabled);
					SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_ArmButtonClick, base.transform.position);
					SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Armed, base.transform.position);
				}
				break;
			}
		}

		private void DetachAndDisarm()
		{
			SetMode(SLAMMode.Disabled);
			base.RootRigidbody.isKinematic = false;
		}

		private void PrimeLaser()
		{
			m_isPriming = true;
			m_primeLerp = 0f;
		}

		private void EngageLaser()
		{
			Laser_Root.SetActive(value: true);
			float storedLaserLength = 200f;
			if (Physics.Raycast(Laser_Root.transform.position, Laser_Root.transform.forward, out m_hit, 200f, LM_Beam, QueryTriggerInteraction.Ignore))
			{
				storedLaserLength = m_hit.distance;
			}
			m_storedLaserLength = storedLaserLength;
			Laser_Beam.localScale = new Vector3(0.02f, 0.02f, m_storedLaserLength * 100f);
		}

		protected override void FVRUpdate()
		{
			if (m_isPriming)
			{
				m_primeLerp += Time.deltaTime;
				if (m_primeLerp > 3f)
				{
					m_isPriming = false;
					m_isPrimed = true;
					SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Armed, base.transform.position);
					EngageLaser();
				}
			}
			if (m_isPrimed && Mode == SLAMMode.LaserArmed)
			{
				LaserTest();
			}
			if (Mode == SLAMMode.LaserPlace)
			{
				AttachTest();
			}
		}

		private void AttachTest()
		{
			if (Physics.Raycast(base.transform.position, base.transform.forward, out m_hit, 0.1f, LM_Attach, QueryTriggerInteraction.Ignore))
			{
				base.transform.position = m_hit.point;
				base.transform.rotation = Quaternion.LookRotation(-m_hit.normal, base.transform.up);
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Priming, base.transform.position);
				SetMode(SLAMMode.LaserArmed);
				if (base.IsHeld)
				{
					ForceBreakInteraction();
				}
				base.RootRigidbody.isKinematic = true;
			}
		}

		private void LaserTest()
		{
			float num = 200f;
			if (Physics.Raycast(Laser_Root.transform.position, Laser_Root.transform.forward, out m_hit, 200f, LM_Beam, QueryTriggerInteraction.Ignore))
			{
				num = m_hit.distance;
			}
			if (m_storedLaserLength > 0f && Mathf.Abs(m_storedLaserLength - num) > 0.1f)
			{
				Detonate();
			}
			m_storedLaserLength = num;
			Laser_Beam.localScale = new Vector3(0.02f, 0.02f, m_storedLaserLength * 100f);
		}

		public void Detonate()
		{
			if (m_hasDetonated)
			{
				return;
			}
			FXM.DeRegisterSLAM(this);
			m_hasDetonated = true;
			if (Mode == SLAMMode.LaserArmed || Mode == SLAMMode.LaserPlace)
			{
				for (int i = 0; i < SpawnOnDestroy.Count; i++)
				{
					Object.Instantiate(SpawnOnDestroy[i], SpawnPoint.position, SpawnPoint.rotation);
				}
			}
			else
			{
				for (int j = 0; j < SpawnOnDestroy.Count; j++)
				{
					Object.Instantiate(SpawnOnDestroy_Broad[j], SpawnPoint_Broad.position, SpawnPoint_Broad.rotation);
				}
			}
			Object.Destroy(base.gameObject);
		}

		public void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Projectile && (Mode == SLAMMode.LaserArmed || Mode == SLAMMode.ThrownArmed))
			{
				Detonate();
			}
		}
	}
}
