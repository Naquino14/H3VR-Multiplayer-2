using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Loot Table", menuName = "SousVie/LootTable", order = 0)]
	public class SV_LootTable : ScriptableObject
	{
		public float TotalWeight;

		public List<SV_LootTableEntry> Chart;

		public SV_LootTableEntry GetWeightedRandomEntry()
		{
			float num = Random.Range(0f, TotalWeight);
			int num2 = -1;
			for (int i = 0; i < Chart.Count; i++)
			{
				if (num < Chart[i].FinalWeight)
				{
					num2 = i;
					break;
				}
			}
			if (num2 < 0)
			{
				num2 = 0;
			}
			return Chart[num2];
		}

		[ContextMenu("CalcWeights")]
		public void CalcWeights()
		{
			for (int i = 0; i < Chart.Count; i++)
			{
				Chart[i].FinalWeight = 0f;
			}
			float num = 0f;
			for (int j = 0; j < Chart.Count; j++)
			{
				Chart[j].FinalWeight = Chart[j].Incidence + num;
				num += Chart[j].Incidence;
				TotalWeight += Chart[j].Incidence;
			}
		}
	}
}
