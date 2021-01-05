using UnityEngine;

namespace FistVR
{
	public class MF_CapturePoint : MonoBehaviour
	{
		public enum MF_CapturePointState
		{
			Unlocked,
			Locked,
			Owned
		}

		private MF_Zone m_zone;

		private bool m_isCapturePointActive;

		public MF_CapturePointState State_RedTeam;

		public MF_CapturePointState State_BlueTeam;

		private int m_iffRed;

		private int m_iffBlue = 1;

		private float m_captureScaleRed;

		private float m_captureScaleBlue;

		public void SetZone(MF_Zone z)
		{
			m_zone = z;
		}

		public MF_Zone GetZone()
		{
			return m_zone;
		}

		public void InitState(MF_TeamColor c, MF_CapturePointState s)
		{
			if (c == MF_TeamColor.Red)
			{
				State_RedTeam = s;
			}
			else
			{
				State_BlueTeam = s;
			}
			if (c == MF_TeamColor.Red)
			{
				if (s == MF_CapturePointState.Unlocked || s == MF_CapturePointState.Locked)
				{
					m_captureScaleRed = 0f;
				}
				else
				{
					m_captureScaleRed = 1f;
				}
			}
			if (c == MF_TeamColor.Blue)
			{
				if (s == MF_CapturePointState.Unlocked || s == MF_CapturePointState.Locked)
				{
					m_captureScaleBlue = 0f;
				}
				else
				{
					m_captureScaleBlue = 1f;
				}
			}
		}

		public MF_CapturePointState GetState(MF_TeamColor c)
		{
			if (c == MF_TeamColor.Red)
			{
				return State_RedTeam;
			}
			return State_BlueTeam;
		}

		public void Tick(float t)
		{
		}
	}
}
