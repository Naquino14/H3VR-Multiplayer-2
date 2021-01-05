using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class ScalpelDrumDisplay : MonoBehaviour
	{
		private int m_numRoundsDisplayed;

		public List<float> RotsForCapacity;

		public FVRFireArmMagazine Mag;

		public Transform RotPiece;

		private void Start()
		{
		}

		private void Update()
		{
			int numRounds = Mag.m_numRounds;
			if (m_numRoundsDisplayed != numRounds)
			{
				m_numRoundsDisplayed = numRounds;
				SetRot(numRounds);
			}
		}

		private void SetRot(int i)
		{
			RotPiece.localEulerAngles = new Vector3(0f, 0f, RotsForCapacity[i]);
		}
	}
}
