using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class PUM : ManagerSingleton<PUM>
	{
		public List<GameObject> PowerupVFX_Player_Positive;

		public List<GameObject> PowerupVFX_Player_Negative;

		public List<GameObject> PowerupVFXByIndex_Bot_Positive;

		public List<GameObject> PowerupVFXByIndex_Bot_Negative;

		public List<GameObject> PowerupCloud;

		public GameObject Sosig_Barfer;

		public GameObject Sosig_Cyclops;

		public GameObject Sosig_Biclops;

		protected override void Awake()
		{
			base.Awake();
		}

		public static bool HasEffectBot(int index, bool isInverted)
		{
			if (isInverted)
			{
				if (ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Negative.Count < index + 1 || ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Negative[index] == null)
				{
					return false;
				}
				return true;
			}
			if (ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Positive.Count < index + 1 || ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Positive[index] == null)
			{
				return false;
			}
			return true;
		}

		public static bool HasEffectPlayer(int index, bool isInverted)
		{
			if (isInverted)
			{
				if (index > ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Negative.Count - 1 || ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Negative[index] == null)
				{
					return false;
				}
				return true;
			}
			if (index > ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Positive.Count - 1 || ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Positive[index] == null)
			{
				return false;
			}
			return true;
		}

		public static GameObject GetEffect(int index, bool isInverted)
		{
			if (isInverted)
			{
				return ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Negative[index];
			}
			return ManagerSingleton<PUM>.Instance.PowerupVFXByIndex_Bot_Positive[index];
		}

		public static GameObject GetEffectPlayer(int index, bool isInverted)
		{
			if (isInverted)
			{
				return ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Negative[index];
			}
			return ManagerSingleton<PUM>.Instance.PowerupVFX_Player_Positive[index];
		}
	}
}
