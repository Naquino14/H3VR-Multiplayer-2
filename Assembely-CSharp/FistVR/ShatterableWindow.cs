using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ShatterableWindow : MonoBehaviour, IFVRDamageable
	{
		public Collider Col;

		public List<Renderer> Rends;

		private bool m_isShattered;

		private float m_life = 300f;

		public List<Transform> Shatter_Points;

		public List<GameObject> Shatter_Shards;

		public AudioEvent AudEvent_Shatter;

		private void Awake()
		{
			m_life = Random.Range(200f, 800f);
		}

		public void Damage(Damage d)
		{
			if (!m_isShattered && d.Dam_TotalKinetic > 0f)
			{
				m_life -= d.Dam_TotalKinetic;
				if (m_life <= 0f)
				{
					Shatter(d.point, d.strikeDir, 1f);
				}
			}
		}

		private void Shatter(Vector3 point, Vector3 dir, float magnitude)
		{
			if (!m_isShattered)
			{
				m_isShattered = true;
				Col.enabled = false;
				for (int i = 0; i < Rends.Count; i++)
				{
					Rends[i].enabled = false;
				}
				for (int j = 0; j < Shatter_Points.Count; j++)
				{
					GameObject gameObject = Object.Instantiate(Shatter_Shards[j], Shatter_Points[j].position, Shatter_Points[j].rotation);
					Rigidbody component = gameObject.GetComponent<Rigidbody>();
					component.AddExplosionForce(Random.Range(1f, 3f), point, 3f, 0.1f, ForceMode.Impulse);
				}
				SM.PlayCoreSoundDelayed(FVRPooledAudioType.Generic, AudEvent_Shatter, base.transform.position, Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position) / 343f);
			}
		}
	}
}
