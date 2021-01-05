using UnityEngine;

namespace FistVR
{
	public class SpinningBladeTrap : ZosigQuestManager
	{
		public HingeJoint Hinge;

		private bool m_isRunning;

		private float curPower;

		private float tarPower;

		public float MotorForce = 10f;

		public float TargSpeed = -100f;

		public AudioSource Aud;

		private ZosigGameManager M;

		public string Flag;

		public int ValueWhenOn;

		private bool m_isGassed;

		private bool isJustMotor;

		public void TurnOn()
		{
			m_isGassed = true;
		}

		public void TurnOff()
		{
			m_isGassed = false;
		}

		public override void Init(ZosigGameManager m)
		{
			M = m;
		}

		private void Start()
		{
			if (Hinge == null)
			{
				isJustMotor = true;
			}
			else
			{
				Hinge.gameObject.GetComponent<Rigidbody>().maxAngularVelocity = 50f;
			}
		}

		private void Update()
		{
			if (m_isRunning && m_isGassed)
			{
				tarPower = 1f;
			}
			else
			{
				tarPower = 0f;
			}
			curPower = Mathf.MoveTowards(curPower, tarPower, Time.deltaTime * 0.25f);
			if (curPower > 0f)
			{
				if (Aud.isPlaying)
				{
					Aud.volume = curPower * 0.4f;
				}
				else
				{
					Aud.volume = curPower * 0.4f;
					Aud.Play();
				}
			}
			else if (Aud.isPlaying)
			{
				Aud.Stop();
			}
			if (!isJustMotor)
			{
				if (m_isRunning)
				{
					JointMotor motor = Hinge.motor;
					motor.targetVelocity = TargSpeed;
					motor.force = curPower * MotorForce;
					Hinge.motor = motor;
				}
				else
				{
					JointMotor motor2 = Hinge.motor;
					motor2.force = 0f;
					motor2.targetVelocity = 0f;
					Hinge.motor = motor2;
				}
			}
		}

		public void ON()
		{
			if (!m_isRunning)
			{
				if (GM.ZMaster != null)
				{
					GM.ZMaster.FlagM.AddToFlag("s_t", 1);
				}
				M.FlagM.SetFlag(Flag, ValueWhenOn);
				m_isRunning = true;
			}
		}

		public void OFF()
		{
			m_isRunning = false;
		}
	}
}
