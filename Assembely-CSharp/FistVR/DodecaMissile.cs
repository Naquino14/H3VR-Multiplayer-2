using UnityEngine;

namespace FistVR
{
	public class DodecaMissile : MonoBehaviour, IFVRDamageable
	{
		public DodecaLauncher Launcher;

		public GameObject DestroyEffect;

		private bool m_isDestroyed;

		public Cubegame Game;

		public int Points;

		public int Life;

		private Rigidbody rb;

		public bool IsLaunched;

		public Vector3 TargetPos = new Vector3(0f, 1.4f, 0f);

		public ParticleSystem SmokeSystem;

		public void Awake()
		{
			Game = GameObject.Find("CubeGame").GetComponent<Cubegame>();
		}

		public void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Projectile)
			{
				Life -= (int)d.Dam_TotalKinetic;
			}
			rb.AddForceAtPosition(d.strikeDir * 1f, d.point);
			if (Life <= 0)
			{
				rb.velocity = d.strikeDir * 20f;
				Boom(getPoints: true);
			}
		}

		public void Launch()
		{
			if (!IsLaunched)
			{
				if (SmokeSystem != null)
				{
					SmokeSystem.gameObject.SetActive(value: true);
				}
				IsLaunched = true;
				base.transform.SetParent(null);
				rb = base.gameObject.AddComponent<Rigidbody>();
				rb.mass = 5f;
				rb.useGravity = false;
			}
		}

		public void MisFire()
		{
			if (!IsLaunched)
			{
				SmokeSystem.gameObject.SetActive(value: true);
				IsLaunched = true;
				TargetPos = Random.onUnitSphere * 50f;
				base.transform.SetParent(null);
				rb = base.gameObject.AddComponent<Rigidbody>();
				rb.mass = 5f;
				rb.useGravity = false;
			}
		}

		public void FixedUpdate()
		{
			if (IsLaunched && rb != null)
			{
				rb.MoveRotation(Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(TargetPos - base.transform.position, Random.onUnitSphere), Time.deltaTime * 5f));
				rb.velocity = base.transform.forward * 10f;
			}
		}

		public void Boom(bool getPoints)
		{
			if (!m_isDestroyed)
			{
				if (IsLaunched && SmokeSystem != null)
				{
					SmokeSystem.gameObject.transform.SetParent(null);
					SmokeSystem.enableEmission = false;
				}
				m_isDestroyed = true;
				if (getPoints)
				{
					Game.ScorePoints(Points);
				}
				GameObject gameObject = Object.Instantiate(DestroyEffect, base.transform.position, base.transform.rotation);
				if (rb != null)
				{
					gameObject.GetComponent<Rigidbody>().velocity = rb.velocity;
				}
				Object.Destroy(base.gameObject);
			}
		}

		private void OnCollisionEnter(Collision col)
		{
			if (IsLaunched && (col.other.gameObject.layer == LayerMask.NameToLayer("ColOnlyTarget") || col.other.gameObject.tag != "Harmless"))
			{
				Boom(getPoints: false);
			}
		}
	}
}
