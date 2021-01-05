using UnityEngine;

namespace FistVR
{
	public class DropTrap : ZosigQuestManager
	{
		private ZosigGameManager M;

		public string Flag;

		public int ValueWhenOn;

		private bool m_isEnabled;

		public Transform Cable;

		public Rigidbody Logs;

		public Transform LogTarget;

		private bool m_isGassed;

		public override void Init(ZosigGameManager m)
		{
			M = m;
		}

		private void Start()
		{
		}

		public void TurnOn()
		{
			m_isGassed = true;
		}

		public void TurnOff()
		{
			m_isGassed = false;
		}

		private void Update()
		{
			if (m_isEnabled && m_isGassed)
			{
				float y = Logs.transform.position.y;
				float num = Mathf.MoveTowards(y, LogTarget.position.y, Time.deltaTime * 0.25f);
				if (Mathf.Abs(y - num) > 0.001f)
				{
					Vector3 position = new Vector3(Logs.transform.position.x, num, Logs.transform.position.z);
					Logs.MovePosition(position);
				}
			}
			float z = Mathf.Max((Cable.transform.position - Logs.transform.position).magnitude, 0.01f);
			Cable.localScale = new Vector3(0.03f, 0.03f, z);
		}

		public void ON()
		{
			if (!m_isEnabled && GM.ZMaster != null)
			{
				GM.ZMaster.FlagM.AddToFlag("s_t", 1);
			}
			M.FlagM.SetFlag(Flag, ValueWhenOn);
			m_isEnabled = true;
			Logs.isKinematic = true;
		}

		public void OFF()
		{
			m_isEnabled = false;
			Logs.isKinematic = false;
			Logs.velocity = Vector3.up * -9.81f;
		}
	}
}
