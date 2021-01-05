using UnityEngine;

namespace FistVR
{
	public class RealisticLaserSword : FVRPhysicalObject
	{
		public Transform OuterBeam;

		public Transform InnerBeam;

		public GameObject ImpactPointPrefab;

		private GameObject ImpactPoint;

		private TrailRenderer BurnTrail;

		public ParticleSystem ImpactSparks;

		public ParticleSystem GlowSprites;

		public LayerMask LaserMask;

		public LayerMask TargetMask;

		private RaycastHit m_hit;

		public Transform Aperture;

		private Vector3 LastPoint1 = Vector3.zero;

		private Vector3 LastPoint2 = Vector3.zero;

		private Vector3 LastNormal = Vector3.zero;

		public int maxCasts = 3;

		private bool m_isBeamActive;

		private bool m_wasBeamActive;

		public AudioSource humm;

		protected override void Awake()
		{
			base.Awake();
			ImpactPoint = Object.Instantiate(ImpactPointPrefab, base.transform.position, Quaternion.identity);
			ImpactPoint.SetActive(value: false);
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (hand.Input.TriggerPressed && m_hasTriggeredUpSinceBegin)
			{
				m_isBeamActive = true;
				OuterBeam.gameObject.SetActive(value: true);
				if (!humm.isPlaying)
				{
					humm.Play();
				}
			}
			else
			{
				m_isBeamActive = false;
				OuterBeam.gameObject.SetActive(value: false);
				ImpactPoint.SetActive(value: false);
				humm.Stop();
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			m_isBeamActive = false;
			OuterBeam.gameObject.SetActive(value: false);
			ImpactPoint.SetActive(value: false);
			humm.Stop();
			base.EndInteraction(hand);
		}

		private void GenerateNewTrail(Vector3 point)
		{
			ImpactPoint = Object.Instantiate(ImpactPointPrefab, point, Quaternion.identity);
			BurnTrail = ImpactPoint.GetComponent<TrailRenderer>();
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (m_isBeamActive)
			{
				float z = 100f;
				Vector3 lastPoint = Aperture.position + Aperture.forward * 100f;
				if (Physics.Raycast(Aperture.position, Aperture.forward, out m_hit, 100f, LaserMask, QueryTriggerInteraction.Collide))
				{
					z = m_hit.distance;
					lastPoint = m_hit.point;
					ImpactPoint.SetActive(value: true);
					ImpactPoint.transform.position = m_hit.point;
					if (m_hit.collider.gameObject.layer == LayerMask.NameToLayer("Flammable"))
					{
						m_hit.collider.gameObject.BroadcastMessage("Ignite", SendMessageOptions.DontRequireReceiver);
					}
				}
				else
				{
					ImpactPoint.SetActive(value: false);
				}
				OuterBeam.localScale = new Vector3(0.012f, 0.012f, z);
				if (!m_wasBeamActive)
				{
					LastPoint1 = Aperture.position;
					LastPoint2 = lastPoint;
				}
				else
				{
					for (int i = 0; i < maxCasts; i++)
					{
						float t = (float)i / ((float)maxCasts - 1f);
						Vector3 vector = Vector3.Lerp(LastPoint1, Aperture.position, t);
						Vector3 vector2 = Vector3.Lerp(LastPoint2, Aperture.position, t);
						if (!Physics.Raycast(vector, (vector2 - vector).normalized, out m_hit, 100f, TargetMask, QueryTriggerInteraction.Collide))
						{
							continue;
						}
						ImpactSparks.transform.position = m_hit.point + m_hit.normal * 0.05f;
						ImpactSparks.Emit(1);
						if (m_hit.collider.attachedRigidbody != null && m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>() != null)
						{
							if ((bool)m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>())
							{
								FXM.Ignite(m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>(), 1f);
							}
							Damage damage = new Damage();
							damage.Class = Damage.DamageClass.Projectile;
							damage.Dam_Piercing = 1f;
							damage.Dam_TotalKinetic = 1f;
							damage.Dam_Thermal = 10f;
							damage.Dam_TotalEnergetic = 10f;
							damage.point = m_hit.point;
							damage.hitNormal = m_hit.normal;
							damage.strikeDir = base.transform.forward;
							m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>().Damage(damage);
						}
					}
				}
				LastPoint1 = Aperture.position;
				LastPoint2 = lastPoint;
			}
			m_wasBeamActive = m_isBeamActive;
		}
	}
}
