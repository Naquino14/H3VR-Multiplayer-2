using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New SafePosMatrix Def", menuName = "TNH/SafePosMatrix", order = 0)]
	public class TNH_SafePositionMatrix : ScriptableObject
	{
		[Serializable]
		public class PositionEntry
		{
			public List<bool> SafePositions_HoldPoints = new List<bool>();

			public List<bool> SafePositions_SupplyPoints = new List<bool>();
		}

		public List<PositionEntry> Entries_HoldPoints;

		public List<PositionEntry> Entries_SupplyPoints;

		[ContextMenu("GenereateSupplyPointsData")]
		public void GenereateSupplyPointsData()
		{
			Entries_SupplyPoints.Clear();
			int count = Entries_HoldPoints.Count;
			int count2 = Entries_HoldPoints[0].SafePositions_SupplyPoints.Count;
			for (int i = 0; i < count2; i++)
			{
				PositionEntry positionEntry = new PositionEntry();
				for (int j = 0; j < count; j++)
				{
					bool item = Entries_HoldPoints[j].SafePositions_SupplyPoints[i];
					positionEntry.SafePositions_HoldPoints.Add(item);
				}
				Entries_SupplyPoints.Add(positionEntry);
			}
		}

		[ContextMenu("Fix1")]
		public void Fix1()
		{
			for (int i = 0; i < Entries_SupplyPoints.Count; i++)
			{
				for (int j = 0; j < Entries_SupplyPoints[i].SafePositions_SupplyPoints.Count; j++)
				{
					Entries_SupplyPoints[i].SafePositions_SupplyPoints[j] = true;
				}
			}
		}

		[ContextMenu("Fix2")]
		public void Fix2()
		{
			for (int i = 0; i < Entries_HoldPoints.Count; i++)
			{
				for (int j = 0; j < Entries_HoldPoints[i].SafePositions_HoldPoints.Count; j++)
				{
					if (i == j)
					{
						Entries_HoldPoints[i].SafePositions_HoldPoints[j] = false;
					}
				}
			}
			for (int k = 0; k < Entries_SupplyPoints.Count; k++)
			{
				for (int l = 0; l < Entries_SupplyPoints[k].SafePositions_SupplyPoints.Count; l++)
				{
					if (k == l)
					{
						Entries_SupplyPoints[k].SafePositions_SupplyPoints[l] = false;
					}
				}
			}
		}
	}
}
