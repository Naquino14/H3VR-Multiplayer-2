using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New PD Hierarchy Config", menuName = "PancakeDrone/Hierarchy Definition", order = 0)]
	public class PDHierarchyConfig : ScriptableObject
	{
		[Serializable]
		public class PDChildren
		{
			public string name;

			public PDComponentID ID;

			[SearchableEnum]
			public List<PDComponentID> Children;

			public void SetName(string s)
			{
				name = s;
			}
		}

		public List<PDChildren> IDs = new List<PDChildren>();

		[ContextMenu("Prime")]
		public void Prime()
		{
			IDs.Clear();
			for (int i = 1; i < 255; i++)
			{
				if (Enum.IsDefined(typeof(PDComponentID), i))
				{
					PDChildren pDChildren = new PDChildren();
					pDChildren.ID = (PDComponentID)i;
					PDComponentID pDComponentID = (PDComponentID)i;
					pDChildren.SetName(pDComponentID.ToString());
					IDs.Add(pDChildren);
				}
			}
		}
	}
}
