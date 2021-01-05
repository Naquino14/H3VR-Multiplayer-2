using UnityEngine;

namespace FistVR
{
	public class wwOldTimeySpectacles : FVRPhysicalObject
	{
		private bool m_hasTurnedOn;

		public Texture2D tex;

		public GameObject EffectPrefab;

		public override void UpdateInteraction(FVRViveHand hand)
		{
			base.UpdateInteraction(hand);
			if (!m_hasTurnedOn && Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.EyeCam.transform.position) < 0.2f)
			{
				m_hasTurnedOn = true;
				Object.Instantiate(EffectPrefab, Vector3.zero, Quaternion.identity);
				hand.ForceSetInteractable(null);
				Object.Destroy(base.gameObject);
			}
		}
	}
}
