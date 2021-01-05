using UnityEngine;

namespace FistVR
{
	public class M203_Fore : FVRInteractiveObject
	{
		public enum ForePos
		{
			Forward,
			Mid,
			Rearward
		}

		public M203 launcher;

		public ForePos CurPos = ForePos.Rearward;

		public ForePos LastPos = ForePos.Rearward;

		public Transform Point_Forward;

		public Transform Point_Rearward;

		private float m_handZOffset;

		private float m_curSlideSpeed;

		private float m_slideZ_current;

		private float m_slideZ_heldTarget;

		private float m_slideZ_forward;

		private float m_slideZ_rear;

		public Transform EjectPos;

		protected override void Awake()
		{
			base.Awake();
			m_slideZ_current = base.transform.localPosition.z;
			m_slideZ_forward = Point_Forward.localPosition.z;
			m_slideZ_rear = Point_Rearward.localPosition.z;
		}

		public override void BeginInteraction(FVRViveHand hand)
		{
			base.BeginInteraction(hand);
			m_handZOffset = base.transform.InverseTransformPoint(hand.Input.Pos).z;
		}

		public void UpdateSlide()
		{
			bool flag = false;
			if (base.IsHeld)
			{
				flag = true;
			}
			if (base.IsHeld)
			{
				Vector3 closestValidPoint = GetClosestValidPoint(Point_Forward.position, Point_Rearward.position, m_hand.Input.Pos + -base.transform.forward * m_handZOffset * launcher.transform.localScale.x);
				m_slideZ_heldTarget = launcher.transform.InverseTransformPoint(closestValidPoint).z;
			}
			Vector2 vector = new Vector2(m_slideZ_rear, m_slideZ_forward);
			float value = m_slideZ_current;
			float slideZ_current = m_slideZ_current;
			if (flag)
			{
				slideZ_current = m_slideZ_heldTarget;
				value = Mathf.MoveTowards(m_slideZ_current, slideZ_current, 5f * Time.deltaTime);
			}
			value = Mathf.Clamp(value, vector.x, vector.y);
			if (Mathf.Abs(value - m_slideZ_current) > Mathf.Epsilon)
			{
				m_slideZ_current = value;
				base.transform.localPosition = new Vector3(base.transform.localPosition.x, base.transform.localPosition.y, m_slideZ_current);
			}
			else
			{
				m_curSlideSpeed = 0f;
			}
			ForePos curPos = CurPos;
			curPos = ((!(Mathf.Abs(m_slideZ_current - m_slideZ_forward) < 0.003f)) ? ((!(Mathf.Abs(m_slideZ_current - m_slideZ_rear) < 0.001f)) ? ForePos.Mid : ForePos.Rearward) : ForePos.Forward);
			int curPos2 = (int)CurPos;
			CurPos = (ForePos)Mathf.Clamp((int)curPos, curPos2 - 1, curPos2 + 1);
			if (CurPos == ForePos.Rearward && LastPos != ForePos.Rearward)
			{
				launcher.Chamber.IsAccessible = false;
				CloseEvent();
			}
			else if (CurPos != ForePos.Rearward && LastPos == ForePos.Rearward)
			{
				launcher.Chamber.IsAccessible = true;
				OpenEvent();
			}
			else if (CurPos == ForePos.Forward && LastPos != 0)
			{
				EjectEvent();
			}
			LastPos = CurPos;
		}

		private void CloseEvent()
		{
			launcher.PlayAudioEvent(FirearmAudioEventType.BreachClose);
		}

		private void OpenEvent()
		{
			launcher.PlayAudioEvent(FirearmAudioEventType.BreachOpen);
		}

		private void EjectEvent()
		{
			if (launcher.Chamber.IsFull)
			{
				launcher.Chamber.EjectRound(EjectPos.position, -base.transform.forward, Vector3.zero);
				launcher.PlayAudioEvent(FirearmAudioEventType.MagazineEjectRound);
			}
		}
	}
}
