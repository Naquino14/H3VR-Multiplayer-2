using UnityEngine;

public class WaggleJoint : MonoBehaviour
{
	private const float startTouchingAngle = 0.5f;

	private const float stopTouchingAngle = 10f;

	[Tooltip("The distance the particle is clamped to, scale this up to make it less sensitive to changes.")]
	public float distanceLimit = 0.25f;

	public float angleLimitLeft = 45f;

	public float angleLimitRight = 45f;

	[Tooltip("Multiplier for the physics gravity effecting the particle. Good for dealing with stiff joints or low gravity values.")]
	public float gravityScale = 1f;

	[Tooltip("Pulls the particle back to the hinge direction")]
	public bool useSpring;

	[Tooltip("Rate at which the particle approaches the hinge direction, eg: 0.95 mean's 95% closer each second.")]
	public float springApproachRate = 0.95f;

	[Tooltip("Rate at which the particle loses velocity, eg: 0.5 mean's 50% slower each second.")]
	public float damping;

	[Tooltip("This transform will LookAt() the particle being simulated.")]
	public Transform hingeGraphic;

	public bool ManualExecution;

	[Tooltip("Puts a cooldown on how often the OnHitLimit event can be fired.")]
	public float onHitLimitRefireLimit = 0.1f;

	private Vector3 particlePos;

	private Vector3 particleVel;

	private bool wasTouchingLimit;

	private bool wasTouchingRight;

	private float lastOnHitLimitTime = float.MinValue;

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

	private static bool DetectOnHitLimit(ref bool wasTouching, ref bool wasTouchingMax, float min, float max, float currentValue, float limit, float releaseLimit)
	{
		float num = currentValue - min;
		float num2 = max - num;
		float num3 = Mathf.Min(num, num2);
		bool flag = num > num2;
		bool flag2 = num3 < limit;
		bool flag3 = wasTouching;
		bool flag4 = wasTouchingMax;
		float num4 = ((!flag4) ? num : num2);
		bool result = false;
		if (flag2 && flag != flag4)
		{
			flag3 = true;
			flag4 = flag;
			result = true;
		}
		if (num4 > releaseLimit && wasTouching)
		{
			flag3 = false;
		}
		if (flag2 && !wasTouching)
		{
			flag3 = true;
			flag4 = flag;
			result = true;
		}
		wasTouching = flag3;
		wasTouchingMax = flag4;
		return result;
	}

	private void Start()
	{
		particlePos = base.transform.position + hingeGraphic.forward * distanceLimit;
		particleVel = Vector3.zero;
	}

	public void ResetParticlePos()
	{
		particlePos = base.transform.position + hingeGraphic.forward * distanceLimit;
		particleVel = Vector3.zero;
	}

	private void Update()
	{
		if (!ManualExecution)
		{
			Execute();
		}
	}

	public void Execute()
	{
		Transform transform = base.transform;
		Vector3 position = transform.position;
		Vector3 up = transform.up;
		Vector3 right = transform.right;
		Vector3 forward = transform.forward;
		float deltaTime = Time.deltaTime;
		Vector3 vector = Physics.gravity * gravityScale;
		Vector3 vector2 = particlePos;
		Vector3 vector3 = Approach(particleVel, Vector3.zero, damping, deltaTime);
		Vector3 vector4 = vector3 + vector * deltaTime;
		Vector3 vector5 = vector2 + vector4 * deltaTime;
		Vector3 vector6 = vector5;
		if (useSpring)
		{
			vector5 = Approach(vector5, position + up * distanceLimit, springApproachRate, deltaTime);
		}
		vector5 = (particlePos = ProjectOnHinge(vector5, position, right, up, distanceLimit, angleLimitLeft, angleLimitRight));
		particleVel = (vector5 - vector2) / deltaTime;
		Vector3 normalized = (vector5 - position).normalized;
		Vector3 to = Quaternion.AngleAxis(angleLimitLeft, right) * up;
		Vector3 to2 = Quaternion.AngleAxis(0f - angleLimitRight, right) * up;
		float num = Vector3.Angle(normalized, to);
		float num2 = Vector3.Angle(normalized, to2);
		float currentValue = ((!(num2 < num)) ? (0f - num) : num2);
		bool flag = DetectOnHitLimit(ref wasTouchingLimit, ref wasTouchingRight, 0f - angleLimitLeft, angleLimitRight, currentValue, 0.5f, 10f);
		Vector3 normalized2 = (vector2 - position).normalized;
		Vector3 normalized3 = (vector6 - position).normalized;
		float angularVelocity = Vector3.Angle(normalized2, normalized3) / deltaTime;
		if (flag && Time.timeSinceLevelLoad >= lastOnHitLimitTime + onHitLimitRefireLimit)
		{
			OnHitLimit(angularVelocity);
			lastOnHitLimitTime = Time.timeSinceLevelLoad;
		}
		if (hingeGraphic != null)
		{
			Vector3 worldUp = Vector3.Cross(hingeGraphic.forward, right);
			hingeGraphic.LookAt(particlePos, worldUp);
		}
	}

	private void OnHitLimit(float angularVelocity)
	{
	}

	private void OnDrawGizmosSelected()
	{
		if (Application.isPlaying)
		{
			Gizmos.color = new Color(0.1f, 0.3f, 0.9f);
			Gizmos.DrawWireCube(particlePos, Vector3.one * 0.025f);
			Gizmos.color = new Color(0.1f, 0.3f, 0.9f, 0.5f);
			Gizmos.DrawCube(particlePos, Vector3.one * 0.025f);
			float num = Time.timeSinceLevelLoad - lastOnHitLimitTime;
			Gizmos.color = new Color(0.1f, 0.7f, 0.9f, Mathf.Clamp01((0.5f - num) * 2f));
			Gizmos.DrawWireCube(particlePos, Vector3.one * (0.1f * (num + 0.5f)));
		}
		Vector3 center = base.transform.position + base.transform.up * distanceLimit;
		Gizmos.color = new Color(0.7f, 0.9f, 0.1f);
		Gizmos.DrawWireCube(center, Vector3.one * 0.02f);
		Gizmos.color = new Color(0.7f, 0.9f, 0.1f, 0.5f);
		Gizmos.DrawCube(center, Vector3.one * 0.02f);
		Gizmos.color = new Color(0.9f, 0.7f, 0.1f);
		Gizmos.DrawRay(base.transform.position, Quaternion.AngleAxis(angleLimitLeft, base.transform.right) * base.transform.up * distanceLimit);
		Gizmos.DrawRay(base.transform.position, Quaternion.AngleAxis(0f - angleLimitRight, base.transform.right) * base.transform.up * distanceLimit);
	}
}
