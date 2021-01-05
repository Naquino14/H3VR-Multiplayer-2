using UnityEngine;

public class PlayRandomClipOnAwake : MonoBehaviour
{
	public AudioSource source;

	public AudioClip[] clips;

	public Vector2 pitchRange;

	public Vector2 volumeRange;

	private void Awake()
	{
		source.pitch = Random.Range(pitchRange.x, pitchRange.y);
		source.PlayOneShot(clips[Random.Range(0, clips.Length)], Random.Range(volumeRange.x, volumeRange.y));
	}
}
