using UnityEngine;

namespace FistVR
{
	public class HeadTriggerDetect : MonoBehaviour
	{
		public ObstacleCourseGame Game;

		private void Awake()
		{
			if (Object.FindObjectOfType<ObstacleCourseGame>() != null)
			{
				Game = Object.FindObjectOfType<ObstacleCourseGame>();
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("ColOnlyHead") && Game != null)
			{
				Game.RegisterHeadPenalty();
			}
		}
	}
}
