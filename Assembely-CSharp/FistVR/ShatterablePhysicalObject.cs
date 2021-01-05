using UnityEngine;

namespace FistVR
{
	public class ShatterablePhysicalObject : FVRPhysicalObject, IFVRDamageable
	{
		public float currentToughness = 200f;

		[Header("Shatterable Params")]
		public PMat Pmaterial;

		public Rigidbody[] SubObjects;

		public GameObject[] SecondarySpawns;

		private bool isShattered;

		public float CollisionShatterThreshold = 5f;

		public bool TransfersVelocityExplosively;

		public float DamageReceivedMultiplier = 1f;

		private Rigidbody m_rb;

		private bool m_hasRigidbody;

		protected override void Awake()
		{
			base.Awake();
			if (m_rb != null)
			{
				m_hasRigidbody = true;
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
		}

		public void Damage(Damage dam)
		{
			if (isShattered || base.QuickbeltSlot != null)
			{
				return;
			}
			Vector3 vector = dam.Dam_TotalKinetic * dam.strikeDir * 0.01f * DamageReceivedMultiplier;
			Vector3 point = dam.point;
			currentToughness -= dam.Dam_TotalKinetic;
			if (currentToughness <= 0f)
			{
				isShattered = true;
				if (SubObjects.Length > 0)
				{
					for (int i = 0; i < SubObjects.Length; i++)
					{
						SubObjects[i].gameObject.SetActive(value: true);
						SubObjects[i].transform.SetParent(null);
						SubObjects[i].velocity = base.RootRigidbody.velocity + vector * (1f / (float)SubObjects.Length) * 0.1f;
						SubObjects[i].angularVelocity = base.RootRigidbody.velocity;
						if (TransfersVelocityExplosively)
						{
							SubObjects[i].AddForceAtPosition((SubObjects[i].transform.position - base.transform.position).normalized * SubObjects[i].velocity.magnitude / SubObjects.Length, base.transform.position, ForceMode.Impulse);
						}
					}
				}
				for (int j = 0; j < SecondarySpawns.Length; j++)
				{
					Object.Instantiate(SecondarySpawns[j], dam.point, Quaternion.LookRotation(-dam.hitNormal, Random.onUnitSphere));
				}
				if (base.IsHeld)
				{
					m_hand.EndInteractionIfHeld(this);
				}
				Object.Destroy(base.gameObject);
			}
			else
			{
				base.RootRigidbody.AddForceAtPosition(vector, point, ForceMode.Impulse);
			}
		}

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			float magnitude = col.relativeVelocity.magnitude;
			if (!(magnitude < CollisionShatterThreshold))
			{
				float num = 100f;
				if (m_hasRigidbody)
				{
					num = m_rb.mass;
				}
				float num2 = num;
				if (col.rigidbody != null)
				{
					num2 = col.rigidbody.mass;
				}
				float num3 = num2 / num;
				float num4 = magnitude * num3;
				Vector3 relativeVelocity = col.relativeVelocity;
				Damage damage = new Damage();
				damage.Dam_Blunt = num4 * magnitude * 100f;
				damage.Dam_TotalKinetic = damage.Dam_Blunt;
				damage.hitNormal = col.contacts[0].normal;
				damage.strikeDir = col.relativeVelocity.normalized;
				damage.point = col.contacts[0].point;
				damage.Class = FistVR.Damage.DamageClass.Environment;
				Debug.Log("collision:" + damage.Dam_Blunt);
				Damage(damage);
			}
		}
	}
}
