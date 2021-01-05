using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "MeatGrinder/EventAIConfig", order = 0)]
	public class MG_EventAIConfig : ScriptableObject
	{
		public float TotalWeight;

		public MeatGrinderMaster.EventAI.EventAIMood Mood;

		public List<MeatGrinderMaster.EventAI.MGEvent> EventList;

		public MeatGrinderMaster.EventAI.MGEvent GetWeightedRandomEntry()
		{
			float num = Random.Range(0f, TotalWeight);
			int num2 = -1;
			for (int i = 0; i < EventList.Count; i++)
			{
				if (num < EventList[i].FinalWeight)
				{
					num2 = i;
					break;
				}
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			return EventList[num2];
		}

		[ContextMenu("CalcWeights")]
		public void CalcWeights()
		{
			float num = 0f;
			TotalWeight = 0f;
			for (int i = 0; i < EventList.Count; i++)
			{
				EventList[i].FinalWeight = EventList[i].Incidence + num;
				num += EventList[i].Incidence;
				TotalWeight += EventList[i].Incidence;
			}
		}
	}
}
