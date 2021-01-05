using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New SlideSequence", menuName = "Temp/SlideSequenceDefinition", order = 0)]
	public class SlideSequenceDef : ScriptableObject
	{
		[Serializable]
		public class Slide
		{
			public int SlideIndex;

			[SearchableEnum]
			public LectureMaster.LectureCam CameraIndex;

			public int EventIndex;

			public string text;

			public FVRObject Head;

			public FVRObject Torso;

			public FVRObject Abdomen;
		}

		public List<Slide> Slides;
	}
}
