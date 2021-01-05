using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class ZosigJammer : ZosigQuestManager
	{
		public Text Label;

		public List<ZosigJammerBox> Boxes;

		public List<GameObject> Jamming;

		private float m_checkTick = 0.1f;

		private ZosigGameManager M;

		public string Flag;

		public int ValueWhenDestroyed;

		private bool m_isDisabled;

		public override void Init(ZosigGameManager m)
		{
			M = m;
			if (M.FlagM.GetFlagValue(Flag) > 0)
			{
				for (int i = 0; i < Jamming.Count; i++)
				{
					Jamming[i].SetActive(value: false);
				}
				m_isDisabled = true;
				for (int j = 0; j < Boxes.Count; j++)
				{
					Boxes[j].SetDestroyed();
				}
			}
		}

		private void Awake()
		{
		}

		private void Update()
		{
			m_checkTick -= Time.deltaTime;
			if (m_checkTick <= 0f)
			{
				m_checkTick = 1f;
				Check();
			}
		}

		private void Check()
		{
			int num = 0;
			for (int i = 0; i < Boxes.Count; i++)
			{
				if (Boxes[i].BState == ZosigJammerBox.JammerBoxState.Functioning)
				{
					num++;
				}
			}
			if (num == 0)
			{
				Label.text = "JAMMER OFFLINE";
			}
			else
			{
				Label.text = num + "JAMMERS ONLINE";
			}
			if (num == 0 && !m_isDisabled)
			{
				m_isDisabled = true;
				ShutDown();
			}
		}

		private void ShutDown()
		{
			M.FlagM.SetFlag(Flag, ValueWhenDestroyed);
			for (int i = 0; i < Jamming.Count; i++)
			{
				Jamming[i].SetActive(value: false);
			}
		}
	}
}
