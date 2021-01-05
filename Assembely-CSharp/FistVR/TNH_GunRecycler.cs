using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class TNH_GunRecycler : MonoBehaviour
	{
		public TNH_Manager M;

		public Transform Spawnpoint_Token;

		public Transform ScanningVolume;

		public LayerMask ScanningLM;

		public AudioEvent AudEvent_Fail;

		public AudioEvent AudEvent_Spawn;

		private List<FVRFireArm> m_detectedFirearms = new List<FVRFireArm>();

		private Collider[] colbuffer;

		private float m_scanTick = 1f;

		private void Start()
		{
			colbuffer = new Collider[50];
		}

		public void Button_Recycler()
		{
			if (m_detectedFirearms.Count <= 0)
			{
				SM.PlayCoreSound(FVRPooledAudioType.UIChirp, AudEvent_Fail, base.transform.position);
				return;
			}
			if (m_detectedFirearms[0] != null)
			{
				Object.Destroy(m_detectedFirearms[0].gameObject);
			}
			m_detectedFirearms.Clear();
			M.AddTokens(1, Scorethis: false);
			M.EnqueueTokenLine(1);
		}

		private void Update()
		{
			m_scanTick -= Time.deltaTime;
			if (m_scanTick <= 0f)
			{
				m_scanTick = Random.Range(0.8f, 1f);
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.transform.position);
				if (num < 12f)
				{
					Scan();
				}
			}
		}

		private void Scan()
		{
			int num = Physics.OverlapBoxNonAlloc(ScanningVolume.position, ScanningVolume.localScale * 0.5f, colbuffer, ScanningVolume.rotation, ScanningLM, QueryTriggerInteraction.Collide);
			m_detectedFirearms.Clear();
			for (int i = 0; i < num; i++)
			{
				if (colbuffer[i].attachedRigidbody != null)
				{
					FVRFireArm component = colbuffer[i].attachedRigidbody.gameObject.GetComponent<FVRFireArm>();
					if (component != null && !component.SpawnLockable && !component.IsHeld && component.QuickbeltSlot == null && !m_detectedFirearms.Contains(component))
					{
						m_detectedFirearms.Add(component);
					}
				}
			}
		}
	}
}
