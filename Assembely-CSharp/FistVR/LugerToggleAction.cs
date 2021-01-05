using UnityEngine;

namespace FistVR
{
	public class LugerToggleAction : MonoBehaviour
	{
		public HandgunSlide Slide;

		public Transform BarrelSlide;

		public Transform BarrelSlideForward;

		public Transform BarrelSlideLockPoint;

		public Transform TogglePiece1;

		public Transform TogglePiece2;

		public Transform TogglePiece3;

		public Vector2 RotSet1 = new Vector2(0f, -86f);

		public Vector2 RotSet2 = new Vector2(0f, 132.864f);

		public Vector2 PosSet1 = new Vector2(0.02199817f, -0.02124f);

		public float Height = 0.03527606f;

		private void Update()
		{
			float t = 1f - Slide.GetSlideLerpBetweenRearAndFore();
			BarrelSlide.localPosition = Vector3.Lerp(BarrelSlideForward.localPosition, BarrelSlideLockPoint.localPosition, t);
			float x = Mathf.Lerp(RotSet1.x, RotSet1.y, t);
			float x2 = Mathf.Lerp(RotSet2.x, RotSet2.y, t);
			float z = Mathf.Lerp(PosSet1.x, PosSet1.y, t);
			Vector3 localEulerAngles = new Vector3(x, 0f, 0f);
			TogglePiece1.localEulerAngles = localEulerAngles;
			Vector3 localEulerAngles2 = new Vector3(x2, 0f, 0f);
			TogglePiece2.localEulerAngles = localEulerAngles2;
			Vector3 localPosition = new Vector3(0f, Height, z);
			TogglePiece3.localPosition = localPosition;
		}
	}
}
