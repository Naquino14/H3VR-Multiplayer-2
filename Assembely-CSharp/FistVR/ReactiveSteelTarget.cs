using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ReactiveSteelTarget : MonoBehaviour, IFVRDamageable
	{
		private Rigidbody rb;

		private bool m_hasRB;

		public AudioEvent HitEvent;

		public float HitSoundRefire = 0.1f;

		private float m_refireTick;

		public GameObject[] BulletHolePrefabs;

		private List<GameObject> m_currentHoles = new List<GameObject>();

		public FVRPooledAudioType PoolType = FVRPooledAudioType.GenericLongRange;

		private int holeindex;

		public void Awake()
		{
			rb = GetComponent<Rigidbody>();
			if (rb != null)
			{
				m_hasRB = true;
			}
		}

		public void Update()
		{
			if (m_refireTick > 0f)
			{
				m_refireTick -= Time.deltaTime;
			}
		}

		private void PlayHitSound(float soundMultiplier)
		{
			if (m_refireTick <= 0f)
			{
				m_refireTick = HitSoundRefire;
				float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
				float num2 = num / 343f;
				SM.PlayCoreSoundDelayedOverrides(PoolType, HitEvent, base.transform.position, HitEvent.VolumeRange * soundMultiplier, HitEvent.PitchRange, num2 + 0.04f);
			}
		}

		public void Damage(Damage dam)
		{
			if (dam.Class != FistVR.Damage.DamageClass.Projectile)
			{
				return;
			}
			Vector3 position = dam.point + dam.hitNormal * Random.Range(0.002f, 0.008f);
			if (BulletHolePrefabs.Length > 0)
			{
				if (m_currentHoles.Count > 20)
				{
					holeindex++;
					if (holeindex > 19)
					{
						holeindex = 0;
					}
					m_currentHoles[holeindex].transform.position = position;
					m_currentHoles[holeindex].transform.rotation = Quaternion.LookRotation(dam.hitNormal, Random.onUnitSphere);
				}
				else
				{
					GameObject gameObject = Object.Instantiate(BulletHolePrefabs[Random.Range(0, BulletHolePrefabs.Length)], position, Quaternion.LookRotation(dam.hitNormal, Random.onUnitSphere));
					gameObject.transform.SetParent(base.transform);
					m_currentHoles.Add(gameObject);
				}
			}
			if (m_hasRB)
			{
				rb.AddForceAtPosition(dam.strikeDir * dam.Dam_Blunt * 0.01f, dam.point, ForceMode.Impulse);
			}
			PlayHitSound(1f);
		}

		public void ClearHoles()
		{
			for (int num = m_currentHoles.Count - 1; num >= 0; num--)
			{
				Object.Destroy(m_currentHoles[num]);
			}
			m_currentHoles.Clear();
		}
	}
}
