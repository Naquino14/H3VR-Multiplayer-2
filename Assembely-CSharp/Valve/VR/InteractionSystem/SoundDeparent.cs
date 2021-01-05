using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	public class SoundDeparent : MonoBehaviour
	{
		public bool destroyAfterPlayOnce = true;

		private AudioSource thisAudioSource;

		private void Awake()
		{
			thisAudioSource = GetComponent<AudioSource>();
		}

		private void Start()
		{
			base.gameObject.transform.parent = null;
			if (destroyAfterPlayOnce)
			{
				Object.Destroy(base.gameObject, thisAudioSource.clip.length);
			}
		}
	}
}
