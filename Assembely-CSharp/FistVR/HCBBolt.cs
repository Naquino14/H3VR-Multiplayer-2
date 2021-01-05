using System;
using UnityEngine;

namespace FistVR
{
	public class HCBBolt : MonoBehaviour
	{
		public enum HBCBoltType
		{
			Impaling,
			Explosive
		}

		public HBCBoltType BoltType;

		private bool m_isFlying;

		[Header("Ballistics")]
		public float BaseSpeed = 40f;

		public float GravityMultiplier = 1f;

		public float AirDragMultiplier = 1f;

		public Vector3 Dimensions;

		public float DragCoefficient;

		public float Mass;

		private Vector3 m_velocity = Vector3.zero;

		private Vector3 m_forward = Vector3.forward;

		private Vector3 m_lastPoint = Vector3.zero;

		private float m_gravMag = 9.81f;

		[Header("Raycasting")]
		public LayerMask LM_Hit;

		public LayerMask LM_Env;

		public LayerMask LM_Agent;

		private RaycastHit m_hit;

		private RaycastHit m_hit2;

		public float PinDistanceLimit = 1f;

		public Vector2 LinkPenetrationRange;

		private bool m_isTickingDownToDestroy;

		private float m_tickDownToDestroy = 30f;

		public Renderer Rend;

		[Header("VFX")]
		public GameObject MeatSplosion;

		[Header("Sound")]
		public AudioEvent AudEvent_Hit_Skewer;

		public AudioEvent AudEvent_Hit_Meat;

		public AudioEvent AudEvent_Hit_Solid;

		public AudioEvent AudEvent_Hit_Solid_Light;

		private int m_numBounces;

		private float m_cookedAmount;

		public void Fire(Vector3 dir, Vector3 initPos, float velMult)
		{
			switch (GM.Options.SimulationOptions.BallisticGravityMode)
			{
			case SimulationOptions.GravityMode.Realistic:
				m_gravMag = 9.81f;
				break;
			case SimulationOptions.GravityMode.Playful:
				m_gravMag = 5f;
				break;
			case SimulationOptions.GravityMode.OnTheMoon:
				m_gravMag = 1.622f;
				break;
			case SimulationOptions.GravityMode.None:
				m_gravMag = 0f;
				break;
			}
			m_velocity = dir.normalized * BaseSpeed * velMult;
			m_forward = dir;
			m_isFlying = true;
		}

		public void SetCookedAmount(float f)
		{
			Rend.material.SetFloat("_BlendScale", f);
			m_cookedAmount = f;
		}

