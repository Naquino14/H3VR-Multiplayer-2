using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigQuestAggregator : ZosigQuestManager
	{
		[Serializable]
		public class ZQuestRequirement
		{
			public string QuestFlag;

			public int ValueNeeded = 2;
		}

		public string QuestFlagToSet;

		public int QuestFlagValueBeneath = 2;

		public int QuestFlagToSetToWhenDone = 2;

		public List<ZQuestRequirement> RequiredQuests;

		private bool m_isQuestCompleted;

		private ZosigGameManager m;

		private float checkTick = 1f;

		public override void Init(ZosigGameManager M)
		{
			m = M;
			InitializeTrapsFromFlagM();
		}

		private void InitializeTrapsFromFlagM()
		{
			if (m.FlagM.GetFlagValue(QuestFlagToSet) >= QuestFlagToSetToWhenDone)
			{
				m_isQuestCompleted = true;
				if (m.IsVerboseDebug)
				{
					Debug.Log(QuestFlagToSet + " already detected as being done, setting aggregator to finished");
				}
			}
			else if (m.IsVerboseDebug)
			{
				Debug.Log(QuestFlagToSet + " is not done yet, setting aggregator to not finished");
			}
		}

		private void Update()
		{
			if (!m_isQuestCompleted)
			{
				checkTick -= Time.deltaTime;
				if (checkTick <= 0f)
				{
					checkTick = 1f;
					CheckQuestState();
				}
			}
		}

		private void CheckQuestState()
		{
			bool flag = true;
			for (int i = 0; i < RequiredQuests.Count; i++)
			{
				if (m.FlagM.GetFlagValue(RequiredQuests[i].QuestFlag) < RequiredQuests[i].ValueNeeded)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				m_isQuestCompleted = true;
				if (m.IsVerboseDebug)
				{
					Debug.Log(QuestFlagToSet + " has been completed! setting aggregator to finished");
				}
				m.FlagM.SetFlag(QuestFlagToSet, QuestFlagToSetToWhenDone);
				GM.ZMaster.FlagM.Save();
			}
		}
	}
}
