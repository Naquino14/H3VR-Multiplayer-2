using UnityEngine;

namespace FistVR
{
	public class Flipzo : FVRPhysicalObject
	{
		private bool m_isOpen;

		private bool m_isLit;

		private bool isTouching;

		private Vector2 initTouch = Vector2.zero;

		private Vector2 LastTouchPoint = Vector2.zero;

		public Transform Lid;

		private float m_lid_startRot;

		private float m_lid_endRot = -150f;

		private float m_lid_curRot;

		public Transform Spring;

		private float m_spring_startRot = 248f;

		private float m_spring_end_rot = 380f;

		private float m_spring_curRot = 248f;

		public Transform Flame;

		private float m_flame_min = 0.1f;

		private float m_flame_max = 0.85f;

		private float m_flame_cur = 0.1f;

		public AudioSource Audio_Lighter;

		public AudioClip AudioClip_Open;

		public AudioClip AudioClip_Close;

		public AudioClip AudioClip_Strike;

		public Transform[] FlameJoints;

		public float[] FlameWeights;

		public ParticleSystem Sparks;

		public AlloyAreaLight AlloyLight;

		public LayerMask LM_FireDamage;

		private RaycastHit m_hit;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (hand.IsInStreamlinedMode)
			{
				if (hand.Input.BYButtonDown)
				{
					if (m_isOpen)
					{
						Close();
					}
					else
					{
						Open();
					}
				}
				if (hand.Input.AXButtonDown && m_isOpen)
				{
					Light();
				}
			}
			else
			{
				if (hand.Input.TouchpadTouched && !isTouching && hand.Input.TouchpadAxes != Vector2.zero)
				{
					isTouching = true;
					initTouch = hand.Input.TouchpadAxes;
				}
				if (hand.Input.TouchpadTouchUp && isTouching)
				{
					isTouching = false;
					Vector2 lastTouchPoint = LastTouchPoint;
					float y = (lastTouchPoint - initTouch).y;
					if ((double)y > 0.5)
					{
						Open();
					}
					else if ((double)y < -0.5 && m_isOpen)
					{
						Light();
					}
					initTouch = Vector2.zero;
					lastTouchPoint = Vector2.zero;
				}
				LastTouchPoint = hand.Input.TouchpadAxes;
			}
			if (hand.Input.TriggerDown)
			{
				Close();
			}
			float x = base.transform.InverseTransformDirection(hand.Input.VelAngularWorld).x;
			if (x > 15f)
			{
				Open();
			}
			else if (x < -15f)
			{
				Close();
			}
			if (m_isOpen && !m_isLit && m_hand.OtherHand != null)
			{
				Vector3 velLinearWorld = m_hand.OtherHand.Input.VelLinearWorld;
				float num = Vector3.Distance(m_hand.OtherHand.PalmTransform.position, base.transform.position);
				if (num < 0.2f && Vector3.Angle(velLinearWorld, base.transform.up) > 110f && velLinearWorld.magnitude > 2f)
				{
					Light();
				}
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_isOpen)
			{
				m_lid_curRot = Mathf.Lerp(m_lid_curRot, m_lid_endRot, Time.deltaTime * 9f);
				m_spring_curRot = Mathf.Lerp(m_spring_curRot, m_spring_end_rot, Time.deltaTime * 9f);
			}
			else
			{
				m_lid_curRot = Mathf.Lerp(m_lid_curRot, m_lid_startRot, Time.deltaTime * 15f);
				m_spring_curRot = Mathf.Lerp(m_spring_curRot, m_spring_startRot, Time.deltaTime * 9f);
				if (m_lid_curRot < 1f)
				{
					Flame.gameObject.SetActive(value: false);
				}
			}
			if (m_isLit)
			{
				m_flame_cur = Mathf.Lerp(m_flame_cur, m_flame_max, Time.deltaTime * 2f);
				AlloyLight.Intensity = m_flame_cur * (Mathf.PerlinNoise(Time.time * 10f, AlloyLight.transform.position.y) * 0.05f + 0.5f);
			}
			else
			{
				m_flame_cur = Mathf.Lerp(m_flame_cur, m_flame_min, Time.deltaTime * 7f);
			}
			Lid.localEulerAngles = new Vector3(0f, 0f, m_lid_curRot);
			Spring.localEulerAngles = new Vector3(0f, 0f, m_spring_curRot);
			Flame.localScale = new Vector3(m_flame_cur, m_flame_cur, m_flame_cur);
			Quaternion a = Quaternion.Inverse(base.transform.rotation);
			a = Quaternion.Slerp(a, Random.rotation, Mathf.PerlinNoise(Time.time * 5f, 0f) * 0.3f);
			for (int i = 0; i < FlameJoints.Length; i++)
			{
				Quaternion b = Quaternion.Slerp(Quaternion.identity, a, FlameWeights[i] + Random.Range(-0.05f, 0.05f));
				FlameJoints[i].localScale = new Vector3(Random.Range(0.95f, 1.05f), Random.Range(0.98f, 1.02f), Random.Range(0.95f, 1.05f));
				FlameJoints[i].localRotation = Quaternion.Slerp(FlameJoints[i].localRotation, b, Time.deltaTime * 6f);
			}
			if (!m_isLit)
			{
				return;
			}
			Vector3 position = FlameJoints[0].position;
			Vector3 position2 = FlameJoints[FlameJoints.Length - 1].position;
			Vector3 vector = position2 - position;
			if (Physics.Raycast(position, vector.normalized, out m_hit, vector.magnitude, LM_FireDamage, QueryTriggerInteraction.Collide))
			{
				IFVRDamageable component = m_hit.collider.gameObject.GetComponent<IFVRDamageable>();
				if (component == null && m_hit.collider.attachedRigidbody != null)
				{
					component = m_hit.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
				}
				if (component != null)
				{
					Damage damage = new Damage();
					damage.Class = Damage.DamageClass.Explosive;
					damage.Dam_Thermal = 50f;
					damage.Dam_TotalEnergetic = 50f;
					damage.point = m_hit.point;
					damage.hitNormal = m_hit.normal;
					damage.strikeDir = base.transform.forward;
					component.Damage(damage);
				}
				FVRIgnitable component2 = m_hit.collider.transform.gameObject.GetComponent<FVRIgnitable>();
				if (component2 == null && m_hit.collider.attachedRigidbody != null)
				{
					m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>();
				}
				if (component2 != null)
				{
					FXM.Ignite(component2, 0.1f);
				}
			}
		}

		private void Open()
		{
			if (!m_isOpen)
			{
				m_isOpen = true;
				Audio_Lighter.PlayOneShot(AudioClip_Open, 0.3f);
			}
		}

		private void Close()
		{
			if (m_isOpen)
			{
				m_isLit = false;
				m_isOpen = false;
				Audio_Lighter.PlayOneShot(AudioClip_Close, 0.3f);
			}
		}

		private void Light()
		{
			Sparks.Emit(Random.Range(2, 3));
			if (!m_isLit)
			{
				m_isLit = true;
				Audio_Lighter.PlayOneShot(AudioClip_Strike, 0.3f);
				Flame.gameObject.SetActive(value: true);
			}
		}
	}
}
