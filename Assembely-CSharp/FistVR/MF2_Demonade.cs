using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class MF2_Demonade : MonoBehaviour, IFVRDamageable
	{
		private bool m_hasExploded;

		public Vector2 FuseTimeRange = new Vector2(2.3f, 2.3f);

		private float m_fuseTime = 2.3f;

		public bool ExplodesOnContact = true;

		public string ExplodeOnContactLayerName;

		public List<GameObject> SpawnOnExplode;

		public bool MatSwitches = true;

		public int IFF;

		public List<Material> Mats;

		public Renderer Rend;

		public void SetIFF(int i)
		{
			IFF = i;
			i = Mathf.Clamp(i, 0, 2);
			if (MatSwitches)
			{
				Rend.material = Mats[i];
			}
		}

		private void Start()
		{
			m_fuseTime = Random.Range(FuseTimeRange.x, FuseTimeRange.y);
		}

		private void Update()
		{
			m_fuseTime -= Time.deltaTime;
			if (m_fuseTime <= 0f)
			{
				Explode();
			}
		}

		public void Damage(Damage d)
		{
			if (d.Class != FistVR.Damage.DamageClass.Explosive)
			{
				Explode();
			}
		}

		private void Explode()
		{
			if (m_hasExploded)
			{
				return;
			}
			m_hasExploded = true;
			for (int i = 0; i < SpawnOnExplode.Count; i++)
			{
				GameObject gameObject = Object.Instantiate(SpawnOnExplode[i], base.transform.position, Random.rotation);
				Explosion component = gameObject.GetComponent<Explosion>();
				if (component != null)
				{
					component.IFF = IFF;
				}
				ExplosionSound component2 = gameObject.GetComponent<ExplosionSound>();
				if (component2 != null)
				{
					component2.IFF = IFF;
				}
			}
			Object.Destroy(base.gameObject);
		}

		private void OnCollisionEnter(Collision collision)
		{
			if (ExplodesOnContact && !(collision.contacts[0].otherCollider.attachedRigidbody == null))
			{
				string text = LayerMask.LayerToName(collision.contacts[0].otherCollider.gameObject.layer);
				if (ExplodeOnContactLayerName == text)
				{
					Explode();
				}
			}
		}
	}
}
