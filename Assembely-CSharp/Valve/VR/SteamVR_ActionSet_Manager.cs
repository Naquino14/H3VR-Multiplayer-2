using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Valve.VR
{
	public static class SteamVR_ActionSet_Manager
	{
		public static VRActiveActionSet_t[] rawActiveActionSetArray;

		[NonSerialized]
		private static uint activeActionSetSize;

		private static bool changed;

		private static int lastFrameUpdated;

		public static string debugActiveSetListText;

		public static bool updateDebugTextInBuilds;

		public static void Initialize()
		{
			activeActionSetSize = (uint)Marshal.SizeOf(typeof(VRActiveActionSet_t));
		}

		public static void DisableAllActionSets()
		{
			for (int i = 0; i < SteamVR_Input.actionSets.Length; i++)
			{
				SteamVR_Input.actionSets[i].Deactivate();
				SteamVR_Input.actionSets[i].Deactivate(SteamVR_Input_Sources.LeftHand);
				SteamVR_Input.actionSets[i].Deactivate(SteamVR_Input_Sources.RightHand);
			}
		}

		public static void UpdateActionStates(bool force = false)
		{
			if (!force && Time.frameCount == lastFrameUpdated)
			{
				return;
			}
			lastFrameUpdated = Time.frameCount;
			if (changed)
			{
				UpdateActionSetsArray();
			}
			if (rawActiveActionSetArray != null && rawActiveActionSetArray.Length > 0)
			{
				EVRInputError eVRInputError = OpenVR.Input.UpdateActionState(rawActiveActionSetArray, activeActionSetSize);
				if (eVRInputError != 0)
				{
					Debug.LogError("<b>[SteamVR]</b> UpdateActionState error: " + eVRInputError);
				}
			}
		}

		public static void SetChanged()
		{
			changed = true;
		}

		private static void UpdateActionSetsArray()
		{
			List<VRActiveActionSet_t> list = new List<VRActiveActionSet_t>();
			SteamVR_Input_Sources[] allSources = SteamVR_Input_Source.GetAllSources();
			for (int i = 0; i < SteamVR_Input.actionSets.Length; i++)
			{
				SteamVR_ActionSet steamVR_ActionSet = SteamVR_Input.actionSets[i];
				foreach (SteamVR_Input_Sources inputSource in allSources)
				{
					if (steamVR_ActionSet.ReadRawSetActive(inputSource))
					{
						VRActiveActionSet_t item = default(VRActiveActionSet_t);
						item.ulActionSet = steamVR_ActionSet.handle;
						item.nPriority = steamVR_ActionSet.ReadRawSetPriority(inputSource);
						item.ulRestrictedToDevice = SteamVR_Input_Source.GetHandle(inputSource);
						int num = 0;
						for (num = 0; num < list.Count && list[num].nPriority <= item.nPriority; num++)
						{
						}
						list.Insert(num, item);
					}
				}
			}
			changed = false;
			rawActiveActionSetArray = list.ToArray();
			if (Application.isEditor || updateDebugTextInBuilds)
			{
				UpdateDebugText();
			}
		}

		public static SteamVR_ActionSet GetSetFromHandle(ulong handle)
		{
			for (int i = 0; i < SteamVR_Input.actionSets.Length; i++)
			{
				SteamVR_ActionSet steamVR_ActionSet = SteamVR_Input.actionSets[i];
				if (steamVR_ActionSet.handle == handle)
				{
					return steamVR_ActionSet;
				}
			}
			return null;
		}

		private static void UpdateDebugText()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < rawActiveActionSetArray.Length; i++)
			{
				VRActiveActionSet_t vRActiveActionSet_t = rawActiveActionSetArray[i];
				stringBuilder.Append(vRActiveActionSet_t.nPriority);
				stringBuilder.Append("\t");
				stringBuilder.Append(SteamVR_Input_Source.GetSource(vRActiveActionSet_t.ulRestrictedToDevice));
				stringBuilder.Append("\t");
				stringBuilder.Append(GetSetFromHandle(vRActiveActionSet_t.ulActionSet).GetShortName());
				stringBuilder.Append("\n");
			}
			debugActiveSetListText = stringBuilder.ToString();
		}
	}
}
