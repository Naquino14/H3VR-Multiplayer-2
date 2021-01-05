using UnityEngine;

namespace FistVR
{
	public class IPSCOscillator : MonoBehaviour
	{
		public float LerpOscillator;

		public float Speed = 0.1f;

		public bool MovingUp = true;

		public Rigidbody Me;

		public Transform Point1;

		public Transform Point2;

		private void FixedUpdate()
		{
			if (MovingUp)
			{
				LerpOscillator += Time.deltaTime * Speed;
				if (LerpOscillator >= 1f)
				{
					MovingUp = false;
					LerpOscillator = 1f;
				}
			}
			else
			{
				LerpOscillator -= Time.deltaTime * Speed;
				if (LerpOscillator <= 0f)
				{
					MovingUp = true;
					LerpOscillator = 0f;
				}
			}
			Vector3 position = Vector3.Lerp(Point1.position, Point2.position, LerpOscillator);
			Me.MovePosition(position);
		}
	}
}
