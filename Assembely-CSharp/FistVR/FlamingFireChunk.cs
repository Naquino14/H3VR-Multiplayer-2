using UnityEngine;

namespace FistVR
{
	public class FlamingFireChunk : MonoBehaviour
	{
		public Rigidbody RB;

		public Vector2 StartVel = new Vector2(5f, 15f);

		private void Start()
		{
			RB.velocity = base.transform.forward * Random.Range(StartVel.x, StartVel.y);
		}
	}
}
