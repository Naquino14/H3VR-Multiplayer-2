using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class FVRHealthBar : MonoBehaviour
	{
		public Text HealthReadOut;

		private void Update()
		{
			string empty = string.Empty;
			empty += GM.CurrentPlayerBody.GetPlayerHealthRaw();
			empty += " / ";
			empty += GM.CurrentPlayerBody.GetMaxHealthPlayerRaw();
			HealthReadOut.text = empty;
			Vector3 position = GM.CurrentPlayerBody.Head.position + Vector3.up * 0.4f;
			Vector3 forward = GM.CurrentPlayerBody.Head.forward;
			forward.y = 0f;
			forward.Normalize();
			forward *= 0.25f;
			position += forward;
			base.transform.position = position;
			base.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
		}
	}
}
