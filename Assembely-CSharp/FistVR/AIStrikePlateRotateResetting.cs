using UnityEngine;

namespace FistVR
{
	public class AIStrikePlateRotateResetting : AIStrikePlate
	{
		private bool isFlippedUp;

		public float XRotUp;

		public float XRotDown;

		private float curXRot;

		private float tarXRot;

		public GameObject FlipUpTarget;

		public string FlipUpMessage;

		public GameObject FlipDownTarget;

		public string FlipDownMessage;

		public override void Damage(Damage dam)
		{
			base.Damage(dam);
		}

		public override void Reset()
		{
			isFlippedUp = false;
			tarXRot = XRotDown;
			FlipDownTarget.SendMessage(FlipDownMessage);
			base.Reset();
		}

		public void Update()
		{
			curXRot = Mathf.Lerp(curXRot, tarXRot, 15f);
			base.transform.localEulerAngles = new Vector3(curXRot, 0f, 0f);
		}

		public override void PlateFelled()
		{
			isFlippedUp = true;
			tarXRot = XRotUp;
			FlipUpTarget.SendMessage(FlipUpMessage);
			CancelInvoke();
			Invoke("Reset", 10f);
		}
	}
}
