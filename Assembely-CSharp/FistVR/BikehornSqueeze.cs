using UnityEngine;

namespace FistVR
{
	public class BikehornSqueeze : FVRInteractiveObject
	{
		public Renderer Horn_Unsqueezed;

		public Renderer Horn_Squeezed;

		public Transform Spot;

		public AudioEvent AudEvent_Squeak;

		protected override void Start()
		{
			base.Start();
			Horn_Unsqueezed.enabled = true;
			Horn_Squeezed.enabled = false;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			SM.PlayCoreSound(FVRPooledAudioType.Generic, AudEvent_Squeak, Spot.position);
			int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
			GM.CurrentSceneSettings.OnPerceiveableSound(60f, 24f, base.transform.position, playerIFF);
			Horn_Unsqueezed.enabled = false;
			Horn_Squeezed.enabled = true;
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			Horn_Unsqueezed.enabled = true;
			Horn_Squeezed.enabled = false;
		}
	}
}
