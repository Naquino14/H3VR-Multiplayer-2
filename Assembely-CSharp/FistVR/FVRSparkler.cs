using UnityEngine;

namespace FistVR
{
	public class FVRSparkler : FVRPhysicalObject
	{
		[Header("Match Config")]
		public Renderer MatchRenderer;

		public Transform Point_Top;

		public Transform Point_Bottom;

		private bool m_hasBeenLit;

		private bool m_isBurning;

		private float m_burntState;

		private float m_burnSpeed = 0.05f;

		private Collider m_curStrikeCol;

		private int m_strikeFrames;

		public GameObject FireGO;

		public ParticleSystem Fire1;

		public ParticleSystem Fire2;

		public ParticleSystem Fire3;

		public ParticleSystem Fire4;

		public ParticleSystem Fire5;

		private float m_currentBurnPoint;

		private float m_currentBreakoff;

		private bool m_isTickingDownToDeath;

		private float m_deathTimer = 5f;

		private AudioSource fireAud;

		protected override void Awake()
		{
			base.Awake();
			fireAud = FireGO.GetComponent<AudioSource>();
		}

		protected override void FVRUpdate()
		{
			if (m_isTickingDownToDeath && !base.IsHeld)
			{
				m_deathTimer -= Time.deltaTime;
				if (m_deathTimer <= 0f)
				{
					Object.Destroy(base.gameObject);
				}
			}
			if (m_isTickingDownToDeath)
			{
				fireAud.volume = Mathf.Lerp(fireAud.volume, 0f, Time.deltaTime * 2f);
			}
			if (m_isBurning)
			{
				m_burntState += Time.deltaTime * m_burnSpeed;
				if (m_burntState >= 1f)
				{
					m_burntState = 1f;
					m_isBurning = false;
					ParticleSystem.EmissionModule emission = Fire1.emission;
					emission.enabled = false;
					ParticleSystem.EmissionModule emission2 = Fire2.emission;
					emission2.enabled = false;
					ParticleSystem.EmissionModule emission3 = Fire3.emission;
					emission3.enabled = false;
					ParticleSystem.EmissionModule emission4 = Fire4.emission;
					emission4.enabled = false;
					ParticleSystem.EmissionModule emission5 = Fire5.emission;
					emission5.enabled = false;
					m_isTickingDownToDeath = true;
					FireGO.layer = LayerMask.NameToLayer("NoCol");
				}
				m_currentBurnPoint = Mathf.Lerp(0.12f, 0.78f, m_burntState);
				MatchRenderer.material.SetFloat("_TransitionCutoff", m_currentBurnPoint);
				m_currentBreakoff = Mathf.Max(m_currentBreakoff, m_currentBurnPoint - 0.15f);
				MatchRenderer.material.SetFloat("_DissolveCutoff", m_currentBreakoff);
				FireGO.transform.position = Vector3.Lerp(Point_Top.position, Point_Bottom.position, m_burntState);
			}
		}

		public void Ignite()
		{
			if (!m_hasBeenLit)
			{
				m_hasBeenLit = true;
				m_isBurning = true;
				FireGO.SetActive(value: true);
				GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
				m_strikeFrames = 0;
			}
		}

		private new void OnCollisionEnter(Collision col)
		{
			if (m_hasBeenLit && m_isBurning && col.collider.attachedRigidbody != null)
			{
				IFVRDamageable component = col.collider.transform.gameObject.GetComponent<IFVRDamageable>();
				if (component == null)
				{
					component = col.collider.attachedRigidbody.gameObject.GetComponent<IFVRDamageable>();
				}
				if (component != null)
				{
					Damage damage = new Damage();
					damage.Dam_Thermal = 50f;
					damage.Dam_TotalEnergetic = 50f;
					damage.point = col.contacts[0].point;
					damage.hitNormal = col.contacts[0].normal;
					damage.strikeDir = base.transform.forward;
					component.Damage(damage);
				}
			}
		}
	}
}
