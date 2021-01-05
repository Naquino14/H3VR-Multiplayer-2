using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New PointSequence", menuName = "PointSequence", order = 0)]
	public class TNH_PointSequence : ScriptableObject
	{
		public int StartSupplyPointIndex;

		public List<int> HoldPoints;
	}
}
