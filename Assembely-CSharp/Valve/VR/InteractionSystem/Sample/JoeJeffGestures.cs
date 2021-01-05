using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class JoeJeffGestures : MonoBehaviour
	{
		private const float openFingerAmount = 0.1f;

		private const float closedFingerAmount = 0.9f;

		private const float closedThumbAmount = 0.4f;

		private JoeJeff joeJeff;

		private bool lastPeaceSignState;

		private void Awake()
		{
			joeJeff = GetComponent<JoeJeff>();
		}

		private void Update()
		{
			if (Player.instance == null)
			{
				return;
			}
			Transform transform = Camera.main.transform;
			if (!(Vector3.Angle(transform.forward, base.transform.position - transform.position) < 90f))
			{
				return;
			}
			for (int i = 0; i < Player.instance.hands.Length; i++)
			{
				if (!(Player.instance.hands[i] != null))
				{
					continue;
				}
				SteamVR_Behaviour_Skeleton skeleton = Player.instance.hands[i].skeleton;
				if (skeleton != null)
				{
					if (skeleton.indexCurl <= 0.1f && skeleton.middleCurl <= 0.1f && skeleton.thumbCurl >= 0.4f && skeleton.ringCurl >= 0.9f && skeleton.pinkyCurl >= 0.9f)
					{
						PeaceSignRecognized(currentPeaceSignState: true);
					}
					else
					{
						PeaceSignRecognized(currentPeaceSignState: false);
					}
				}
			}
		}

		private void PeaceSignRecognized(bool currentPeaceSignState)
		{
			if (!lastPeaceSignState && currentPeaceSignState)
			{
				joeJeff.Jump();
			}
			lastPeaceSignState = currentPeaceSignState;
		}
	}
}
