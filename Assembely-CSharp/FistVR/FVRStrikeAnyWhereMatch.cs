using UnityEngine;

namespace FistVR
{
	public class FVRStrikeAnyWhereMatch : FVRPhysicalObject
	{
		[Header("Match Config")]
		public Transform[] BendingJoints;

		public Renderer MatchRenderer;

		public Collider MatchHeadCol;

		private bool m_hasBeenLit;

		private bool m_isBurning;

		private float m_burntState;

		private float m_burnSpeed = 0.05f;

		private Collider m_curStrikeCol;

		private int m_strikeFrames;

		private bool[] m_hasStartedBending = new bool[4];

		private Vector3 m_randBend = Vector3.zero;

		public GameObject FireGO;

		public ParticleSystem Fire1;

		public ParticleSystem Fire2;

		private float m_currentBurnPoint;

		private float m_currentBreakoff;

		private bool m_isTickingDownToDeath;

		private float m_deathTimer = 5f;

		public Color c1;

		public Color c2;

		private int m_matchTick = 4;

		protected override void FVRUpdate()
		{
			if (m_isTickingDownToDeath)
			{
				m_deathTimer -= Time.deltaTime;
				if (m_deathTimer <= 0f)
				{
					Object.Destroy(base.gameObject);
				}
			}
			if (!m_isBurning)
			{
				return;
			}
			m_matchTick--;
			if (m_matchTick <= 0)
			{
				m_matchTick = Random.Range(2, 4);
				FXM.InitiateMuzzleFlash(FireGO.transform.position, base.transform.up, Random.Range(0.2f, 0.4f), Color.Lerp(c1, c2, Random.Range(0f, 1f)), Random.Range(0.9f, 1f));
			}
			m_burntState += Time.deltaTime * m_burnSpeed;
			if (m_burntState >= 1f)
			{
				m_burntState = 1f;
				m_isBurning = false;
				ParticleSystem.EmissionModule emission = Fire1.emission;
				emission.enabled = false;
				ParticleSystem.EmissionModule emission2 = Fire2.emission;
				emission2.enabled = false;
				if (FireGO.activeSelf)
				{
					FireGO.SetActive(value: false);
				}
				m_isTickingDownToDeath = true;
			}
			m_currentBurnPoint = Mathf.Lerp(0.12f, 0.78f, m_burntState);
			MatchRenderer.material.SetFloat("_TransitionCutoff", m_currentBurnPoint);
			if (m_burntState >= 0.2f && m_burntState < 0.4f)
			{
				FireGO.transform.position = Vector3.Lerp(BendingJoints[0].position, BendingJoints[1].position, (m_burntState - 0.2f) * 5f);
				if (!m_hasStartedBending[0])
				{
					m_hasStartedBending[0] = true;
					m_randBend = new Vector3(Random.Range(-55f, 55f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
				}
				BendingJoints[1].localEulerAngles = Vector3.Lerp(Vector3.zero, m_randBend, (m_burntState - 0.2f) * 5f);
			}
			else if (m_burntState >= 0.4f && m_burntState < 0.6f)
			{
				FireGO.transform.position = Vector3.Lerp(BendingJoints[1].position, BendingJoints[2].position, (m_burntState - 0.4f) * 5f);
				if (!m_hasStartedBending[1])
				{
					m_hasStartedBending[1] = true;
					m_randBend = new Vector3(Random.Range(-55f, 55f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
				}
				BendingJoints[2].localEulerAngles = Vector3.Lerp(Vector3.zero, m_randBend, (m_burntState - 0.4f) * 5f);
			}
			else if (m_burntState >= 0.6f && m_burntState < 0.8f)
			{
				FireGO.transform.position = Vector3.Lerp(BendingJoints[2].position, BendingJoints[3].position, (m_burntState - 0.6f) * 5f);
				if (!m_hasStartedBending[2])
				{
					m_hasStartedBending[2] = true;
					m_randBend = new Vector3(Random.Range(-55f, 55f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
				}
				BendingJoints[3].localEulerAngles = Vector3.Lerp(Vector3.zero, m_randBend, (m_burntState - 0.6f) * 5f);
			}
			else if (m_burntState >= 0.8f)
			{
				FireGO.transform.position = Vector3.Lerp(BendingJoints[3].position, BendingJoints[4].position, (m_burntState - 0.8f) * 5f);
				if (!m_hasStartedBending[3])
				{
					m_hasStartedBending[3] = true;
					m_randBend = new Vector3(Random.Range(-15f, 15f), Random.Range(-25f, 25f), Random.Range(-25f, 25f));
				}
				BendingJoints[4].localEulerAngles = Vector3.Lerp(Vector3.zero, m_randBend, (m_burntState - 0.8f) * 5f);
			}
		}

		public void Ignite()
		{
			if (!m_hasBeenLit)
			{
				m_hasBeenLit = true;
				m_isBurning = true;
				FireGO.SetActive(value: true);
				GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip, 0.4f);
				m_strikeFrames = 0;
			}
		}

		private new void OnCollisionEnter(Collision col)
		{
			if (!m_hasBeenLit)
			{
				return;
			}
			m_currentBreakoff = Mathf.Max(m_currentBreakoff, m_currentBurnPoint - 0.15f);
			MatchRenderer.material.SetFloat("_DissolveCutoff", m_currentBreakoff);
			if (m_isBurning && col.collider.attachedRigidbody != null)
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
				FVRIgnitable component2 = col.collider.transform.gameObject.GetComponent<FVRIgnitable>();
				if (component2 == null && col.collider.attachedRigidbody != null)
				{
					col.collider.attachedRigidbody.gameObject.GetComponent<FVRIgnitable>();
				}
				if (component2 != null)
				{
					FXM.Ignite(component2, 0.1f);
				}
			}
		}

		private void OnCollisionStay(Collision col)
		{
			if (base.IsHeld && !m_hasBeenLit && col.contacts[0].thisCollider == MatchHeadCol)
			{
				if (col.collider != m_curStrikeCol)
				{
					m_curStrikeCol = col.collider;
				}
				Vector3 normal = col.contacts[0].normal;
				float num = Vector3.Angle(normal, col.relativeVelocity);
				if (num > 45f && col.relativeVelocity.magnitude > 1f)
				{
					m_strikeFrames++;
				}
				if (m_strikeFrames >= 10)
				{
					m_hasBeenLit = true;
					m_isBurning = true;
					FireGO.SetActive(value: true);
					GetComponent<AudioSource>().PlayOneShot(GetComponent<AudioSource>().clip);
					m_strikeFrames = 0;
				}
			}
		}

		private void OnCollisionExit(Collision col)
		{
			if (!m_hasBeenLit)
			{
				m_curStrikeCol = null;
				m_strikeFrames = 0;
			}
		}
	}
}
