using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MF2_StickyBomb : MonoBehaviour, IFVRDamageable
	{
		public bool m_hasExploded;

		public bool m_hasStuck;

		public List<GameObject> SpawnOnExplode;

		public Rigidbody RB;

		public int IFF;

		public List<Material> Mats;

		public Renderer Rend;

		public bool DelayFuse;

		public Vector2 FuseTime;

		private float fuse = 1f;

		private bool m_isFUsing;

		public void SetIFF(int i)
		{
			IFF = i;
			i = Mathf.Clamp(i, 0, 2);
			Rend.material = Mats[i];
		}

		private void Start()
		{
		}

		public void Detonate()
		{
			if (DelayFuse)
			{
				fuse = Random.Range(FuseTime.x, FuseTime.y);
				m_isFUsing = true;
			}
			else
			{
				Explode();
			}
		}

		private void Update()
		{
			if (m_isFUsing)
			{
				fuse -= Time.deltaTime;
				if (fuse <= 0f)
				{
					Explode();
				}
			}
		}

		private void Stick()
		{
			RB.isKinematic = true;
			m_hasStuck = true;
		}

		private void UnStick()
		{
			RB.isKinematic = false;
			m_hasStuck = false;
		}

		private void Explode()
		{
			if (!m_hasExploded)
			{
				m_hasExploded = true;
				for (int i = 0; i < SpawnOnExplode.Count; i++)
				{
					Object.Instantiate(SpawnOnExplode[i], base.transform.position, Random.rotation);
				}
				Object.Destroy(base.gameObject);
			}
		}

		public void Damage(Damage d)
		{
			if (m_hasStuck && d.Class != FistVR.Damage.DamageClass.Explosive)
			{
				Explode();
			}
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (!m_hasStuck && !(collision.contacts[0].otherCollider.attachedRigidbody != null))
			{
				Stick();
			}
		}
	}
}
