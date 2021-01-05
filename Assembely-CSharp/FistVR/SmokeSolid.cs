using UnityEngine;

namespace FistVR
{
	public class SmokeSolid : MonoBehaviour, IFVRDamageable
	{
		public Rigidbody RB;

		public AnimationCurve ScaleOverLife;

		public AnimationCurve AngularDragCurve;

		public float ScaleMult = 1f;

		public float Life;

		public float DecaySpeed = 0.1f;

		private void Start()
		{
		}

		private void FixedUpdate()
		{
			Life += DecaySpeed * Time.deltaTime;
			RB.angularDrag = AngularDragCurve.Evaluate(Life);
			float num = ScaleOverLife.Evaluate(Life) * ScaleMult;
			if (Life >= 1f)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				base.transform.localScale = new Vector3(num, num, num);
			}
		}

		public void Damage(Damage d)
		{
			DecaySpeed += DecaySpeed * 0.25f;
		}
	}
}
