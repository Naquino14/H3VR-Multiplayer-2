using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Loot Table", menuName = "TakeAndHold/LootTable", order = 0)]
	public class TAH_LootTable : ScriptableObject
	{
		public float TotalWeight;

		public List<TAH_LootTableEntry> Chart;

		public TAH_LootTableEntry GetWeightedRandomEntry()
		{
			float num = Random.Range(0f, TotalWeight);
			int num2 = -1;
			for (int i = 0; i < Chart.Count; i++)
			{
				if (num < Chart[i].Nums.y)
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
				Chart[i].Nums.y = 0f;
			}
			float num = 0f;
			for (int j = 0; j < Chart.Count; j++)
			{
				Chart[j].Nums.y = Chart[j].Nums.x + num;
				num += Chart[j].Nums.x;
				TotalWeight += Chart[j].Nums.x;
			}
		}
	}
}
