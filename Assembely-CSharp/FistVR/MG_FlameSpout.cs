using UnityEngine;

namespace FistVR
{
	public class MG_FlameSpout : MonoBehaviour
	{
		public ParticleSystem PSystem_Ambient;

		public ParticleSystem PSystem_Spout;

		public Transform CastPoint;

		public Transform FlashPoint;

		public LayerMask CastLM;

		private RaycastHit m_hit;

		private bool m_isSpouting;

		private float m_TickDownTilSpout;

		private float m_TickDownTilIdle;

		private float m_DamageTick;

		public AudioEvent AudEvent_Spout;

		private void Awake()
		{
			m_isSpouting = false;
			m_TickDownTilSpout = Random.Range(5f, 20f);
			m_TickDownTilIdle = Random.Range(1.2f, 1.3f);
		}

		private void Update()
		{
			if (m_isSpouting)
			{
				m_TickDownTilIdle -= Time.deltaTime;
				if (m_TickDownTilIdle <= 0f)
				{
					m_TickDownTilIdle = Random.Range(1.2f, 1.3f);
					m_isSpouting = false;
					PSystem_Ambient.gameObject.SetActive(value: true);
					ParticleSystem.EmissionModule emission = PSystem_Spout.emission;
					ParticleSystem.MinMaxCurve rate = emission.rate;
					rate.mode = ParticleSystemCurveMode.Constant;
					rate.constantMax = 0f;
					rate.constantMin = 0f;
					emission.rate = rate;
				}
				if (m_DamageTick >= 0f)
				{
					m_DamageTick -= Time.deltaTime;
					return;
				}
				m_DamageTick = Random.Range(0.05f, 0.2f);
				FXM.InitiateMuzzleFlashLowPriority(FlashPoint.position + Random.onUnitSphere * 0.2f, FlashPoint.forward, Random.Range(0.5f, 3f), new Color(1f, 0.8f, 0.6f), Random.Range(0.5f, 1.5f));
				if (Physics.Raycast(CastPoint.position, CastPoint.forward, out m_hit, 2f, CastLM, QueryTriggerInteraction.Collide) && m_hit.collider.attachedRigidbody != null)
				{
					FVRPlayerHitbox component = m_hit.collider.attachedRigidbody.gameObject.GetComponent<FVRPlayerHitbox>();
					if (component != null)
					{
						DamageDealt dam = default(DamageDealt);
						dam.force = Vector3.zero;
						dam.hitNormal = Vector3.zero;
						dam.IsInside = false;
						dam.MPa = 1f;
						dam.MPaRootMeter = 1f;
						dam.point = base.transform.position;
						dam.PointsDamage = 250f;
						dam.ShotOrigin = null;
						dam.strikeDir = Vector3.zero;
						dam.uvCoords = Vector2.zero;
						dam.IsInitialContact = true;
						component.Damage(dam);
					}
				}
				return;
			}
			m_TickDownTilSpout -= Time.deltaTime;
			if (m_TickDownTilSpout <= 0f)
			{
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
				if (num < 15f)
				{
					SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, AudEvent_Spout, base.transform.position, num / 343f);
				}
				m_TickDownTilSpout = Random.Range(8f, 25f);
				m_isSpouting = true;
				PSystem_Ambient.gameObject.SetActive(value: false);
				ParticleSystem.EmissionModule emission2 = PSystem_Spout.emission;
				ParticleSystem.MinMaxCurve rate2 = emission2.rate;
				rate2.mode = ParticleSystemCurveMode.Constant;
				rate2.constantMax = 20f;
				rate2.constantMin = 20f;
				emission2.rate = rate2;
			}
		}
	}
}
