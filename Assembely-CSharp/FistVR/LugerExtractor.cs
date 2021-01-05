using UnityEngine;

namespace FistVR
{
	public class LugerExtractor : MonoBehaviour
	{
		public FVRFireArmChamber Chamber;

		private bool isUp;

		public float RotDown;

		public float RotUp;

		private void Update()
		{
			if (isUp)
			{
				if (!Chamber.IsFull)
				{
					isUp = false;
					base.transform.localEulerAngles = new Vector3(RotDown, 0f, 0f);
				}
			}
			else if (Chamber.IsFull)
			{
				isUp = true;
				base.transform.localEulerAngles = new Vector3(RotUp, 0f, 0f);
			}
		}
	}
}
