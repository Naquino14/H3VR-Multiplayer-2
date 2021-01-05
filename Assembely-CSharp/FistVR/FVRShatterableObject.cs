using UnityEngine;

namespace FistVR
{
	public class FVRShatterableObject : MonoBehaviour, IFVRDamageable
	{
		public int NumShotsToShatter = 1;

		public Rigidbody[] Shards;

		public AudioEvent AudEvent_Destruction;

		public ParticleSystem[] DestructionPSystems;

		public int[] DestructionPSystemEmits;

		protected Rigidbody m_rb;

		private AudioSource m_aud;

		private Renderer m_rend;

		private Collider m_col;

		private bool m_isDestroyed;

		public float explosionMultiplier = 1f;

		public GameObject OnDieTarget;

		public string OnDieMessage;

		public bool TakesColDamage;

		public virtual void Awake()
		{
			m_rb = GetComponent<Rigidbody>();
			m_aud = GetComponent<AudioSource>();
			m_rend = GetComponent<Renderer>();
			m_col = GetComponent<Collider>();
			for (int i = 0; i < Shards.Length; i++)
			{
				Shards[i].maxAngularVelocity = 30f;
			}
		}

		public void OnCollisionEnter(Collision col)
		{
			if (!(col.collider.attachedRigidbody != null) || !TakesColDamage)
			{
				return;
			}
			float magnitude = col.relativeVelocity.magnitude;
			if (!(magnitude > 2.5f))
			{
				return;
			}
			int num = (int)(magnitude * col.collider.attachedRigidbody.mass / m_rb.mass);
			if (num > 0)
			{
				NumShotsToShatter -= num;
				if (NumShotsToShatter <= 0)
				{
					Destroy(col.contacts[0].point, col.relativeVelocity);
				}
			}
		}

		public void Damage(Damage dam)
		{
			float dam_TotalKinetic = dam.Dam_TotalKinetic;
			if (dam.Class == FistVR.Damage.DamageClass.Explosive)
			{
				dam_TotalKinetic *= 0.005f;
			}
			else if (dam.Class == FistVR.Damage.DamageClass.Projectile)
			{
				dam_TotalKinetic *= 0.01f;
			}
			else if (dam.Class == FistVR.Damage.DamageClass.Melee)
			{
				dam_TotalKinetic *= 0.005f;
			}
			NumShotsToShatter -= (int)dam.Dam_TotalKinetic;
			if (NumShotsToShatter <= 0)
			{
				Destroy(dam.point, dam.strikeDir * dam.Dam_TotalKinetic * 0.01f);
			}
			else if (m_rb != null)
			{
				m_rb.AddForceAtPosition(dam.strikeDir * dam.Dam_TotalKinetic * 0.01f, dam.point);
			}
		}

		protected void GoNonKinematic()
		{
			if (m_rb != null)
			{
				m_rb.isKinematic = false;
				m_rb.useGravity = true;
			}
		}

		protected void GoNonKinematic(Vector3 point, Vector3 force)
		{
			if (m_rb != null)
			{
				m_rb.isKinematic = false;
				m_rb.useGravity = true;
				m_rb.AddForceAtPosition(force * explosionMultiplier, point, ForceMode.Impulse);
			}
		}

		public virtual void Destroy(Vector3 damagePoint, Vector3 damageDir)
		{
			if (m_isDestroyed)
			{
				return;
			}
			m_isDestroyed = true;
			if (OnDieTarget != null)
			{
				OnDieTarget.SendMessage(OnDieMessage);
			}
			m_rend.enabled = false;
			m_col.enabled = false;
			for (int i = 0; i < DestructionPSystems.Length; i++)
			{
				DestructionPSystems[i].gameObject.SetActive(value: true);
				DestructionPSystems[i].transform.SetParent(null);
				DestructionPSystems[i].Emit(DestructionPSystemEmits[i]);
			}
			for (int j = 0; j < Shards.Length; j++)
			{
				Shards[j].gameObject.SetActive(value: true);
				if (base.transform.parent != null)
				{
					Shards[j].transform.SetParent(base.transform.parent);
				}
				else
				{
					Shards[j].transform.SetParent(null);
				}
				Shards[j].AddForceAtPosition(explosionMultiplier * damageDir * (1f / (float)Shards.Length), damagePoint, ForceMode.Impulse);
			}
			if (AudEvent_Destruction.Clips.Count > 0)
			{
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
				float delay = num / 343f;
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.GenericLongRange, AudEvent_Destruction, base.transform.position, delay);
			}
		}

		private void Update()
		{
			if (m_isDestroyed)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
