using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MedigunBeam : MonoBehaviour
{
	private bool wasBeamEnabled;

	public bool beamEnabled;

	private bool wasElectricityEnabled;

	public bool electricityEnabled;

	public Transform target;

	public LineRenderer lineRenderer;

	public GameObject Electricity;

	public int segments = 256;

	public float startTangentApproachRate = 0.9f;

	public float endTangentApproachRate = 0.1f;

	private Vector3 startTangent;

	private Vector3 endTangent;

	private Vector3[] positions;

	private Material lineMaterial;

	[ColorUsage(true, true, 0f, 8f, 0f, 8f)]
	public Color[] LineColors;

	public GameObject[] ElectricityByTeam;

	private float lineMaterialCullTime = 2f;

	private bool wasVisible;

	private float endTime = -100000f;

	private bool initialized;

	private static Vector3 Lerp(Vector3 a, Vector3 b, float t)
	{
		return Vector3.Lerp(a, b, t);
	}

	private static Vector3 Bezier(Vector3 a, Vector3 b, Vector3 c, Vector3 d, float t)
	{
		float num = (b.x - a.x) * t + a.x;
		float num2 = (b.y - a.y) * t + a.y;
		float num3 = (b.z - a.z) * t + a.z;
		float num4 = (c.x - b.x) * t + b.x;
		float num5 = (c.y - b.y) * t + b.y;
		float num6 = (c.z - b.z) * t + b.z;
		float num7 = (d.x - c.x) * t + c.x;
		float num8 = (d.y - c.y) * t + c.y;
		float num9 = (d.z - c.z) * t + c.z;
		float num10 = (num4 - num) * t + num;
		float num11 = (num5 - num2) * t + num2;
		float num12 = (num6 - num3) * t + num3;
		float num13 = (num7 - num4) * t + num4;
		float num14 = (num8 - num5) * t + num5;
		float num15 = (num9 - num6) * t + num6;
		float x = (num13 - num10) * t + num10;
		float y = (num14 - num11) * t + num11;
		float z = (num15 - num12) * t + num12;
		return new Vector3(x, y, z);
	}

	private static float Approach(float rate, float time)
	{
		return 1f - Mathf.Pow(1f - rate, time);
	}

	private static float Approach(float rate, float time, float referenceFramerate)
	{
		return 1f - Mathf.Pow(1f - rate, time * referenceFramerate);
	}

	private static void ComputeTangents(Vector3 start, Vector3 end, Vector3 startDirection, out Vector3 startTangentPosition, out Vector3 endTangentPosition)
	{
		Vector3 value = end - start;
		float magnitude = value.magnitude;
		float t = 1f - (Vector3.Dot(Vector3.Normalize(value), startDirection) * 0.5f + 0.5f);
		startTangentPosition = start + startDirection * Mathf.Lerp(magnitude * 0.333333343f, magnitude, t);
		endTangentPosition = (startTangentPosition + end) * 0.5f;
	}

	public void StartBeam(Transform target)
	{
		ComputeTangents(base.transform.position, target.position, base.transform.forward, out startTangent, out endTangent);
		beamEnabled = true;
		this.target = target;
	}

	public void StopBeam()
	{
		beamEnabled = false;
	}

	public void SetLineColor(int i)
	{
		lineMaterial.SetColor("_Color", LineColors[i]);
	}

	public void SetElectricityColor(int i)
	{
		Electricity = ElectricityByTeam[i];
	}

	public void Initialize()
	{
		ComputeTangents(base.transform.position, target.position, base.transform.forward, out startTangent, out endTangent);
		if (lineRenderer == null)
		{
			lineRenderer = GetComponentInChildren<LineRenderer>();
		}
		if (lineRenderer == null)
		{
			Debug.LogError("No LineRenderer attached to MedicBeam object", this);
			return;
		}
		positions = new Vector3[segments];
		lineRenderer.positionCount = segments;
		lineMaterial = lineRenderer.material;
		lineRenderer.sharedMaterial = lineMaterial;
		float @float = lineMaterial.GetFloat("_BlendRange");
		float float2 = lineMaterial.GetFloat("_BlendTime");
		lineMaterialCullTime = float2 + float2 * @float;
		initialized = true;
	}

	private void Update()
	{
		if (!(target == null))
		{
			if (!initialized)
			{
				Initialize();
			}
			if (!wasBeamEnabled && beamEnabled)
			{
				lineMaterial.SetFloat("_StartTime", Time.timeSinceLevelLoad);
				endTime = float.MaxValue;
				lineMaterial.SetFloat("_EndTime", endTime);
			}
			else if (wasBeamEnabled && !beamEnabled)
			{
				endTime = Time.timeSinceLevelLoad;
				lineMaterial.SetFloat("_EndTime", endTime);
			}
			if (target == null)
			{
				lineMaterial.SetFloat("_EndTime", float.MinValue);
			}
			wasBeamEnabled = beamEnabled;
			wasElectricityEnabled = electricityEnabled;
			Vector3 position = base.transform.position;
			Vector3 position2 = target.position;
			ComputeTangents(position, position2, base.transform.forward, out var startTangentPosition, out var endTangentPosition);
			startTangent = Vector3.Lerp(startTangent, startTangentPosition, Approach(startTangentApproachRate, Time.deltaTime));
			endTangent = Vector3.Lerp(endTangent, endTangentPosition, Approach(endTangentApproachRate, Time.deltaTime));
			for (int i = 0; i < segments; i++)
			{
				ref Vector3 reference = ref positions[i];
				reference = Bezier(position, startTangent, endTangent, position2, (float)i / (float)(segments - 1));
			}
			lineRenderer.SetPositions(positions);
			bool flag = Time.timeSinceLevelLoad < endTime + lineMaterialCullTime;
			if (flag && !wasVisible)
			{
				lineRenderer.enabled = true;
			}
			else if (!flag && wasVisible)
			{
				lineRenderer.enabled = false;
			}
			wasVisible = flag;
		}
	}

	private void OnDrawGizmos()
	{
		if (!(target == null) && !Application.isPlaying)
		{
			Vector3 position = base.transform.position;
			Vector3 position2 = target.position;
			float num = 0.05f;
			Gizmos.color = new Color(0.1f, 0.3f, 0.9f);
			Gizmos.DrawWireCube(startTangent, Vector3.one * num);
			Gizmos.DrawWireCube(endTangent, Vector3.one * num);
			Gizmos.DrawWireCube(position, Vector3.one * num);
			Gizmos.DrawWireCube(position2, Vector3.one * num);
			Gizmos.color = new Color(0.1f, 0.3f, 0.9f, 0.25f);
			Gizmos.DrawLine(position, startTangent);
			Gizmos.DrawLine(startTangent, endTangent);
			Gizmos.DrawLine(endTangent, position2);
			Gizmos.DrawCube(position, Vector3.one * num);
			Gizmos.DrawCube(position2, Vector3.one * num);
			Gizmos.DrawCube(startTangent, Vector3.one * num);
			Gizmos.DrawCube(endTangent, Vector3.one * num);
			Gizmos.color = new Color(0.7f, 0.9f, 0.1f);
			for (int i = 0; i < segments; i++)
			{
				float num2 = (float)i / (float)segments;
				float t = num2 + 1f / (float)segments;
				Vector3 from = Bezier(position, startTangent, endTangent, position2, num2);
				Vector3 to = Bezier(position, startTangent, endTangent, position2, t);
				Gizmos.DrawLine(from, to);
			}
		}
	}
}
