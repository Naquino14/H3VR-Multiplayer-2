using UnityEngine;

namespace FistVR
{
	public class wwBotWurstDamageablePiece : MonoBehaviour, IFVRDamageable
	{
		public wwBotWurst Bot;

		public float m_life = 10f;

		private bool m_destroyed;

		public Rigidbody[] Shards;

		public GameObject[] Spawns;

		public Transform SpawnPoint;

		public GameObject[] SpawnsOnlySplode;

		private float m_countDownToExplode = 10f;

		private bool m_isCountingDown;

		private bool m_hasSploded;

		public Transform Child;

		public Joint ParentAttachingJoint;

		public FVRDestroyableObject[] DetachingPieces;

		public bool UsesParams = true;

		public FVRDestroyableObject.DetachRBParams DetachRigidbodyParams;

		public Vector2 DestroyEventTimeRange;

		private bool m_hasDetached;

		private bool m_isHead;

		public void SetIsHead(bool b)
		{
			m_isHead = true;
		}

		public void SetLife(float i)
		{
			m_life = i;
		}

		public void StartCountingDown()
		{
			m_isCountingDown = true;
			m_countDownToExplode = Random.Range(2f, 5f);
		}

		public void Update()
		{
			if (m_isCountingDown)
			{
				m_countDownToExplode -= Time.deltaTime;
				if (m_countDownToExplode <= 0f && !m_hasSploded)
				{
					m_hasSploded = true;
					Splode();
					Object.Destroy(base.gameObject);
				}
			}
		}

		public void Splode()
		{
			if (Child != null)
			{
				Child.SetParent(null);
			}
			if (ParentAttachingJoint != null)
			{
				Object.Destroy(ParentAttachingJoint);
			}
			for (int i = 0; i < Spawns.Length; i++)
			{
				Object.Instantiate(Spawns[i], SpawnPoint.position, SpawnPoint.rotation);
			}
			for (int j = 0; j < SpawnsOnlySplode.Length; j++)
			{
				Object.Instantiate(SpawnsOnlySplode[j], SpawnPoint.position, SpawnPoint.rotation);
			}
			for (int k = 0; k < Shards.Length; k++)
			{
				Shards[k].gameObject.SetActive(value: true);
				Shards[k].transform.SetParent(null);
				Shards[k].AddForceAtPosition(-Vector3.up * Random.Range(0.1f, 1f), base.transform.position, ForceMode.Impulse);
			}
			if (DetachingPieces.Length > 0 && !m_hasDetached)
			{
				DetachPieces();
			}
		}

		private void Explode(Damage dam)
		{
			if (Child != null)
			{
				Child.SetParent(null);
			}
			if (ParentAttachingJoint != null)
			{
				Object.Destroy(ParentAttachingJoint);
			}
			for (int i = 0; i < Spawns.Length; i++)
			{
				Object.Instantiate(Spawns[i], SpawnPoint.position, SpawnPoint.rotation);
			}
			for (int j = 0; j < Shards.Length; j++)
			{
				Shards[j].gameObject.SetActive(value: true);
				Shards[j].transform.SetParent(null);
				Shards[j].AddForceAtPosition(dam.strikeDir * Random.Range(0.1f, 1f), dam.point, ForceMode.Impulse);
			}
			if (DetachingPieces.Length > 0 && !m_hasDetached)
			{
				DetachPieces();
			}
		}

		private void DetachPieces()
		{
			m_hasDetached = true;
			for (int i = 0; i < DetachingPieces.Length; i++)
			{
				if (!(DetachingPieces[i] == null))
				{
					DetachingPieces[i].transform.SetParent(null);
					Rigidbody rigidbody = DetachingPieces[i].gameObject.AddComponent<Rigidbody>();
					if (UsesParams)
					{
						rigidbody.mass = DetachRigidbodyParams.Mass;
						rigidbody.drag = DetachRigidbodyParams.Drag;
						rigidbody.angularDrag = DetachRigidbodyParams.AngularDrag;
					}
					DetachingPieces[i].Invoke("DestroyEvent", Random.Range(DestroyEventTimeRange.x, DestroyEventTimeRange.y));
				}
			}
		}

		public void Damage(Damage dam)
		{
			if (m_isHead && Bot != null)
			{
				Bot.StunBot(Mathf.Max(0.5f, dam.Dam_Stunning));
			}
			if (m_destroyed)
			{
				return;
			}
			float num = 0f;
			num += dam.Dam_Blunt;
			num += dam.Dam_Piercing * 1f;
			num += dam.Dam_Cutting * 1.5f;
			m_life -= num;
			if (m_life <= 0f)
			{
				if (Bot != null)
				{
					Bot.RegisterHit(dam, IsDeath: true);
				}
				m_destroyed = true;
				Explode(dam);
				if (Bot != null)
				{
					Bot.BotExplosionPiece(this, dam.point, dam.strikeDir);
				}
				else
				{
					Object.Destroy(base.gameObject);
				}
			}
			else if (Bot != null)
			{
				Bot.RegisterHit(dam, IsDeath: false);
			}
		}
	}
}
