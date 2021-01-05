using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class AR3_Lemon : MonoBehaviour
	{
		public float InitialVel = 18.8f;

		public LayerMask LM_Vaporize;

		public LayerMask LM_Bounce;

		private Vector3 m_dir;

		private float m_vel;

		private RaycastHit m_hit;

		public int IFF;

		private float m_lifeLeft = 4f;

		public List<GameObject> SplodeOnSpawn;

		public Transform InnerSphere;

		public Transform Lemon;

		public Transform ParticleTrail;

		public AudioEvent AudEvent_Bounce;

		public AudioEvent AudEvent_NearMiss;

		private float m_lastDistanceToHead;

		private float m_timeSinceNearMiss;

		private bool m_isExploded;

		public void Start()
		{
			m_dir = base.transform.forward;
			m_vel = InitialVel;
			IFF = GM.CurrentPlayerBody.GetPlayerIFF();
			Lemon.rotation = Random.rotation;
			FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_NearMiss, base.transform.position);
			fVRPooledAudioSource.FollowThisTransform(base.transform);
			m_timeSinceNearMiss = 0f;
			m_lastDistanceToHead = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
		}

		private void Update()
		{
			if (m_timeSinceNearMiss < 10f)
			{
				m_timeSinceNearMiss += Time.deltaTime;
			}
			float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
			if (m_timeSinceNearMiss > 0.3f && num < 3f && num < m_lastDistanceToHead)
			{
				FVRPooledAudioSource fVRPooledAudioSource = SM.PlayCoreSound(FVRPooledAudioType.GenericClose, AudEvent_NearMiss, base.transform.position);
				fVRPooledAudioSource.FollowThisTransform(base.transform);
				m_timeSinceNearMiss = 0f;
			}
			m_lastDistanceToHead = num;
			Vector3 position = base.transform.position;
			Vector3 forward = base.transform.forward;
			float num2 = m_vel * Time.deltaTime;
			Vector3 position2 = base.transform.position + forward * num2;
			if (Physics.Raycast(position, forward, out m_hit, num2, LM_Bounce, QueryTriggerInteraction.Ignore))
			{
				position2 = m_hit.point + m_hit.normal * 0.002f;
				m_dir = Vector3.Reflect(m_dir, m_hit.normal);
				if (num < 30f)
				{
					SM.PlayCoreSound(FVRPooledAudioType.Impacts, AudEvent_Bounce, base.transform.position);
				}
				num2 = m_hit.distance;
				FXM.SpawnImpactEffect(m_hit.point, m_hit.normal, 1, ImpactEffectMagnitude.Large, forwardBack: false);
				FXM.InitiateMuzzleFlashLowPriority(m_hit.point, m_hit.normal, 1.5f, Color.white, 2.5f);
				base.transform.rotation = Quaternion.LookRotation(m_dir);
			}
			RaycastHit[] array = Physics.SphereCastAll(position, 0.1f, forward, num2, LM_Vaporize);
			for (int i = 0; i < array.Length; i++)
			{
				RaycastHit raycastHit = array[i];
				if (raycastHit.collider.attachedRigidbody != null)
				{
					raycastHit.collider.attachedRigidbody.gameObject.GetComponent<IVaporizable>()?.Vaporize(IFF);
				}
			}
			base.transform.position = position2;
			InnerSphere.LookAt(GM.CurrentPlayerBody.Head.position);
			m_lifeLeft -= Time.deltaTime;
			if (m_lifeLeft < 0f)
			{
				Explode();
			}
		}

		private void Explode()
		{
			if (!m_isExploded)
			{
				m_isExploded = true;
				for (int i = 0; i < SplodeOnSpawn.Count; i++)
				{
					Object.Instantiate(SplodeOnSpawn[i], base.transform.position, base.transform.rotation);
				}
				ParticleTrail.SetParent(null);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