		private void Update()
		{
			if (m_isTickingDownToDestroy)
			{
				m_tickDownToDestroy -= Time.deltaTime;
				if (m_tickDownToDestroy <= 0f)
				{
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
			if (!m_isFlying)
			{
				return;
			}
			float deltaTime = Time.deltaTime;
			m_velocity += Vector3.down * m_gravMag * deltaTime * GravityMultiplier;
			float materialDensity = 1.225f * AirDragMultiplier;
			m_velocity = ApplyDrag(m_velocity, materialDensity, deltaTime);
			Vector3 position = base.transform.position;
			Vector3 normalized = m_velocity.normalized;
			float magnitude = m_velocity.magnitude;
			float maxDistance = magnitude * deltaTime;
			if (!Physics.Raycast(position, normalized, out m_hit, maxDistance, LM_Hit, QueryTriggerInteraction.Collide))
			{
				base.transform.position = position + m_velocity * deltaTime;
				base.transform.rotation = Quaternion.LookRotation(normalized);
				return;
			}
			m_isFlying = false;
			Rigidbody attachedRigidbody = m_hit.collider.attachedRigidbody;
			if (m_cookedAmount < 0.4f)
			{
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, AudEvent_Hit_Meat, base.transform.position, Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
				UnityEngine.Object.Instantiate(MeatSplosion, m_hit.point, Quaternion.identity);
				UnityEngine.Object.Destroy(base.gameObject);
			}
			else if (attachedRigidbody == null)
			{
				FXM.SpawnImpactEffect(m_hit.point, m_hit.normal, 1, ImpactEffectMagnitude.Medium, forwardBack: false);
				float num = Vector3.Angle(-normalized, m_hit.normal);
				if (num > 60f && m_numBounces < 4)
				{
					m_numBounces++;
					m_velocity = Vector3.Reflect(normalized, m_hit.normal).normalized * (magnitude * 0.8f);
					base.transform.rotation = Quaternion.LookRotation(m_velocity);
					base.transform.position = m_hit.point + m_hit.normal * 0.001f;
					SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, AudEvent_Hit_Solid_Light, base.transform.position, Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
				}
				else
				{
					base.transform.rotation = Quaternion.LookRotation(normalized);
					StickIntoWall(m_hit.point);
					SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, AudEvent_Hit_Solid, base.transform.position, Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
				}
			}
			else
			{
				SosigLink component = m_hit.collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
				if (component == null)
				{
					DamageOtherThing(m_hit.collider, attachedRigidbody, m_hit.point - m_hit.normal * 0.005f, normalized, passOn: true);
					Explode(m_hit.point);
				}
				else
				{
					DamageOtherThing(m_hit.collider, attachedRigidbody, m_hit.point - m_hit.normal * 0.005f, normalized, passOn: true);
					ImpaleCheck(component, m_hit.point - m_hit.normal * 0.005f, normalized);
				}
			}
		}

		private Vector3 ApplyDrag(Vector3 velocity, float materialDensity, float time)
		{
			float num = (float)Math.PI * Mathf.Pow(Dimensions.x * 0.5f, 2f);
			float magnitude = velocity.magnitude;
			Vector3 normalized = velocity.normalized;
			float dragCoefficient = DragCoefficient;
			return normalized * Mathf.Clamp(magnitude - (-velocity * (materialDensity * 0.5f * dragCoefficient * num / Mass) * magnitude).magnitude * time, 0f, magnitude);
		}

		private void Explode(Vector3 point)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}

		private void StickIntoWall(Vector3 point)
		{
			base.transform.position = point + base.transform.forward * UnityEngine.Random.Range(LinkPenetrationRange.x, LinkPenetrationRange.y);
			m_isTickingDownToDestroy = true;
		}

		private void DamageOtherThing(Collider c, Rigidbody r, Vector3 hitPoint, Vector3 castDir, bool passOn)
		{
			IFVRDamageable component = c.transform.gameObject.GetComponent<IFVRDamageable>();
			if (component == null && passOn)
			{
				component = r.gameObject.GetComponent<IFVRDamageable>();
			}
			if (component != null)
			{
				Damage damage = new Damage();
				damage.Class = Damage.DamageClass.Projectile;
				damage.damageSize = Dimensions.x;
				damage.hitNormal = -castDir;
				damage.Source_IFF = GM.CurrentPlayerBody.GetPlayerIFF();
				damage.strikeDir = castDir;
				damage.point = hitPoint;
				damage.Dam_Piercing = 3000f;
				damage.Dam_TotalKinetic = 3000f;
				damage.point = hitPoint;
				damage.Dam_Stunning = 2f;
				component.Damage(damage);
			}
		}

		private void ImpaleCheck(SosigLink l, Vector3 hitPoint, Vector3 castDir)
		{
			float maxDistance = Dimensions.z + PinDistanceLimit;
			Damage damage = new Damage();
			damage.Class = Damage.DamageClass.Projectile;
			damage.damageSize = Dimensions.x;
			damage.hitNormal = -castDir;
			damage.Source_IFF = GM.CurrentPlayerBody.GetPlayerIFF();
			damage.strikeDir = castDir;
			damage.Dam_Piercing = 500f;
			damage.Dam_TotalKinetic = 500f;
			damage.point = hitPoint;
			damage.Dam_Stunning = 2f;
			Vector3 point = l.transform.position + l.transform.up * 0.15f;
			Vector3 point2 = l.transform.position - l.transform.up * 0.15f;
			bool flag = false;
			bool flag2 = false;
			if (Physics.CapsuleCast(point, point2, 0.13f, castDir, out m_hit, maxDistance, LM_Env, QueryTriggerInteraction.Ignore))
			{
				if (Physics.Raycast(hitPoint, castDir, out m_hit2, m_hit.distance + Dimensions.z, LM_Env, QueryTriggerInteraction.Collide) && l.gameObject.GetComponent<FixedJoint>() == null)
				{
					l.transform.position = l.transform.position + m_hit.distance * castDir;
					base.transform.position = m_hit2.point + castDir * 0.05f;
					FixedJoint fixedJoint = l.gameObject.AddComponent<FixedJoint>();
					l.S.KillSosig();
					base.transform.SetParent(l.transform);
					damage.point += m_hit.distance * castDir;
					flag = true;
					l.Damage(damage);
				}
			}
			else if (Physics.CapsuleCast(point, point2, 0.13f, castDir, out m_hit, maxDistance, LM_Agent, QueryTriggerInteraction.Ignore) && l.gameObject.GetComponent<FixedJoint>() == null)
			{
				Ray ray = new Ray(hitPoint, castDir);
				RaycastHit[] array = Physics.RaycastAll(ray, m_hit.distance + Dimensions.z, LM_Agent, QueryTriggerInteraction.Collide);
				for (int i = 0; i < array.Length; i++)
				{
					if (!(array[i].collider.attachedRigidbody == null))
					{
						SosigLink component = array[i].collider.attachedRigidbody.gameObject.GetComponent<SosigLink>();
						if (!(component == l))
						{
							base.transform.position = hitPoint + castDir * UnityEngine.Random.Range(LinkPenetrationRange.y * 0.8f, LinkPenetrationRange.y);
							base.transform.SetParent(l.transform);
							l.transform.position = l.transform.position + m_hit.distance * castDir;
							FixedJoint fixedJoint2 = l.gameObject.AddComponent<FixedJoint>();
							fixedJoint2.connectedBody = component.R;
							l.S.KillSosig();
							component.S.KillSosig();
							damage.point += array[i].distance * castDir;
							flag2 = true;
							l.Damage(damage);
							component.Damage(damage);
							break;
						}
					}
				}
			}
			if (!flag && !flag2)
			{
				base.transform.position = hitPoint + castDir * UnityEngine.Random.Range(LinkPenetrationRange.x, LinkPenetrationRange.y);
				base.transform.SetParent(l.transform);
				l.Damage(damage);
				if (l.S.BodyState != Sosig.SosigBodyState.Dead)
				{
					l.S.KillSosig();
				}
				l.R.AddForceAtPosition(castDir * 30f, hitPoint, ForceMode.Impulse);
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, AudEvent_Hit_Meat, base.transform.position, Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
			}
			else
			{
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, AudEvent_Hit_Skewer, base.transform.position, Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
			}
		}
	}
}
