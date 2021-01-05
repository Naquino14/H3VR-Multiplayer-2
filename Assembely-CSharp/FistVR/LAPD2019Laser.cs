using UnityEngine;

namespace FistVR
{
	public class LAPD2019Laser : MonoBehaviour
	{
		public GameObject BeamHitPoint;

		public Transform Aperture;

		public LayerMask LM;

		private RaycastHit m_hit;

		private bool m_isOn;

		public bool UsesAutoOnOff;

		public FVRPhysicalObject PO;

		public Transform Muzzle;

		public void TurnOn()
		{
			m_isOn = true;
		}

		public void TurnOff()
		{
			m_isOn = false;
		}

		private void Update()
		{
			if (UsesAutoOnOff)
			{
				if (PO.IsHeld)
				{
					TurnOn();
				}
				else
				{
					TurnOff();
				}
			}
			if (m_isOn)
			{
				if (Muzzle != null)
				{
					Aperture.transform.LookAt(Muzzle.position + Muzzle.forward * 25f);
					Aperture.transform.localEulerAngles = new Vector3(Aperture.transform.localEulerAngles.x, 0f, 0f);
				}
				if (!BeamHitPoint.activeSelf)
				{
					BeamHitPoint.SetActive(value: true);
				}
				Vector3 position = Aperture.position + Aperture.forward * 100f;
				float num = 100f;
				if (Physics.Raycast(Aperture.position, Aperture.forward, out m_hit, 100f, LM, QueryTriggerInteraction.Ignore))
				{
					position = m_hit.point;
					num = m_hit.distance;
				}
				float t = num * 0.01f;
				float num2 = Mathf.Lerp(0.01f, 0.2f, t);
				BeamHitPoint.transform.position = position;
				BeamHitPoint.transform.localScale = new Vector3(num2, num2, num2);
			}
			else if (BeamHitPoint.activeSelf)
			{
				BeamHitPoint.SetActive(value: false);
			}
		}
	}
}
