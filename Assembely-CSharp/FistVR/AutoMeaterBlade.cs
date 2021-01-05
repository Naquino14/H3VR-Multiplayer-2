using UnityEngine;

namespace FistVR
{
	public class AutoMeaterBlade : MonoBehaviour
	{
		public AutoMeater M;

		public AudioEvent AudEvent_Hit;

		public HingeJoint Joint;

		public ParticleSystem PSystem_Sparks;

		private ParticleSystem.EmitParams emitParams;

		private bool m_isShutDown;

		private float m_timeSinceDamage = 1f;

		private void Update()
		{
			if (m_timeSinceDamage < 1f)
			{
				m_timeSinceDamage += Time.deltaTime;
			}
		}

		public void ShutDown()
		{
			if (!m_isShutDown)
			{
				m_isShutDown = true;
				Joint.useMotor = false;
			}
		}

		public void Reactivate()
		{
			if (m_isShutDown)
			{
				m_isShutDown = false;
				Joint.useMotor = true;
			}
		}

		private void OnCollisionEnter(Collision col)
		{
			if (m_isShutDown || m_timeSinceDamage < 0.3f)
			{
				return;
			}
			m_timeSinceDamage = Random.Range(0f, 0.1f);
			SM.PlayCoreSound(FVRPooledAudioType.NPCBarks, AudEvent_Hit, col.contacts[0].point);
			int num = 0;
			for (int i = 0; i < col.contacts.Length; i++)
			{
				if (num < 3)
				{
					num++;
					emitParams.position = col.contacts[i].point;
					Vector3 normalized = (col.contacts[i].point - base.transform.position).normalized;
					normalized = Vector3.Cross(normalized, base.transform.up) * 30f;
					normalized += Random.onUnitSphere * 3f;
					emitParams.velocity = normalized;
					PSystem_Sparks.Emit(emitParams, 1);
					emitParams.velocity = normalized * 0.2f + Random.onUnitSphere * 15f;
					PSystem_Sparks.Emit(emitParams, 1);
				}
			}
			bool flag = false;
			if (col.collider.attachedRigidbody != null)
			{
				flag = true;
				float magnitude = col.relativeVelocity.magnitude;
				bool flag2 = false;
				IFVRDamageable iFVRDamageable = null;
				Damage damage = new Damage();
				IFVRDamageable component = col.contacts[0].otherCollider.transform.gameObject.GetComponent<IFVRDamageable>();
				if (component == null && col.contacts[0].otherCollider.attachedRigidbody != null)
				{
					component = col.contacts[0].otherCollider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
				}
				if (component != null)
				{
					iFVRDamageable = component;
					flag2 = true;
					damage.Class = Damage.DamageClass.Melee;
					damage.point = col.contacts[0].point;
					damage.hitNormal = col.contacts[0].normal;
					damage.strikeDir = M.RB.GetPointVelocity(col.contacts[0].point).normalized;
					damage.damageSize = 0.02f;
					damage.Source_IFF = M.E.IFFCode;
					damage.edgeNormal = base.transform.forward;
					damage.Dam_Blunt = 50f;
					damage.Dam_Cutting = 400f;
					damage.Dam_Piercing = 50f;
					damage.Dam_TotalKinetic = damage.Dam_Blunt + damage.Dam_Cutting + damage.Dam_Piercing;
					iFVRDamageable.Damage(damage);
				}
			}
		}
	}
}
