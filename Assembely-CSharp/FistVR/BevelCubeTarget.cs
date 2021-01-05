using UnityEngine;

namespace FistVR
{
	public class BevelCubeTarget : MonoBehaviour, IFVRDamageable
	{
		public enum BevelCubeTargetMovementType
		{
			TowardCenter,
			Random,
			Vertical
		}

		public BevelCubeTargetMovementType MoveType;

		public GameObject DestroyEffect;

		private bool m_isDestroyed;

		public GameObject OnDieMessageTarget;

		public int MessageNum;

		public Cubegame Game;

		public int Points;

		public bool DoesSuicide;

		public int Life;

		private Rigidbody rb;

		public float VelMultiplier = 0.3f;

		public float RotMultiplier = 0.1f;

		public void Start()
		{
			Game = GameObject.Find("CubeGame").GetComponent<Cubegame>();
			rb = GetComponent<Rigidbody>();
			switch (MoveType)
			{
			case BevelCubeTargetMovementType.TowardCenter:
				Debug.Log("Toward Center");
				rb.velocity = -base.transform.position * VelMultiplier;
				rb.angularVelocity = Random.onUnitSphere * RotMultiplier;
				break;
			case BevelCubeTargetMovementType.Vertical:
			{
				Vector3 velocity = -base.transform.position * VelMultiplier;
				velocity.x = 0f;
				velocity.z = 0f;
				rb.velocity = velocity;
				rb.angularVelocity = new Vector3(0f, RotMultiplier * Random.Range(-3f, 3f), 0f);
				break;
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
			if (!m_isDestroyed)
			{
				Game.TargetDown();
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

		private void OnCollisionEnter(Collision col)
		{
			if (DoesSuicide && col.other.gameObject.layer == LayerMask.NameToLayer("ColOnlyTarget"))
			{
				Boom(getPoints: false);
			}
		}
	}
}
