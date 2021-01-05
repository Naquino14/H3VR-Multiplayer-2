using UnityEngine;

namespace FistVR
{
	public class MG_SmashyStool : MonoBehaviour
	{
		public Transform TopStool;

		public LayerMask PlayerLM;

		private RaycastHit m_hit;

		public AudioEvent AudEvent_Smash;

		private float m_tick = 1f;

		private float m_MaxTick = 10f;

		private Vector3 retractedPos = new Vector3(0f, 2.1f, 0f);

		private bool m_isRetracting;

		public ParticleSystem Sparks;

		private bool m_isPlayingSound;

		private void Awake()
		{
			m_tick = Random.Range(8f, 20f);
			base.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
		}

		private void Update()
		{
			m_tick -= Time.deltaTime;
			if (m_tick > 0.001f && m_tick < 0.7f && !m_isPlayingSound)
			{
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
				if (num < 15f)
				{
					m_isPlayingSound = true;
					SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Smash, base.transform.position);
				}
			}
			if (m_tick <= 0f)
			{
				m_isPlayingSound = false;
				m_tick = Random.Range(8f, 20f);
				TopStool.transform.localPosition = Vector3.zero;
				Sparks.Emit(Random.Range(15, 30));
				FXM.InitiateMuzzleFlashLowPriority(base.transform.position, Vector3.up, Random.Range(0.5f, 2f), Color.white, Random.Range(0.25f, 0.6f));
				if (Physics.Raycast(TopStool.position, -Vector3.down, out m_hit, 2.1f, PlayerLM, QueryTriggerInteraction.Collide) && m_hit.collider.attachedRigidbody != null)
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
						dam.PointsDamage = 1000f;
						dam.ShotOrigin = null;
						dam.strikeDir = Vector3.zero;
						dam.uvCoords = Vector2.zero;
						dam.IsInitialContact = true;
						component.Damage(dam);
					}
				}
				Invoke("StartRetracting", 1f);
			}
			if (m_isRetracting)
			{
				float y = TopStool.transform.localPosition.y;
				y += Time.deltaTime;
				if (y >= retractedPos.y)
				{
					y = retractedPos.y;
					m_isRetracting = false;
				}
				TopStool.transform.localPosition = new Vector3(0f, y, 0f);
			}
		}

		private void StartRetracting()
		{
			m_isRetracting = true;
		}
	}
}
