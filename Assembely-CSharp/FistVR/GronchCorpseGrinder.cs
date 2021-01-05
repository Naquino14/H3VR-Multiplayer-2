using UnityEngine;

namespace FistVR
{
	public class GronchCorpseGrinder : MonoBehaviour
	{
		private bool m_isJobStarted;

		private GronchJobManager m_M;

		public ParticleSystem PSystem_Sparks;

		public ParticleSystem PSystem_Sauce;

		public Transform[] Rollers;

		private float m_roll;

		public AudioSource Aud;

		public AudioClip Aud_Grind;

		public AudioSource Aud_GrindLoop;

		private float GrindTick = 0.1f;

		public void BeginJob(GronchJobManager m)
		{
			m_M = m;
			Aud_GrindLoop.Play();
		}

		public void EndJob(GronchJobManager m)
		{
			m_M = null;
			Aud_GrindLoop.Stop();
		}

		private void Update()
		{
			m_roll += Time.deltaTime * 1720f;
			m_roll = Mathf.Repeat(m_roll, 360f);
			Rollers[0].localEulerAngles = new Vector3(0f, 0f, 0f - m_roll);
			Rollers[1].localEulerAngles = new Vector3(0f, 0f, m_roll);
			if (GrindTick > 0f)
			{
				GrindTick -= Time.deltaTime;
			}
		}

		private void OnTriggerEnter(Collider col)
		{
			CheckCol(col);
		}

		private void CheckCol(Collider col)
		{
			if (col.attachedRigidbody == null)
			{
				return;
			}
			SosigLink component = col.attachedRigidbody.gameObject.GetComponent<SosigLink>();
			if (!(component == null) && !(component.O.QuickbeltSlot != null) && !component.O.m_isHardnessed)
			{
				if (GrindTick <= 0f && Aud_Grind != null)
				{
					GrindTick = Random.Range(0.2f, 0.5f);
					PSystem_Sparks.Emit(50);
					Aud.pitch = Random.Range(0.85f, 1.05f);
					Aud.PlayOneShot(Aud_Grind, 0.5f);
				}
				Damage damage = new Damage();
				damage.hitNormal = Vector3.up;
				damage.Class = Damage.DamageClass.Environment;
				damage.damageSize = 0.1f;
				damage.Dam_Cutting = 100000f;
				damage.Dam_Piercing = 100000f;
				damage.Dam_Blunt = 100000f;
				damage.point = component.transform.position;
				damage.strikeDir = Vector3.up;
				component.Damage(damage);
				m_M.DidJobStuff();
			}
		}
	}
}
