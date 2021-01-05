using System;

namespace FistVR
{
	[Serializable]
	public class InputOverrider
	{
		public enum OverriderType
		{
			Trigger
		}

		private FVRViveHand m_hand;

		public OverriderType Type;

		private bool m_triggerPressed;

		private bool m_triggerUp;

		private bool m_triggerDown;

		private float m_triggerFloat;

		public bool Real_triggerPressed;

		public bool Real_triggerUp;

		public bool Real_triggerDown;

		public float Real_triggerFloat;

		public void ConnectToHand(FVRViveHand h)
		{
			m_hand = h;
			m_hand.SetOverrider(this);
		}

		public void FlushHandConnection()
		{
			if (m_hand != null)
			{
				m_hand.FlushOverrideIfThis(this);
			}
			m_hand = null;
		}

		public void UpdateTrigger(bool m_pressed)
		{
			if (m_pressed && !m_triggerPressed)
			{
				m_triggerDown = true;
			}
			if (!m_pressed && m_triggerPressed)
			{
				m_triggerUp = true;
			}
			m_triggerPressed = m_pressed;
			if (m_pressed)
			{
				m_triggerFloat = 1f;
			}
			else
			{
				m_triggerFloat = 0f;
			}
		}

		public void Process(ref HandInput i)
		{
			if (Type == OverriderType.Trigger)
			{
				Real_triggerPressed = i.TriggerPressed;
				Real_triggerUp = i.TriggerUp;
				Real_triggerDown = i.TriggerDown;
				Real_triggerFloat = i.TriggerFloat;
				i.TriggerDown = m_triggerDown;
				i.TriggerUp = m_triggerUp;
				i.TriggerPressed = m_triggerPressed;
				i.TriggerFloat = m_triggerFloat;
			}
		}
	}
}
