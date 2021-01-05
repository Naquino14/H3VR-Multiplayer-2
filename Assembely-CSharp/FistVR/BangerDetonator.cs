using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class BangerDetonator : FVRPhysicalObject
	{
		private List<Banger> m_bangers = new List<Banger>();

		private float m_triggerFloat;

		private float m_lastTriggerFloat;

		public Transform TriggerPiece;

		public Vector2 TriggerRange;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (m_hasTriggeredUpSinceBegin)
			{
				m_triggerFloat = hand.Input.TriggerFloat;
				if (hand.Input.TriggerDown)
				{
					Detonate();
				}
			}
			else
			{
				m_triggerFloat = 0f;
			}
		}

		public override void EndInteraction(FVRViveHand hand)
		{
			base.EndInteraction(hand);
			m_triggerFloat = 0f;
		}

		public void RegisterBanger(Banger b)
		{
			if (!m_bangers.Contains(b))
			{
				m_bangers.Add(b);
			}
		}

		protected override void FVRUpdate()
		{
			base.FVRUpdate();
			if (m_triggerFloat != m_lastTriggerFloat)
			{
				SetAnimatedComponent(TriggerPiece, Mathf.Lerp(TriggerRange.x, TriggerRange.y, m_triggerFloat), InterpStyle.Translate, Axis.Z);
			}
			m_lastTriggerFloat = m_triggerFloat;
		}

		private void Detonate()
		{
			for (int num = m_bangers.Count - 1; num >= 0; num--)
			{
				if (m_bangers[num] != null && m_bangers[num].IsArmed)
				{
					m_bangers[num].StartExploding();
					m_bangers.RemoveAt(num);
				}
			}
			m_bangers.TrimExcess();
		}
	}
}
