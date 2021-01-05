using UnityEngine;

namespace FistVR
{
	public class IcoSphereTarget : MonoBehaviour, IFVRDamageable
	{
		public BevelCubeTarget.BevelCubeTargetMovementType MoveType;

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

		private ParticleSystem DamageParts;

		public ArmorPlate[] ArmorPieces;

		public void Start()
		{
			DamageParts = GetComponent<ParticleSystem>();
			Game = GameObject.Find("CubeGame").GetComponent<Cubegame>();
			rb = GetComponent<Rigidbody>();
			switch (MoveType)
			{
			case BevelCubeTarget.BevelCubeTargetMovementType.TowardCenter:
				rb.velocity = -base.transform.position * VelMultiplier;
				rb.angularVelocity = Random.onUnitSphere * RotMultiplier;
				break;
			case BevelCubeTarget.BevelCubeTargetMovementType.Random:
				rb.velocity = Random.onUnitSphere * VelMultiplier;
				rb.angularVelocity = Random.onUnitSphere * RotMultiplier;
				break;
			case BevelCubeTarget.BevelCubeTargetMovementType.Vertical:
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

		public void DamageParticle(Vector3 point, int num)
		{
			for (int i = 0; i < num; i++)
			{
				DamageParts.Emit(point, Random.onUnitSphere * 2f, 1.5f, 0.2f, Color.white);
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
				return;
			}
			for (int i = 0; i < 3; i++)
			{
				DamageParts.Emit(d.point, -d.strikeDir * 5f, 1f, 0.35f, Color.white);
			}
		}

		public void Boom(bool getPoints)
		{
			if (m_isDestroyed)
			{
				return;
			}
			for (int i = 0; i < ArmorPieces.Length; i++)
			{
				if (ArmorPieces[i] != null)
				{
					ArmorPieces[i].Detach(ArmorPieces[i].transform.position - base.transform.position);
				}
			}
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

		private void OnCollisionEnter(Collision col)
		{
			if (DoesSuicide && col.other.gameObject.layer == LayerMask.NameToLayer("ColOnlyTarget"))
			{
				Boom(getPoints: false);
			}
		}
	}
}
