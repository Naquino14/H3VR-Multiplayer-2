using UnityEngine;

[AddComponentMenu("Camera-Control/Smooth Mouse Orbit - Unluck Software")]
public class SmoothCameraOrbit : MonoBehaviour
{
	public Transform target;

	public Vector3 targetOffset;

	public float distance = 5f;

	public float maxDistance = 20f;

	public float minDistance = 0.6f;

	public float xSpeed = 200f;

	public float ySpeed = 200f;

	public int yMinLimit = -80;

	public int yMaxLimit = 80;

	public int zoomRate = 40;

	public float panSpeed = 0.3f;

	public float zoomDampening = 5f;

	public float autoRotate = 1f;

	public float autoRotateSpeed = 0.1f;

	private float xDeg;

	private float yDeg;

	private float currentDistance;

	private float desiredDistance;

	private Quaternion currentRotation;

	private Quaternion desiredRotation;

	private Quaternion rotation;

	private Vector3 position;

	private float idleTimer;

	private float idleSmooth;

	private void Start()
	{
		Init();
	}

	private void OnEnable()
	{
		Init();
	}

	public void Init()
	{
		if (!target)
		{
			GameObject gameObject = new GameObject("Cam Target");
			gameObject.transform.position = base.transform.position + base.transform.forward * distance;
			target = gameObject.transform;
		}
		currentDistance = distance;
		desiredDistance = distance;
		position = base.transform.position;
		rotation = base.transform.rotation;
		currentRotation = base.transform.rotation;
		desiredRotation = base.transform.rotation;
		xDeg = Vector3.Angle(Vector3.right, base.transform.right);
		yDeg = Vector3.Angle(Vector3.up, base.transform.up);
		position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
	}

	private void LateUpdate()
	{
		if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
		{
			desiredDistance -= Input.GetAxis("Mouse Y") * 0.02f * (float)zoomRate * 0.125f * Mathf.Abs(desiredDistance);
		}
		else if (Input.GetMouseButton(0))
		{
			xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
			yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
			yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
			desiredRotation = Quaternion.Euler(yDeg, xDeg, 0f);
			currentRotation = base.transform.rotation;
			rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f * zoomDampening);
			base.transform.rotation = rotation;
			idleTimer = 0f;
			idleSmooth = 0f;
		}
		else
		{
			idleTimer += 0.02f;
			if (idleTimer > autoRotate && autoRotate > 0f)
			{
				idleSmooth += (0.02f + idleSmooth) * 0.005f;
				idleSmooth = Mathf.Clamp(idleSmooth, 0f, 1f);
				xDeg += xSpeed * Time.deltaTime * idleSmooth * autoRotateSpeed;
			}
			yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
			desiredRotation = Quaternion.Euler(yDeg, xDeg, 0f);
			currentRotation = base.transform.rotation;
			rotation = Quaternion.Lerp(currentRotation, desiredRotation, 0.02f * zoomDampening * 2f);
			base.transform.rotation = rotation;
		}
		desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * 0.02f * (float)zoomRate * Mathf.Abs(desiredDistance);
		desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
		currentDistance = Mathf.Lerp(currentDistance, desiredDistance, 0.02f * zoomDampening);
		position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
		base.transform.position = position;
	}

	private static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
