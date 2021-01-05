using UnityEngine;

namespace FistVR
{
	public class MG_LaserMineBeam : MonoBehaviour
	{
		public MG_LaserMine Mine;

		private void OnTriggerEnter(Collider col)
		{
			LaserTest(col);
		}

		private void LaserTest(Collider col)
		{
			if (!(col.attachedRigidbody == null) && !col.attachedRigidbody.transform.root.gameObject.CompareTag("Agent") && Mine != null)
			{
				Mine.Explode();
			}
		}
	}
}
