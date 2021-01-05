using UnityEngine;

namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Camera))]
	public class FallbackCameraController : MonoBehaviour
	{
		public float speed = 4f;

		public float shiftSpeed = 16f;

		public bool showInstructions = true;

		private Vector3 startEulerAngles;

		private Vector3 startMousePosition;

		private float realTime;

		private void OnEnable()
		{
			realTime = Time.realtimeSinceStartup;
		}

		private void Update()
		{
			float num = 0f;
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
			{
				num += 1f;
			}
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			{
				num -= 1f;
			}
			float num2 = 0f;
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
			{
				num2 += 1f;
			}
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
			{
				num2 -= 1f;
			}
			float num3 = speed;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				num3 = shiftSpeed;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num4 = realtimeSinceStartup - realTime;
			realTime = realtimeSinceStartup;
			Vector3 direction = new Vector3(num2, 0f, num) * num3 * num4;
			base.transform.position += base.transform.TransformDirection(direction);
			Vector3 mousePosition = Input.mousePosition;
			if (Input.GetMouseButtonDown(1))
			{
				startMousePosition = mousePosition;
				startEulerAngles = base.transform.localEulerAngles;
			}
			if (Input.GetMouseButton(1))
			{
				Vector3 vector = mousePosition - startMousePosition;
				base.transform.localEulerAngles = startEulerAngles + new Vector3((0f - vector.y) * 360f / (float)Screen.height, vector.x * 360f / (float)Screen.width, 0f);
			}
		}

		private void OnGUI()
		{
			if (showInstructions)
			{
				GUI.Label(new Rect(10f, 10f, 600f, 400f), "WASD/Arrow Keys to translate the camera\nRight mouse click to rotate the camera\nLeft mouse click for standard interactions.\n");
			}
		}
	}
}
