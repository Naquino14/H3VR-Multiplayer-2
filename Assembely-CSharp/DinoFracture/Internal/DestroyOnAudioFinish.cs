using UnityEngine;

namespace DinoFracture.Internal
{
	[RequireComponent(typeof(AudioSource))]
	public class DestroyOnAudioFinish : MonoBehaviour
	{
		private AudioSource _source;

		private void Start()
		{
			_source = GetComponent<AudioSource>();
			_source.Play();
		}

		private void Update()
		{
			if (!_source.isPlaying)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
