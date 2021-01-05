using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem.Sample
{
	public class TargetMeasurement : MonoBehaviour
	{
		public GameObject visualWrapper;

		public Transform measurementTape;

		public Transform endPoint;

		public Text measurementTextM;

		public Text measurementTextFT;

		public float maxDistanceToDraw = 6f;

		public bool drawTape;

		private float lastDistance;

		private void Update()
		{
			if (Camera.main != null)
			{
				Vector3 position = Camera.main.transform.position;
				position.y = endPoint.position.y;
				float num = Vector3.Distance(position, endPoint.position);
				Vector3 position2 = Vector3.Lerp(position, endPoint.position, 0.5f);
				base.transform.position = position2;
				base.transform.forward = endPoint.position - position;
				measurementTape.localScale = new Vector3(0.05f, num, 0.05f);
				if (Mathf.Abs(num - lastDistance) > 0.01f)
				{
					measurementTextM.text = num.ToString("00.0m");
					measurementTextFT.text = ((double)num * 3.28084).ToString("00.0ft");
					lastDistance = num;
				}
				if (drawTape)
				{
					visualWrapper.SetActive(num < maxDistanceToDraw);
				}
				else
				{
					visualWrapper.SetActive(value: false);
				}
			}
		}
	}
}
