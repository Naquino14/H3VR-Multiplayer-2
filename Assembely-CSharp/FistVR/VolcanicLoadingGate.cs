using UnityEngine;

namespace FistVR
{
	public class VolcanicLoadingGate : FVRInteractiveObject
	{
		public Transform LoadingGate;

		private float m_curRot;

		private float m_tarRot;

		private bool m_isOpen;

		public FVRFireArmMagazine Magazine;

		public GameObject Gate;

		public new void Awake()
		{
			Magazine.IsExtractable = !m_isOpen;
			Gate.SetActive(m_isOpen);
		}

		public void Update()
		{
			m_curRot = Mathf.MoveTowards(m_curRot, m_tarRot, Time.deltaTime * 700f);
			LoadingGate.localEulerAngles = new Vector3(0f, 0f, m_curRot);
		}

		public override void SimpleInteraction(FVRViveHand hand)
		{
			base.SimpleInteraction(hand);
			ToggleGate();
		}

		private void ToggleGate()
		{
			m_isOpen = !m_isOpen;
			if (m_isOpen)
			{
				m_tarRot = 180f;
			}
			else
			{
				m_tarRot = 0f;
			}
			Magazine.IsExtractable = !m_isOpen;
			Gate.SetActive(m_isOpen);
		}
	}
}
