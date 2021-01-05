using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class AmbientSound : MonoBehaviour
	{
		private AudioSource s;

		public float fadeintime;

		private float t;

		public bool fadeblack;

		private float vol;

		private void Start()
		{
			AudioListener.volume = 1f;
			s = GetComponent<AudioSource>();
			s.time = Random.Range(0f, s.clip.length);
			if (fadeintime > 0f)
			{
				t = 0f;
			}
			vol = s.volume;
			SteamVR_Fade.Start(Color.black, 0f);
			SteamVR_Fade.Start(Color.clear, 7f);
		}

		private void Update()
		{
			if (fadeintime > 0f && t < 1f)
			{
				t += Time.deltaTime / fadeintime;
				s.volume = t * vol;
			}
		}
	}
}
