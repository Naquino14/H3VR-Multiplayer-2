using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class InSceneShotTailTesting : MonoBehaviour
	{
		public FVRFireArm FireArmToTest;

		public AudioEvent[] ShotSets;

		public FVRSoundEnvironment[] Environments;

		public Text DebugDisplay;

		public AudioSource Aud;

		public void SetShotSet(int i)
		{
		}

		private void Update()
		{
		}

		public void SetTailSet(int i)
		{
			GM.CurrentSceneSettings.DefaultSoundEnvironment = Environments[i];
			Aud.PlayOneShot(Aud.clip, 0.25f);
		}
	}
}
