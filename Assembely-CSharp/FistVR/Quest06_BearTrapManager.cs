using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class Quest06_BearTrapManager : ZosigQuestManager
	{
		public List<BearTrap> Field1_HBH;

		public List<BearTrap> Field2_Crossroads;

		public List<BearTrap> Field3_Bend;

		public List<BearTrap> Field4_WoodedPass;

		private ZosigGameManager m;

		private bool m_isQuestCompleted;

		public override void Init(ZosigGameManager M)
		{
			m = M;
			InitializeTrapsFromFlagM();
		}

		private void InitializeTrapsFromFlagM()
		{
			if (m.FlagM.GetFlagValue("quest06_state") < 2)
			{
				CloseTraps(Field1_HBH);
				if (m.IsVerboseDebug)
				{
					Debug.Log("HBH Trap field not complete");
				}
			}
			else
			{
				OpenTraps(Field1_HBH);
				if (m.IsVerboseDebug)
				{
					Debug.Log("HBH Trap field already complete");
				}
			}
			if (m.FlagM.GetFlagValue("quest06a_state") < 1)
			{
				CloseTraps(Field2_Crossroads);
				if (m.IsVerboseDebug)
				{
					Debug.Log("Crossroads Trap field not complete");
				}
			}
			else
			{
				OpenTraps(Field2_Crossroads);
				if (m.IsVerboseDebug)
				{
					Debug.Log("Crossroads Trap field already complete");
				}
			}
			if (m.FlagM.GetFlagValue("quest06b_state") < 1)
			{
				CloseTraps(Field3_Bend);
				if (m.IsVerboseDebug)
				{
					Debug.Log("Bend field not complete");
				}
			}
			else
			{
				OpenTraps(Field3_Bend);
				if (m.IsVerboseDebug)
				{
					Debug.Log("Bend field already complete");
				}
			}
			if (m.FlagM.GetFlagValue("quest06c_state") < 1)
			{
				CloseTraps(Field4_WoodedPass);
				if (m.IsVerboseDebug)
				{
					Debug.Log("WoodedPass Trap field not complete");
				}
			}
			else
			{
				OpenTraps(Field4_WoodedPass);
				if (m.IsVerboseDebug)
				{
					Debug.Log("WoodedPass Trap field already complete");
				}
			}
			if (m.FlagM.GetFlagValue("quest06_state") > 2)
			{
				m_isQuestCompleted = true;
				if (m.IsVerboseDebug)
				{
					Debug.Log("Quest Completed Trap field already complete");
				}
			}
		}

		private void OpenTraps(List<BearTrap> t)
		{
			for (int i = 0; i < t.Count; i++)
			{
				t[i].ForceOpen();
			}
		}

		private void CloseTraps(List<BearTrap> t)
		{
			for (int i = 0; i < t.Count; i++)
			{
				t[i].ForceClose();
			}
		}

		private void Update()
		{
			if (!m_isQuestCompleted)
			{
				if (m.FlagM.GetFlagValue("quest06_state") < 2)
				{
					SetCompletedFlagIfAllOpen(Field1_HBH, "quest06_state", 2);
				}
				if (m.FlagM.GetFlagValue("quest06a_state") < 1)
				{
					SetCompletedFlagIfAllOpen(Field2_Crossroads, "quest06a_state", 1);
				}
				if (m.FlagM.GetFlagValue("quest06b_state") < 1)
				{
					SetCompletedFlagIfAllOpen(Field3_Bend, "quest06b_state", 1);
				}
				if (m.FlagM.GetFlagValue("quest06c_state") < 1)
				{
					SetCompletedFlagIfAllOpen(Field4_WoodedPass, "quest06c_state", 1);
				}
				if (m.FlagM.GetFlagValue("quest06_state") == 2 && m.FlagM.GetFlagValue("quest06a_state") == 1 && m.FlagM.GetFlagValue("quest06b_state") == 1 && m.FlagM.GetFlagValue("quest06c_state") == 1)
				{
					m.FlagM.SetFlagMaxBlend("quest06_state", 3);
					m_isQuestCompleted = true;
				}
			}
		}

		private void SetCompletedFlagIfAllOpen(List<BearTrap> t, string flag, int successValue)
		{
			bool flag2 = true;
			for (int i = 0; i < t.Count; i++)
			{
				if (!t[i].IsOpen())
				{
					flag2 = false;
					break;
				}
			}
			if (flag2)
			{
				m.FlagM.SetFlagMaxBlend(flag, successValue);
				if (m.IsVerboseDebug)
				{
					Debug.Log(flag + "Set to 2 because testing list is all open");
				}
			}
		}
	}
}
