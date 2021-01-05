using UnityEngine;

namespace FistVR
{
	public class HairsprayCan : FVRPhysicalObject, IFVRDamageable
	{
		public Transform Tip;

		public Vector2 TipRange;

		private bool m_isSpraying;

		private bool m_wasSpraying;

		private bool m_isIgnited;

		private bool m_wasIgnited;

		public LayerMask LM_sprayCast;

		private RaycastHit m_hit;

		public string Filltag;

		public float SprayDistance;

		public float SprayAngle;

		public AudioEvent AudEvent_Head;

		public AudioEvent AudEvent_Tail;

		public AudioSource AudSource_Loop;

		public AudioClip AudClip_Spray;

		public AudioClip AudClip_Fire;

		public Transform Muzzle;

		public ParticleSystem PSystem_Spray;

		public GameObject Proj_Fire;

		public GameObject IgnitorTrigger;

		public AudioEvent AudEvent_Rattle;

		public float RattleRadius;

		public float RattleHeight;

		private Vector3 m_rattlePos = Vector3.zero;

		private Vector3 m_rattleLastPos = Vector3.zero;

		private Vector3 m_rattleVel = Vector3.zero;

		private bool m_israttleSide;

		private bool m_wasrattleSide;

		public Transform ballviz;

		private bool m_hasExploed;

		public GameObject Splode;

		private float m_timeTilCast = 0.03f;

		public void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Projectile || (d.Dam_Thermal > 0f && m_isSpraying))
			{
				Explode();
			}
		}

		private void Explode()
		{
			if (!m_hasExploed)
			{
				m_hasExploed = true;
				Object.Instantiate(Splode, base.transform.position, base.transform.rotation);
				Object.Destroy(base.gameObject);
			}
		}

		private void RattleUpdate()
		{
			m_wasrattleSide = m_israttleSide;
			if (m_wasrattleSide)
			{
				m_rattleVel = base.RootRigidbody.velocity;
			}
			else
			{
				m_rattleVel -= base.RootRigidbody.GetPointVelocity(base.transform.TransformPoint(m_rattlePos)) * Time.deltaTime;
			}
			m_rattleVel += Vector3.up * -0.5f * Time.deltaTime;
			Vector3 vector = base.transform.InverseTransformDirection(m_rattleVel);
			m_rattlePos += vector * Time.deltaTime;
			float num = m_rattlePos.y;
			Vector2 vector2 = new Vector2(m_rattlePos.x, m_rattlePos.z);
			m_israttleSide = false;
			float magnitude = vector2.magnitude;
			if (magnitude > RattleRadius)
			{
				float num2 = RattleRadius - magnitude;
				vector2 = Vector3.ClampMagnitude(vector2, RattleRadius);
				num += num2 * Mathf.Sign(num);
				vector = Vector3.ProjectOnPlane(vector, new Vector3(vector2.x, 0f, vector2.y));
				m_israttleSide = true;
			}
			if (Mathf.Abs(num) > RattleHeight)
			{
				num = RattleHeight * Mathf.Sign(num);
				vector.y = 0f;
				m_israttleSide = true;
			}
			m_rattlePos = new Vector3(vector2.x, num, vector2.y);
			m_rattleVel = base.transform.TransformDirection(vector);
			ballviz.localPosition = m_rattlePos;
			if (m_israttleSide && !m_wasrattleSide)
			{
				float num3 = Mathf.Clamp(4f * (Vector3.Distance(m_rattlePos, m_rattleLastPos) / RattleRadius), 0f, 1f);
				SM.PlayCoreSoundOverrides(FVRPooledAudioType.Casings, AudEvent_Rattle, ballviz.position, new Vector2(num3 * 0.4f, num3 * 0.4f), new Vector2(1f, 1f));
			}
			m_rattleLastPos = m_rattlePos;
		}

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			m_isSpraying = false;
			if (m_hasTriggeredUpSinceBegin)
			{
				SetAnimatedComponent(Tip, Mathf.Lerp(TipRange.x, TipRange.y, hand.Input.TriggerFloat), InterpStyle.Translate, Axis.Y);
				if (hand.Input.TriggerFloat > 0.8f)
				{
					m_isSpraying = true;
				}
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			SetAnimatedComponent(Tip, TipRange.x, InterpStyle.Translate, Axis.Y);
			m_isSpraying = false;
		}

		public void Ignite()
		{
			if (m_isSpraying)
			{
				m_isIgnited = true;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			RattleUpdate();
			if (m_isSpraying)
			{
				m_timeTilCast -= Time.deltaTime;
				if (m_isIgnited)
				{
					GameObject gameObject = Object.Instantiate(Proj_Fire, Muzzle.position, Muzzle.rotation);
					gameObject.transform.Rotate(new Vector3(Random.Range(0f - SprayAngle, SprayAngle), Random.Range(0f - SprayAngle, SprayAngle), 0f));
					BallisticProjectile component = gameObject.GetComponent<BallisticProjectile>();
					component.Fire(component.MuzzleVelocityBase, gameObject.transform.forward, null);
				}
				else
				{
					PSystem_Spray.Emit(10);
					if (m_timeTilCast < 0f)
					{
						m_timeTilCast = 0.03f;
						Vector3 forward = Muzzle.forward;
						forward = Quaternion.AngleAxis(Random.Range((0f - SprayAngle) * 3f, SprayAngle * 3f), Muzzle.up) * forward;
						forward = Quaternion.AngleAxis(Random.Range((0f - SprayAngle) * 3f, SprayAngle * 3f), Muzzle.right) * forward;
						if (Physics.Raycast(Muzzle.position, Muzzle.forward, out m_hit, SprayDistance, LM_sprayCast, QueryTriggerInteraction.Ignore) && m_hit.collider.gameObject.CompareTag(Filltag))
						{
							PotatoGun component2 = m_hit.collider.attachedRigidbody.gameObject.GetComponent<PotatoGun>();
							component2.InsertGas(0.04f);
						}
					}
				}
			}
			if (m_isSpraying && !m_wasSpraying)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Head, Muzzle.position);
				AudSource_Loop.clip = AudClip_Spray;
				AudSource_Loop.Play();
			}
			else if (m_wasSpraying && !m_isSpraying)
			{
				SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_Tail, Muzzle.position);
				AudSource_Loop.Stop();
			}
			if (m_isSpraying && !m_isIgnited)
			{
				if (!IgnitorTrigger.activeSelf)
				{
					IgnitorTrigger.SetActive(value: true);
				}
			}
			else if (IgnitorTrigger.activeSelf)
			{
				IgnitorTrigger.SetActive(value: false);
			}
			if (!m_isSpraying)
			{
				m_isIgnited = false;
			}
			if (m_isIgnited && !m_wasIgnited)
			{
				AudSource_Loop.clip = AudClip_Fire;
				AudSource_Loop.Play();
			}
			m_wasSpraying = m_isSpraying;
			m_wasIgnited = m_isIgnited;
		}
	}
}
