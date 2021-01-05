using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ZosigQuestShowOnFlagList : MonoBehaviour
	{
		[Serializable]
		public class ShowByFlag
		{
			public GameObject Show;

			public string Flag;

			public int ValueEqualOrAbove;

			public bool IsRevealed;
		}

		public List<ShowByFlag> ShowByFlags;

		private float checkTick = 1f;

		private void Start()
		{
			for (int i = 0; i < ShowByFlags.Count; i++)
			{
				ShowByFlags[i].Show.SetActive(value: false);
			}
		}

		private void Update()
		{
			if (checkTick > 0f)
			{
				checkTick -= Time.deltaTime;
				return;
			}
			checkTick = UnityEngine.Random.Range(1f, 1.4f);
			for (int i = 0; i < ShowByFlags.Count; i++)
			{
				if (!ShowByFlags[i].IsRevealed && GM.ZMaster.FlagM.GetFlagValue(ShowByFlags[i].Flag) >= ShowByFlags[i].ValueEqualOrAbove)
				{
					ShowByFlags[i].IsRevealed = true;
					if (!ShowByFlags[i].Show.activeSelf)
					{
						ShowByFlags[i].Show.SetActive(value: true);
					}
				}
			}
		}
	}
}
