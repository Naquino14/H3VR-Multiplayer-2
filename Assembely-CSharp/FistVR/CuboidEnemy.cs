using UnityEngine;

namespace FistVR
{
	public class CuboidEnemy : MonoBehaviour, IFVRDamageable
	{
		public enum CuboidMoveStyle
		{
			SimpleRotate,
			TowardCenter,
			Random,
			Vertical,
			JukeAroundPos
		}

		[Header("Base Stats")]
		public int Life = 1;

		public CuboidMoveStyle MoveStyle;

		public float LinearMoveSpeed = 0.1f;

		public float RotMoveSpeed = 0.1f;

		public int DamageOnCollision = 1;

		public int Points;

		public GameObject WarpInEffect;

		public GameObject WarpOutEffect;

		public GameObject SpawnOnHit;

		public GameObject[] SpawnOnDestruction;

		private Rigidbody rb;

		private bool m_isDestroyed;

		public virtual void Awake()
		{
			rb = GetComponent<Rigidbody>();
			rb.maxAngularVelocity = 20f;
			switch (MoveStyle)
			{
			case CuboidMoveStyle.SimpleRotate:
				rb.angularVelocity = Random.onUnitSphere * RotMoveSpeed;
				break;
			case CuboidMoveStyle.TowardCenter:
				rb.velocity = -base.transform.position * LinearMoveSpeed;
				rb.angularVelocity = Random.onUnitSphere * RotMoveSpeed;
				break;
			case CuboidMoveStyle.Random:
				rb.velocity = Random.onUnitSphere * LinearMoveSpeed;
				rb.angularVelocity = Random.onUnitSphere * RotMoveSpeed;
				break;
			case CuboidMoveStyle.Vertical:
			{
				float num = Mathf.Sign(0f - base.transform.position.y);
				Vector3 velocity = new Vector3(0f, num * LinearMoveSpeed, 0f);
				rb.velocity = velocity;
				rb.angularVelocity = new Vector3(0f, RotMoveSpeed * Random.Range(-10f, 10f), 0f);
				break;
			}
			case CuboidMoveStyle.JukeAroundPos:
				rb.velocity = Random.onUnitSphere * LinearMoveSpeed;
				break;
			}
		}

		public void Start()
		{
			if (WarpInEffect != null)
			{
				Object.Instantiate(WarpInEffect, base.transform.position, base.transform.rotation);
			}
		}

		public void Damage(int i)
		{
			Life -= i;
			if (Life <= 0)
			{
				Destroy(Vector3.zero, Vector3.zero, GetPoints: false);
			}
		}

		public void Damage(int i, Vector3 force, Vector3 point)
		{
			Life -= i;
			if (Life <= 0)
			{
				Destroy(force, point, GetPoints: false);
			}
		}

		public void Damage(Damage d)
		{
			if (d.Class == FistVR.Damage.DamageClass.Projectile)
			{
				Life -= (int)d.Dam_TotalKinetic;
			}
			if (Life <= 0)
			{
				Destroy(d.strikeDir, d.point, GetPoints: true);
			}
		}

		protected void Destroy(Vector3 force, Vector3 point, bool GetPoints)
		{
			if (!m_isDestroyed)
			{
				m_isDestroyed = true;
				if (GetPoints)
				{
				}
				Object.Destroy(base.gameObject);
			}
		}

		public void WarpOut()
		{
			if (WarpOutEffect != null)
			{
				Object.Instantiate(WarpInEffect, base.transform.position, base.transform.rotation);
			}
			Object.Destroy(base.gameObject);
		}

		private void OnCollisionEnter(Collision col)
		{
			if (col.other.gameObject.layer == LayerMask.NameToLayer("ColOnlyTarget") && DamageOnCollision > 0)
			{
				Damage(DamageOnCollision);
			}
		}
	}
}
