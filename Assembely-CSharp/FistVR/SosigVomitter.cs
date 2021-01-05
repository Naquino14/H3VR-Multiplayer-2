using UnityEngine;

namespace FistVR
{
	public class SosigVomitter : MonoBehaviour
	{
		public AudioEvent AudEvent_Vomit;

		public ParticleSystem PSystem_Vomit;

		private float m_tickDownToVomit = 0.25f;

		private void Start()
		{
			m_tickDownToVomit = Random.Range(0.6f, 2.8f);
		}

		private void Update()
		{
			m_tickDownToVomit -= Time.deltaTime;
			if (m_tickDownToVomit <= 0f)
			{
				m_tickDownToVomit = Random.Range(0.6f, 2.8f);
				PSystem_Vomit.Emit(10);
				SM.PlayCoreSound(FVRPooledAudioType.NPCBarks, AudEvent_Vomit, base.transform.position);
			}
		}
	}
}
