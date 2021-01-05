using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class AudioEvent
	{
		public List<AudioClip> Clips;

		public Vector2 VolumeRange = new Vector2(1f, 1f);

		public Vector2 PitchRange = new Vector2(1f, 1f);

		public Vector2 ClipLengthRange = new Vector2(1f, 1f);

		public AudioEvent()
		{
			Clips = new List<AudioClip>();
		}

		public void SetLengthRange()
		{
			float length = Clips[0].length;
			float length2 = Clips[0].length;
			for (int i = 1; i < Clips.Count; i++)
			{
				if (Clips[i].length < length)
				{
					length = Clips[i].length;
				}
				if (Clips[i].length > length2)
				{
					length2 = Clips[i].length;
				}
			}
			ClipLengthRange = new Vector2(length, length2);
		}
	}
}
