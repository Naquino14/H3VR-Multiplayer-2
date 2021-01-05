using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	[CreateAssetMenu(fileName = "New Melee Anim Set", menuName = "Sosig/MeleeAnimationSet", order = 0)]
	public class SosigMeleeAnimationSet : ScriptableObject
	{
		public List<Vector3> Frames_LocalPos;

		public List<Vector3> Frames_LocalForward;

		public List<Vector3> Frames_LocalUp;

		private Vector3 GetLerp(List<Vector3> v, float f, bool doEXP, bool loop)
		{
			if (v.Count == 0)
			{
				Debug.Log("Frame list is 0 wtf");
			}
			f = Mathf.Clamp(f, 0f, 1f);
			if (doEXP)
			{
				f = Mathf.Pow(f, 2f);
			}
			int value = Mathf.FloorToInt(f / 1f * (float)v.Count);
			value = Mathf.Clamp(value, 0, v.Count - 1);
			if (value < 0)
			{
				value = 0;
			}
			int num = value + 1;
			if (num >= v.Count)
			{
				num = ((!loop) ? (v.Count - 1) : 0);
			}
			Vector3 a = v[value];
			Vector3 b = v[num];
			float num2 = 1f / (float)v.Count;
			float t = f % num2 * (float)v.Count;
			return Vector3.Slerp(a, b, t);
		}

		public Vector3 GetPos(float f, bool doEXP, bool loop)
		{
			return GetLerp(Frames_LocalPos, f, doEXP, loop);
		}

		public Vector3 GetForward(float f, bool doEXP, bool loop)
		{
			return GetLerp(Frames_LocalForward, f, doEXP, loop);
		}

		public Vector3 GetUp(float f, bool doEXP, bool loop)
		{
			return GetLerp(Frames_LocalUp, f, doEXP, loop);
		}
	}
}
