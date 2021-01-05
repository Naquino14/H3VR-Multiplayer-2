using System;
using UnityEngine;

namespace FistVR
{
	[Serializable]
	public class AIEvent : IComparable<AIEvent>
	{
		public enum AIEType
		{
			None,
			Visual,
			Sonic,
			Damage
		}

		public float TimeSinceSensed;

		public float TimeSeen;

		public bool IsEntity;

		public AIEntity Entity;

		public Vector3 Pos;

		public AIEType Type;

		public float Value;

		public AIEvent(AIEntity e, AIEType t, float v)
		{
			Entity = e;
			IsEntity = true;
			Pos = e.GetPos();
			Type = t;
			Value = v;
		}

		public AIEvent(Vector3 p, AIEType t, float v)
		{
			IsEntity = false;
			Pos = p;
			Type = t;
			Value = v;
		}

		public Vector3 UpdatePos()
		{
			if (IsEntity)
			{
				Pos = Entity.GetPos();
			}
			return Pos;
		}

		public void Tick()
		{
			TimeSinceSensed += Time.deltaTime;
			TimeSeen += Time.deltaTime;
		}

		public void UpdateFrom(AIEvent e)
		{
			Pos = e.Pos;
			Type = e.Type;
			Value = e.Value;
			TimeSinceSensed = 0f;
		}

		public void Dispose()
		{
			Entity = null;
			IsEntity = false;
			Type = AIEType.None;
		}

		public int CompareTo(AIEvent other)
		{
			if (other == null)
			{
				return 1;
			}
			return Value.CompareTo(other.Value);
		}
	}
}
