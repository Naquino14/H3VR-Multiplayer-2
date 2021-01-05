using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigQuestDisplay : ZosigQuestManager
	{
		[Serializable]
		public class DisplayItem
		{
			public GameObject ObjectToShow;

			public string flagNeeded;

			public int valueNeeded;
		}

		private ZosigGameManager m;

		public List<DisplayItem> Display;

		private float checkTick = 1f;

		public override void Init(ZosigGameManager M)
		{
			m = M;
		}

		private void Update()
		{
			checkTick -= Time.deltaTime;
			if (checkTick <= 0f)
			{
				checkTick = 1f;
				UpdateList();
			}
		}

		private void UpdateList()
		{
			for (int i = 0; i < Display.Count; i++)
			{
				if (m.FlagM.GetFlagValue(Display[i].flagNeeded) >= Display[i].valueNeeded)
				{
					Display[i].ObjectToShow.SetActive(value: true);
				}
				else
				{
					Display[i].ObjectToShow.SetActive(value: false);
				}
			}
		}
	}
}
