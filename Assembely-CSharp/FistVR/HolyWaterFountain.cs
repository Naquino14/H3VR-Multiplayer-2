using UnityEngine;

namespace FistVR
{
	public class HolyWaterFountain : MonoBehaviour
	{
		public GameObject Water;

		private float m_waterCheckTick = 1f;

		private void Update()
		{
			if (m_waterCheckTick > 0f)
			{
				m_waterCheckTick -= Time.deltaTime;
				return;
			}
			m_waterCheckTick = 1f;
			float num = Vector3.Distance(base.transform.position, GM.CurrentPlayerBody.Head.position);
			if (num < 50f)
			{
				Water.SetActive(value: true);
			}
			else
			{
				Water.SetActive(value: false);
			}
			if ((double)num < 0.5)
			{
				GM.CurrentPlayerBody.ResetHealth();
			}
		}
	}
}
