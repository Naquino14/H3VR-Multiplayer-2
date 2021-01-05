using UnityEngine;

namespace FistVR
{
	public class BikeBell : FVRInteractiveObject
	{
		public enum BellState
		{
			Unrung,
			Ringing,
			Unringing
		}

		private BellState Bstate;

		private float bLerp;

		public Transform Piece;

		public Vector2 PieceRange;

		public AudioEvent AudEvent_Ring;

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			if (Bstate == BellState.Unrung)
			{
				Bstate = BellState.Ringing;
				bLerp = 0f;
				SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Ring, base.transform.position);
				int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
				GM.CurrentSceneSettings.OnPerceiveableSound(40f, 12f, base.transform.position, playerIFF);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			switch (Bstate)
			{
			case BellState.Ringing:
				bLerp += Time.deltaTime * 6f;
				if (bLerp >= 1f)
				{
					bLerp = 1f;
					Bstate = BellState.Unringing;
				}
				Piece.localEulerAngles = new Vector3(0f, Mathf.Lerp(PieceRange.x, PieceRange.y, bLerp), 0f);
				break;
			case BellState.Unringing:
				bLerp -= Time.deltaTime * 6f;
				if (bLerp <= 0f)
				{
					bLerp = 0f;
					Bstate = BellState.Unrung;
				}
				Piece.localEulerAngles = new Vector3(0f, Mathf.Lerp(PieceRange.x, PieceRange.y, bLerp), 0f);
				break;
			}
		}
	}
}
