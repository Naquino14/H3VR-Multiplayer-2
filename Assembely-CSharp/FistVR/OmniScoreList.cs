using System;
using System.Collections.Generic;

namespace FistVR
{
	[Serializable]
	public class OmniScoreList
	{
		public string SequenceID = string.Empty;

		public List<OmniScore> Scores = new List<OmniScore>();

		public int Trophy = 3;
	}
}
