using UnityEngine;

namespace FistVR
{
	public class DodecaLauncher : MonoBehaviour, IFVRDamageable
	{
		public DodecaMissile[] Missiles;

		public GameObject DestroyEffect;

		private bool m_isDestroyed;

		public GameObject OnDieMessageTarget;

		public int MessageNum;

		public Cubegame Game;

		public int Points;

		public int Life;

		private Rigidbody rb;

		private bool m_isLaunching = true;

		private float m_launchTick = 2f;

		public void Awake()
		{
			Game = GameObject.Find("CubeGame").GetComponent<Cubegame>();
			rb = GetComponent<Rigidbody>();
			rb.angularVelocity = Random.onUnitSphere * 15f;
		}

		public void Update()
		{
			if (!m_isLaunching)
			{
				return;
			}
			if (m_launchTick > 0f)
			{
				m_launchTick -= Time.deltaTime;
				return;
			}
			m_launchTick = 3f;
			for (int i = 0; i < 1; i++)
			{
				for (int j = 0; j < Missiles.Length; j++)
				{
					bool flag = false;
					if (!Missiles[j].IsLaunched)
					{
						Missiles[j].Launch();
						flag = true;
					}
					if (j == Missiles.Length - 1)
					{
						m_isLaunching = false;
					}
					if (flag)
					{
						break;
					}
				}
			}
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

		public void Boom(bool getPoints)
		{
			if (m_isDestroyed)
			{
				return;
			}
			Game.TargetDown();
			for (int i = 0; i < Missiles.Length; i++)
			{
				if (Missiles[i] != null && !Missiles[i].IsLaunched)
				{
					Missiles[i].MisFire();
				}
			}
			m_isDestroyed = true;
			if (getPoints)
			{
				Game.ScorePoints(Points);
			}
			GameObject gameObject = Object.Instantiate(DestroyEffect, base.transform.position, base.transform.rotation);
			gameObject.GetComponent<Rigidbody>().velocity = rb.velocity;
			Object.Destroy(base.gameObject);
		}
	}
}
