using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class FVRReverbSystem : MonoBehaviour
	{
		public List<FVRReverbEnvironment> Environments;

		public int NumToCheckAFrame = 10;

		private bool hasEnvironmentsToCheck;

		public FVRReverbEnvironment CurrentReverbEnvironment;

		public FVRReverbEnvironment DefaultEnvironment;

		private int m_lowestPriority = 100;

		public void Awake()
		{
			CurrentReverbEnvironment = DefaultEnvironment;
			if (Environments.Count > 0)
			{
				hasEnvironmentsToCheck = true;
			}
		}

		public void Start()
		{
			SM.ReverbSystem = this;
			for (int i = 0; i < Environments.Count; i++)
			{
				Environments[i].SetPriorityBasedOnType();
				m_lowestPriority = Mathf.Min(m_lowestPriority, Environments[i].Priority);
			}
		}

		public void OnDestroy()
		{
			SM.ReverbSystem = null;
		}

		public void Update()
		{
			if (hasEnvironmentsToCheck)
			{
				CheckPlayerEnvironment();
			}
		}

		public void CheckPlayerEnvironment()
		{
			CurrentReverbEnvironment = GetSoundEnvironment(GM.CurrentPlayerBody.Head.position);
			SetCurrentReverbSettings(CurrentReverbEnvironment.Environment);
		}

		public bool TestVolumeBool(FVRReverbEnvironment e, Vector3 pos)
		{
			bool result = true;
			Vector3 vector = e.transform.InverseTransformPoint(pos);
			if (Mathf.Abs(vector.x) > 0.5f || Mathf.Abs(vector.y) > 0.5f || Mathf.Abs(vector.z) > 0.5f)
			{
				result = false;
			}
			return result;
		}

		public FVRReverbEnvironment GetSoundEnvironment(Vector3 pos)
		{
			float num = 100f;
			bool flag = false;
			int index = 0;
			for (int i = 0; i < Environments.Count; i++)
			{
				if (TestVolumeBool(Environments[i], pos) && (float)Environments[i].Priority < num)
				{
					num = Environments[i].Priority;
					flag = true;
					index = i;
					if (num <= (float)m_lowestPriority)
					{
						break;
					}
				}
			}
			if (flag)
			{
				return Environments[index];
			}
			return DefaultEnvironment;
		}

		public void SetCurrentReverbSettings(FVRSoundEnvironment e)
		{
			if (GM.CurrentSceneSettings.DefaultSoundEnvironment != e)
			{
				GM.CurrentSceneSettings.DefaultSoundEnvironment = e;
				SM.TransitionToReverbEnvironment(e, 0.1f);
			}
		}
	}
}
