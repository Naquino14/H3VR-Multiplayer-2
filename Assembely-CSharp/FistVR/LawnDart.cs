using UnityEngine;

namespace FistVR
{
	public class LawnDart : FVRPhysicalObject
	{
		public ParticleSystem[] TrailSystems;

		public LawnDartGame Game;

		public Transform DartPoint;

		public LayerMask CastingMask;

		private RaycastHit m_hit;

		public void SetGame(LawnDartGame g)
		{
			Game = g;
		}

		protected override void FVRFixedUpdate()
		{
			base.FVRFixedUpdate();
			if (!base.IsHeld && !base.RootRigidbody.isKinematic && base.RootRigidbody.velocity.magnitude > 2f)
			{
				ParticleSystem[] trailSystems = TrailSystems;
				foreach (ParticleSystem particleSystem in trailSystems)
				{
					particleSystem.Emit(1);
				}
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(base.RootRigidbody.velocity.normalized, Vector3.up), Time.deltaTime * 4.5f);
			}
		}

		public override void OnCollisionEnter(Collision col)
		{
			base.OnCollisionEnter(col);
			if (!base.IsHeld && base.QuickbeltSlot == null && base.transform.position.y < 20f)
			{
				base.RootRigidbody.isKinematic = true;
				Scorecast();
			}
		}

		private void Scorecast()
		{
			if (base.transform.position.y < 20f && Physics.Raycast(DartPoint.position + Vector3.up * 5f, -Vector3.up, out m_hit, 100f, CastingMask))
			{
				if ((bool)m_hit.collider.gameObject.GetComponent<LawnDartPointCollider>())
				{
					LawnDartPointCollider component = m_hit.collider.gameObject.GetComponent<LawnDartPointCollider>();
					Game.ScoreEvent(DartPoint.position, component.SpecialDisplay, component.Points, component.Multiplier, this);
				}
				else
				{
					Game.ScoreEvent(DartPoint.position, "Miss!", 0, 0, this);
				}
			}
		}
	}
}
