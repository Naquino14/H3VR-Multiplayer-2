using UnityEngine;

namespace FistVR
{
	public class wwTargetShatterable : wwTarget
	{
		private bool m_isShattered;

		public Rigidbody RB;

		public Rigidbody[] Shards;

		public GameObject[] Spawns;

		public bool DoSpawnsAlignWithStrikerDir;

		public bool DoSpawnsRotateRandom;

		public float explosionMultiplier = 1f;

		public FVRObject ObjectWrapper;

		public bool IsTNT;

		public Vector3 compareTo;

		public float DistTrigger;

		public bool UsesPoints;

		public float PointsLife = 1200f;

		public bool IgnoresCollisionDamage;

		public override void TargetStruck(Damage dam, bool sendStruckEvent)
		{
			base.TargetStruck(dam, sendStruckEvent);
			if (UsesPoints)
			{
				PointsLife -= dam.Dam_TotalKinetic;
				if (PointsLife > 0f)
				{
					return;
				}
			}
			if (!m_isShattered)
			{
				if (hasManager)
				{
					Manager.PrimeForRespawn(this, m_originalPos, m_originalRot, m_originalScale, DoesRescale);
				}
				m_isShattered = true;
				Shatter(dam);
			}
		}

		public void OnCollisionEnter(Collision col)
		{
			if (col.relativeVelocity.magnitude > 3f && !IgnoresCollisionDamage)
			{
				Damage damage = new Damage();
				damage.strikeDir = col.relativeVelocity.normalized;
				damage.point = col.contacts[0].point;
				damage.hitNormal = col.contacts[0].normal;
				damage.Class = FistVR.Damage.DamageClass.Environment;
				damage.Dam_Blunt = col.relativeVelocity.magnitude * 100f;
				TargetStruck(damage, sendStruckEvent: false);
			}
		}

		private void Shatter(Damage dam)
		{
			if (IsTNT && Vector3.Distance(base.transform.position, compareTo) <= DistTrigger && hasManager)
			{
				Manager.Manager.ExplodeBullet();
			}
			Vector3 vector = Vector3.ClampMagnitude(dam.strikeDir * dam.Dam_Blunt * 0.01f, 100f);
			for (int i = 0; i < Shards.Length; i++)
			{
				if (RB != null)
				{
					Shards[i].velocity = RB.velocity;
					Shards[i].angularVelocity = RB.angularVelocity;
				}
				Shards[i].gameObject.SetActive(value: true);
				Shards[i].transform.SetParent(null);
				Shards[i].AddForceAtPosition(explosionMultiplier * vector * (1f / (float)Shards.Length), dam.point, ForceMode.Impulse);
			}
			for (int j = 0; j < Spawns.Length; j++)
			{
				GameObject gameObject = Object.Instantiate(Spawns[j], base.transform.position, base.transform.rotation);
				if (DoSpawnsAlignWithStrikerDir)
				{
					gameObject.transform.rotation = Quaternion.LookRotation(dam.strikeDir);
				}
				else if (DoSpawnsRotateRandom)
				{
					gameObject.transform.rotation = Random.rotation;
				}
			}
			Object.Destroy(base.gameObject);
		}
	}
}
