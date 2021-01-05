using UnityEngine;

public class PTargetRailJoint : MonoBehaviour
{
	public bool travelToEnd = true;

	public float travelSpeed = 1f;

	public float particleDistance = 1.5f;

	public float angleLimitBack = 45f;

	public float angleLimitFront = 45f;

	public float gravityScale = 1f;

	public float damping = 0.05f;

	public Transform targetTransform;

	private Vector3 particlePosition;

	private Vector3 particleVelocity;

	private Vector3 worldSpaceStartPosition;

	private Vector3 worldSpaceRailDirection;

	private Quaternion targetInitialRoation;

	private float currentMoveDistance;

	private float targetOffsetDistance;

	private static float Approach(float rate, float time)
	{
		return 1f - Mathf.Pow(1f - rate, time);
	}

	private static Vector3 Approach(Vector3 point, Vector3 target, float rate, float time)
	{
		return Vector3.Lerp(point, target, Approach(rate, time));
	}

	private static Vector3 ProjectOnHinge(Vector3 point, Vector3 hingePivot, Vector3 hingeRotationAxis, Vector3 hingeDirection, float distanceLimit, float angleLimitLeft, float angleLimitRight)
	{
		Vector3 rhs = Vector3.Cross(hingeRotationAxis, hingeDirection);
		Vector3 vector = point - hingePivot;
		vector = Vector3.ProjectOnPlane(vector, hingeRotationAxis).normalized * distanceLimit;
		Vector3 normalized = vector.normalized;
		float num = Vector3.Angle(normalized, hingeDirection);
		bool flag = Vector3.Dot(normalized, rhs) > 0f;
		float num2 = Mathf.Min(angleLimitLeft - num, 0f);
		float num3 = 0f - Mathf.Min(angleLimitRight - num, 0f);
		float angle = ((!flag) ? num3 : num2);
		Quaternion quaternion = Quaternion.AngleAxis(angle, hingeRotationAxis);
		vector = quaternion * vector;
		return vector + hingePivot;
	}

	public void GoToDistance(float distance)
	{
		currentMoveDistance = distance;
	}

	public void SetHeight(float height)
	{
		Vector3 position = base.transform.position;
		position.y = height;
		base.transform.position = position;
		worldSpaceStartPosition.y = height;
		particleVelocity = Vector3.zero;
		ResetSimulation();
	}

	public void ResetSimulation()
	{
		Transform transform = base.transform;
		particlePosition = transform.position - transform.up * particleDistance;
		particleVelocity = Vector3.zero;
	}

	private void Start()
	{
		Transform transform = base.transform;
		ResetSimulation();
		worldSpaceStartPosition = transform.position;
		currentMoveDistance = 0f;
		worldSpaceRailDirection = -transform.forward;
		targetOffsetDistance = 0f - targetTransform.localPosition.y;
		targetInitialRoation = targetTransform.localRotation;
		Vector3 right = transform.right;
		Vector3 normalized = (particlePosition - transform.position).normalized;
		targetTransform.position = base.transform.position + normalized * targetOffsetDistance;
		Vector3 forward = Vector3.Cross(right, -normalized);
		targetTransform.rotation = Quaternion.LookRotation(forward, -normalized) * targetInitialRoation;
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		Transform transform = base.transform;
		transform.position = Vector3.MoveTowards(transform.position, worldSpaceStartPosition + worldSpaceRailDirection * currentMoveDistance, travelSpeed * deltaTime);
		Vector3 position = transform.position;
		Vector3 up = transform.up;
		Vector3 right = transform.right;
		Vector3 forward = transform.forward;
		Vector3 vector = Physics.gravity * gravityScale;
		Vector3 vector2 = particlePosition;
		Vector3 vector3 = Approach(particleVelocity, Vector3.zero, damping, deltaTime * 90f);
		Vector3 vector4 = vector3 + vector * deltaTime;
		Vector3 point = vector2 + vector4 * deltaTime;
		particleVelocity = ((particlePosition = ProjectOnHinge(point, position, right, -up, particleDistance, angleLimitBack, angleLimitFront)) - vector2) / deltaTime;
		Vector3 normalized = (particlePosition - position).normalized;
		targetTransform.position = position + normalized * targetOffsetDistance;
		Vector3 forward2 = Vector3.Cross(right, -normalized);
		targetTransform.rotation = Quaternion.LookRotation(forward2, -normalized) * targetInitialRoation;
	}

	private void OnDrawGizmosSelected()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = new Color(0.1f, 0.3f, 0.9f);
			Gizmos.DrawWireCube(particlePosition, Vector3.one * 0.025f);
			Gizmos.color = new Color(0.1f, 0.3f, 0.9f, 0.5f);
			Gizmos.DrawCube(particlePosition, Vector3.one * 0.025f);
		}
		Vector3 center = base.transform.position - base.transform.up * particleDistance;
		Gizmos.color = new Color(0.7f, 0.9f, 0.1f);
		Gizmos.DrawWireCube(center, Vector3.one * 0.02f);
		Gizmos.color = new Color(0.7f, 0.9f, 0.1f, 0.5f);
		Gizmos.DrawCube(center, Vector3.one * 0.02f);
		Gizmos.color = new Color(0.9f, 0.7f, 0.1f);
		Gizmos.DrawRay(base.transform.position, Quaternion.AngleAxis(angleLimitBack, base.transform.right) * -base.transform.up * particleDistance);
		Gizmos.DrawRay(base.transform.position, Quaternion.AngleAxis(0f - angleLimitFront, base.transform.right) * -base.transform.up * particleDistance);
	}
}
