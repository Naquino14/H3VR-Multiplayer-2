using UnityEngine;

namespace FistVR
{
	public class SingleActionRevolverCylinder : MonoBehaviour
	{
		public int NumChambers = 6;

		public FVRFireArmChamber[] Chambers;

		public int GetClosestChamberIndex()
		{
			float num = 0f - base.transform.localEulerAngles.z;
			num += 360f / (float)NumChambers * 0.5f;
			num = Mathf.Repeat(num, 360f);
			return Mathf.CeilToInt(num / (360f / (float)NumChambers)) - 1;
		}

		public Quaternion GetLocalRotationFromCylinder(int cylinder)
		{
			float t = (float)cylinder * (360f / (float)NumChambers) * -1f;
			t = Mathf.Repeat(t, 360f);
			return Quaternion.Euler(new Vector3(0f, 0f, t));
		}
	}
}
