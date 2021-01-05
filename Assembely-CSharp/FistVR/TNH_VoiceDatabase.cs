using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_VoiceDatabase", menuName = "TNH/TNH_VoiceDatabase", order = 0)]
	public class TNH_VoiceDatabase : ScriptableObject
	{
		[Serializable]
		public class TNH_VoiceLine
		{
			public TNH_VoiceLineID ID;

			public AudioClip Clip_Standard;

			public AudioClip Clip_Corrupted;
		}

		public List<TNH_VoiceLine> Lines;
	}
}
