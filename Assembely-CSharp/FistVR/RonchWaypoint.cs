using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class RonchWaypoint : MonoBehaviour
	{
		public List<RonchWaypoint> Neighbors;

		public bool Debug = true;

		[ContextMenu("Neigh")]
		public void Neigh()
		{
			for (int i = 0; i < Neighbors.Count; i++)
			{
				if (!Neighbors[i].Neighbors.Contains(this))
				{
					Neighbors[i].Neighbors.Add(this);
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (Debug && Neighbors.Count > 0)
			{
				for (int i = 0; i < Neighbors.Count; i++)
				{
					Gizmos.color = Color.red;
					Gizmos.DrawLine(base.transform.position, Neighbors[i].transform.position);
				}
			}
		}
	}
}
