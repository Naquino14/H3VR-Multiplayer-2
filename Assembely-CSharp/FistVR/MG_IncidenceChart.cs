using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Definition", menuName = "MeatGrinder/IncidenceChart", order = 0)]
	public class MG_IncidenceChart : ScriptableObject
	{
		public List<MGItemSpawnChartEntry> Chart;

		public float TotalWeight;

		public MGItemSpawnChartEntry GetWeightedRandomEntry()
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
			float num = 0f;
			for (int i = 0; i < Chart.Count; i++)
			{
				Chart[i].FinalWeight = Chart[i].Incidence + num;
				num += Chart[i].Incidence;
				TotalWeight += Chart[i].Incidence;
			}
		}
	}
}
