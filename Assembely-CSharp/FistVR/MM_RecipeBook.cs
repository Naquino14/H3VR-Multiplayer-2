using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class MM_RecipeBook : MonoBehaviour
	{
		[Serializable]
		public class ImageList
		{
			public bool ImagesActive;

			public List<Image> RevealThese;
		}

		public List<ImageList> ResourceLists;

		private float UpdateTick = 1f;

		private void Start()
		{
		}

		private void Update()
		{
			UpdateTick -= Time.deltaTime;
			if (!(UpdateTick <= 0f))
			{
				return;
			}
			UpdateTick = UnityEngine.Random.Range(0.3f, 1f);
			for (int i = 0; i < ResourceLists.Count; i++)
			{
				if (!ResourceLists[i].ImagesActive && GM.MMFlags.MMMisKnown[i])
				{
					ResourceLists[i].ImagesActive = true;
					for (int j = 0; j < ResourceLists[i].RevealThese.Count; j++)
					{
						ResourceLists[i].RevealThese[j].gameObject.SetActive(value: true);
					}
				}
			}
		}
	}
}
