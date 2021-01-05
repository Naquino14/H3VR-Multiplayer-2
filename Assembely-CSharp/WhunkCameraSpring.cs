using UnityEngine;

public class WhunkCameraSpring : MonoBehaviour
{
	public Vector3 sphereCenter;

	public float sphereRadius = 0.5f;

	[Range(0f, 1f)]
	public float springAppoachSpeed = 0.1f;

	[Range(0f, 1f)]
	public float localDamping;

	[Range(0f, 1f)]
	public float worldDamping;

	[HideInInspector]
	public Vector3 particlePosition;

	[HideInInspector]
	public Vector3 particleVelocity;

	private Vector3 lastWorldSphereCenter;

	private static float Approach(float rate, float time)
	{
		return 1f - Mathf.Pow(1f - rate, time);
	}

	private static Vector3 Approach(Vector3 point, Vector3 target, float rate, float time)
	{
		return Vector3.Lerp(point, target, Approach(rate, time));
	}

	private static Vector3 ClampSphere(Vector3 x, Vector3 center, float radius)
	{
		Vector3 vector = x - center;
		Vector3 vector2 = Vector3.ClampMagnitude(vector, radius);
		return center + vector2;
	}

	private static Vector3 FromToVelocity(Vector3 from, Vector3 to, float deltaTime)
	{
		return (to - from) / deltaTime;
	}

	public static void SimulateSpringBall(ref Vector3 particlePosition, ref Vector3 particleVelocity, Vector3 sphereCenter, Vector3 sphereVelocity, float sphereRadius, float deltaTime, float springAppoachSpeed = 0.95f, float localDamping = 0.1f, float worldDamping = 0f)
	{
		Vector3 vector = particlePosition;
		Vector3 point = particleVelocity;
		Vector3 vector2 = Approach(point, new Vector3(0f, 0f, 0f), worldDamping, deltaTime * 90f);
		vector2 = Approach(point, sphereVelocity, localDamping, deltaTime * 90f);
		Vector3 vector3 = Approach(vector2, FromToVelocity(vector, sphereCenter, deltaTime), springAppoachSpeed, deltaTime * 90f);
		Vector3 vector4 = vector3 * deltaTime;
		Vector3 x = vector + vector4;
		particleVelocity = FromToVelocity(vector, particlePosition = ClampSphere(x, sphereCenter, sphereRadius), deltaTime);
	}

	public Vector3 GetSphereCenterWS()
	{
		Vector3 vector = sphereCenter;
		Transform parent = base.transform.parent;
		if (parent != null)
		{
			vector = parent.TransformPoint(vector);
		}
		return vector;
	}

	public void Tick(float deltaTime)
	{
		Vector3 sphereCenterWS = GetSphereCenterWS();
		SimulateSpringBall(ref particlePosition, ref particleVelocity, sphereCenterWS, FromToVelocity(lastWorldSphereCenter, sphereCenterWS, deltaTime), sphereRadius, deltaTime, springAppoachSpeed, localDamping, worldDamping);
		lastWorldSphereCenter = sphereCenterWS;
		base.transform.position = particlePosition;
	}

	public void Reset()
	{
		Vector3 position = (particlePosition = (lastWorldSphereCenter = GetSphereCenterWS()));
		base.transform.position = position;
		particleVelocity = new Vector3(0f, 0f, 0f);
	}

	public void ResetPosition()
	{
		particlePosition = GetSphereCenterWS();
		base.transform.position = particlePosition;
	}

	public void ResetVelocity()
	{
		lastWorldSphereCenter = GetSphereCenterWS();
		particleVelocity = new Vector3(0f, 0f, 0f);
	}

	private void OnEnable()
	{
		lastWorldSphereCenter = GetSphereCenterWS();
	}

	private void FixedUpdate()
	{
		Tick(Time.deltaTime);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(0.3f, 0.9f, 0.1f);
		Gizmos.DrawWireSphere(GetSphereCenterWS(), sphereRadius);
	}
}
