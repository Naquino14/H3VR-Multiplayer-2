using UnityEngine;

namespace FistVR
{
	public class Dumple : MonoBehaviour, IFVRDamageable
	{
		public enum DumpleState
		{
			Static,
			Scanning,
			Wandering,
			Combat
		}

		public DumpleState State = DumpleState.Scanning;

		public AIEntity E;

		public AITargetPrioritySystem Priority;

		private bool m_hasPriority;

		[Header("Refs")]
		public Transform Eye;

		[Header("DamagePoints")]
		public Transform DP_Eye;

		public Transform DP_GunLeft;

		public Transform DP_GunRight;

		private void Start()
		{
			E.AIEventReceiveEvent += EventReceive;
		}

		public void EventReceive(AIEvent e)
		{
			if (State == DumpleState.Static || (e.IsEntity && e.Entity.IFFCode == E.IFFCode))
			{
				return;
			}
			if (e.Type == AIEvent.AIEType.Damage)
			{
				if (State == DumpleState.Scanning || State == DumpleState.Wandering)
				{
					SetState(DumpleState.Combat);
				}
			}
			else if (e.Type == AIEvent.AIEType.Visual && m_hasPriority)
			{
				Priority.ProcessEvent(e);
				SetState(DumpleState.Combat);
			}
		}

		private void SetState(DumpleState s)
		{
			State = s;
			switch (State)
			{
			case DumpleState.Static:
				UpdateState_Static();
				break;
			case DumpleState.Scanning:
				UpdateState_Scanning();
				break;
			case DumpleState.Wandering:
				UpdateState_Wandering();
				break;
			case DumpleState.Combat:
				UpdateState_Combat();
				break;
			}
		}

		private void Update()
		{
		}

		private void UpdateState_Static()
		{
		}

		private void UpdateState_Scanning()
		{
		}

		private void UpdateState_Wandering()
		{
		}

		private void UpdateState_Combat()
		{
		}

		public void Damage(Damage d)
		{
		}
	}
}
