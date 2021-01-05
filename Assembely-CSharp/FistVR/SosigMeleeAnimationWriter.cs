using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class SosigMeleeAnimationWriter : MonoBehaviour
	{
		public Transform Root;

		public bool ShowGizmos = true;

		public bool IsShieldGizmo;

		public float WeaponLength_Forward = 0.1f;

		public float WeaponLength_Behind = 0.1f;

		public Vector3 ShieldDimensions;

		public bool usesForwardBit = true;

		public float WeaponLength_ForwardBit = 0.1f;

		public Color AnimColor_Start;

		public Color AnimColor_End;

		public List<SosigMeleeAnimationFrame> Frames;

		public SosigMeleeAnimationSet SetToWriteTo;

		public bool IsAnimTesting;

		private float m_animTestFloat;

		private float m_testanimSpeed = 1f;

		public Color GetInterpolatedColor(int index)
		{
			return Color.Lerp(AnimColor_Start, AnimColor_End, (float)index / (float)Frames.Count);
		}

		public Color GetInterpolatedColor(float f)
		{
			return Color.Lerp(AnimColor_Start, AnimColor_End, f);
		}

		public void Update()
		{
			m_animTestFloat += Time.deltaTime * m_testanimSpeed;
			m_animTestFloat = Mathf.Repeat(m_animTestFloat, 1f);
		}

		public void OnDrawGizmos()
		{
			if (Frames.Count > 0 && IsAnimTesting)
			{
				Color interpolatedColor = GetInterpolatedColor(m_animTestFloat);
				Vector3 vector = Root.TransformPoint(SetToWriteTo.GetPos(m_animTestFloat, doEXP: true, loop: true));
				Vector3 vector2 = Root.TransformDirection(SetToWriteTo.GetForward(m_animTestFloat, doEXP: true, loop: true));
				Vector3 vector3 = Root.TransformDirection(SetToWriteTo.GetUp(m_animTestFloat, doEXP: true, loop: true));
				Gizmos.color = new Color(interpolatedColor.r, interpolatedColor.g, interpolatedColor.b, 1f);
				Gizmos.DrawSphere(vector, 0.025f);
				Gizmos.DrawLine(vector, vector + vector2 * WeaponLength_Forward);
				Gizmos.DrawLine(vector, vector - vector2 * WeaponLength_Behind);
				if (usesForwardBit)
				{
					Gizmos.DrawLine(vector + vector2 * WeaponLength_Forward, vector + vector2 * WeaponLength_Forward - vector3 * WeaponLength_ForwardBit);
				}
			}
		}

		[ContextMenu("SetIndicies")]
		public void SetIndicies()
		{
			for (int i = 0; i < Frames.Count; i++)
			{
				Frames[i].Index = i;
			}
		}

		[ContextMenu("Write")]
		public void Write()
		{
			SetToWriteTo.Frames_LocalPos.Clear();
			SetToWriteTo.Frames_LocalForward.Clear();
			SetToWriteTo.Frames_LocalUp.Clear();
			for (int i = 0; i < Frames.Count; i++)
			{
				Transform transform = Frames[i].transform;
				SetToWriteTo.Frames_LocalPos.Add(transform.localPosition);
				SetToWriteTo.Frames_LocalForward.Add(transform.forward);
				SetToWriteTo.Frames_LocalUp.Add(transform.up);
			}
		}
	}
}
