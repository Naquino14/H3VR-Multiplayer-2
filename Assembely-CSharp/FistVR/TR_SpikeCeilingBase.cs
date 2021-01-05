using UnityEngine;

namespace FistVR
{
	public class TR_SpikeCeilingBase : MonoBehaviour, IMG_HandlePumpable
	{
		public TR_SpikeCeilingBaseHandle Handle;

		public Transform Jack;

		public float CurHeight = 0.8f;

		public float MinHeight = 0.8f;

		public float MaxHeight = 4.2f;

		public ParticleSystem Sparks;

		public void Pump(float delta)
		{
			CurHeight += delta * 0.002f;
			CurHeight = Mathf.Clamp(CurHeight, MinHeight, MaxHeight);
			Jack.localPosition = new Vector3(Jack.localPosition.x, CurHeight, Jack.localPosition.z);
		}

		public void LowerTo(float max)
		{
			if (CurHeight > max)
			{
				CurHeight = Mathf.Clamp(max, MinHeight, MaxHeight);
				Jack.localPosition = new Vector3(Jack.localPosition.x, CurHeight, Jack.localPosition.z);
				Sparks.Emit(1);
			}
		}
	}
}
