using System;
using System.Collections.Generic;

namespace FistVR
{
	[Serializable]
	public class MF_ZoneMeta : IComparable<MF_ZoneMeta>
	{
		public MF_Zone Zone;

		public MF_ZoneType Type;

		public float Neccesity;

		private List<MF_Squad> m_assignedSquads = new List<MF_Squad>();

		public void AssignSquad(MF_Squad s)
		{
			m_assignedSquads.Add(s);
		}

		public void DeAssignSquad(MF_Squad s)
		{
			if (m_assignedSquads.Contains(s))
			{
				m_assignedSquads.Remove(s);
			}
		}

		public int GetNumAssignedSquads()
		{
			return m_assignedSquads.Count;
		}

		public int CompareTo(MF_ZoneMeta other)
		{
			if (other == null)
			{
				return 1;
			}
			return Neccesity.CompareTo(other.Neccesity);
		}
	}
}
