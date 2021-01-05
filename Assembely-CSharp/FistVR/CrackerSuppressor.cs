using UnityEngine;

namespace FistVR
{
	public class CrackerSuppressor : Suppressor
	{
		[Header("Cracker")]
		public GameObject Unsploded;

		public GameObject Sploded;

		public GameObject Effect;

		private bool m_isSploded;

		public override void ShotEffect()
		{
			base.ShotEffect();
			if (!m_isSploded)
			{
				m_isSploded = true;
				Unsploded.SetActive(value: false);
				Sploded.SetActive(value: true);
				Effect.SetActive(value: true);
			}
		}
	}
}
