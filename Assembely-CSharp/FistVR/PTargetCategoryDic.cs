using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New TNH_CharacterDatabase", menuName = "PaperTarget/CategorySet", order = 0)]
	public class PTargetCategoryDic : ScriptableObject
	{
		[Serializable]
		public class PTCat
		{
			public string Name;

			public Sprite CatImage;

			public List<Sprite> TargetIcons;

			public List<FVRObject> Targets;
		}

		public List<PTCat> Cats;

		[ContextMenu("SetIcons")]
		public void SetIcons()
		{
			for (int i = 0; i < Cats.Count; i++)
			{
				Cats[i].TargetIcons.Clear();
				for (int j = 0; j < Cats[i].Targets.Count; j++)
				{
					Cats[i].TargetIcons.Add(Cats[i].Targets[j].GetGameObject().GetComponent<PTargetReferenceHolder>().Profile.displayIcon);
				}
			}
		}
	}
}
