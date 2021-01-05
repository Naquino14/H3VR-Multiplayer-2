using System;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Sequence Library Def", menuName = "OmniSequencer/Libraries/Sequence Library", order = 0)]
	public class OmniSequenceLibrary : ScriptableObject
	{
		[Serializable]
		public class SequenceLibraryTheme
		{
			public OmniSequencerSequenceDefinition.OmniSequenceTheme Theme;

			public Sprite Sprite;

			public OmniSequencerSequenceDefinition[] SequenceList;

			[Multiline(16)]
			public string ThemeDetails;
		}

		public SequenceLibraryTheme[] Themes;
	}
}
