using UnityEngine;

namespace FistVR
{
	public class eSlabDiscSensor : MonoBehaviour
	{
		public eSlabDisc Disc;

		private eSlabSensor m_detectedSensor;

		private void OnTriggerEnter(Collider col)
		{
			Detect(col);
		}

		private void Detect(Collider col)
		{
			if (!(Disc == null) && !(col.gameObject.tag != "ESlabSlot"))
			{
				m_detectedSensor = col.gameObject.GetComponent<eSlabSensor>();
			}
		}

		private void Update()
		{
			if (!(m_detectedSensor != null))
			{
				return;
			}
			if (m_detectedSensor.eSlab.insertCooldown <= 0f)
			{
				if (m_detectedSensor.eSlab.LoadDisc(Disc))
				{
					if (Disc.m_hand != null)
					{
						Disc.m_hand.ForceSetInteractable(null);
					}
					Object.Destroy(Disc.gameObject);
				}
				else
				{
					m_detectedSensor = null;
				}
			}
			else
			{
				m_detectedSensor = null;
			}
		}
	}
}
