using UnityEngine;

public class AICoverPoint : MonoBehaviour
{
	public enum SampleHeight
	{
		Standing,
		Crouching,
		Prone
	}

	[InspectorButton("Calc")]
	public bool DoCalc;

	[InspectorButton("TestDist")]
	public bool DoTestDist;

	public int GroupIndex;

	public bool IsActive = true;

	private const int width = 8;

	private const int height = 3;

	public float[] MaxVis = new float[24];

	public LayerMask VisMask;

	public bool[] DoDebug = new bool[3];

	public bool[] DoDebugNew = new bool[3];

	public float[] Heights = new float[3];

	public Vector3[] HitPoints = new Vector3[24];

	public Vector3 Pos;

	public Transform TestCube;

	public float Scratch;

	public bool IsClaimed;

	public float[] MaxDist_Standing = new float[42];

	public float[] MaxDist_Crouching = new float[42];

	public float[] MaxDist_Prone = new float[42];

	public void Start()
	{
		Pos = base.transform.position;
	}

	public void TestDist()
	{
	}

	public float GetMaxVisToPoint(Vector3 point, int whichHeight)
	{
		Vector3 v = point - Pos;
		v.y = 0f;
		int indexFromDir = GetIndexFromDir(v);
		int num = 8 * whichHeight + indexFromDir;
		int num2 = MaxVis.Length;
		if (num >= num2)
		{
			Debug.Log("HOLY SHIT INDEX WRONG");
		}
		return MaxVis[Mathf.Clamp(num, 0, num2 - 1)];
	}

	public float GetMaxVisToPoint(Vector3 point, SampleHeight height)
	{
		Vector3 dir = point - Pos;
		return height switch
		{
			SampleHeight.Standing => GetMaxVisFromDir(dir, MaxDist_Standing), 
			SampleHeight.Crouching => GetMaxVisFromDir(dir, MaxDist_Crouching), 
			_ => GetMaxVisFromDir(dir, MaxDist_Prone), 
		};
	}

	public float GetMaxVisFromDir(Vector3 dir, int whichHeight)
	{
		dir.y = 0f;
		int indexFromDir = GetIndexFromDir(dir);
		return MaxVis[8 * whichHeight + indexFromDir];
	}

	public float GetMaxVisFromDir(Vector3 dir, SampleHeight height)
	{
		return height switch
		{
			SampleHeight.Standing => GetMaxVisFromDir(dir, MaxDist_Standing), 
			SampleHeight.Crouching => GetMaxVisFromDir(dir, MaxDist_Crouching), 
			_ => GetMaxVisFromDir(dir, MaxDist_Prone), 
		};
	}

	public float GetMaxVisFromDir(Vector3 dir, float[] dists)
	{
		float num = Vector3.Angle(dir, Vector3.up);
		int num2 = 0;
		num2 = ((num <= 45f) ? 40 : ((num <= 65f) ? (GetIndexFromDir(dir) + 24) : ((num <= 80f) ? (GetIndexFromDir(dir) + 8) : ((num >= 100f) ? (GetIndexFromDir(dir) + 16) : ((num >= 115f) ? (GetIndexFromDir(dir) + 32) : ((!(num >= 135f)) ? GetIndexFromDir(dir) : 51))))));
		return dists[num2];
	}

	private int GetIndexFromDir(Vector3 v)
	{
		float num = AngleSigned(Vector3.forward, v, Vector3.up);
		if (num < 0f)
		{
			num += 360f;
		}
		return Mathf.FloorToInt(num / 45f);
	}

	public static float AngleSigned(Vector3 v1, Vector3 v2, Vector3 n)
	{
		return Mathf.Atan2(Vector3.Dot(n, Vector3.Cross(v1, v2)), Vector3.Dot(v1, v2)) * 57.29578f;
	}

	private void OnDrawGizmos()
	{
		if (!IsActive)
		{
			Gizmos.color = new Color(0.2f, 0.2f, 0.2f);
		}
		else if (IsClaimed)
		{
			Gizmos.color = new Color(1f, 0.2f, 0.2f);
		}
		else
		{
			Gizmos.color = new Color(0.2f, 1f, 0.2f);
		}
		Gizmos.DrawSphere(base.transform.position, 0.1f);
		Gizmos.DrawWireSphere(base.transform.position, 0.1f);
		for (int i = 0; i < 3; i++)
		{
			if (DoDebug[i])
			{
				float num = Heights[i];
				Vector3 vector = base.transform.position + Vector3.up * num;
				for (int j = 0; j < 8; j++)
				{
					Vector3 forward = Vector3.forward;
					forward = Quaternion.AngleAxis(45f * (float)j, Vector3.up) * forward;
					forward *= MaxVis[8 * i + j];
					Gizmos.color = Color.red;
					Gizmos.DrawLine(vector, vector + forward);
				}
			}
		}
		if (DoDebugNew[0])
		{
			float num2 = Heights[0];
			Vector3 p = base.transform.position + Vector3.up * num2;
			DrawLineSet(p, MaxDist_Standing, 0f, 0);
			DrawLineSet(p, MaxDist_Standing, -10f, 8);
			DrawLineSet(p, MaxDist_Standing, 10f, 16);
			DrawLineSet(p, MaxDist_Standing, -25f, 24);
			DrawLineSet(p, MaxDist_Standing, 25f, 32);
		}
	}

	private void DrawLineSet(Vector3 p1, float[] values, float angle, int offset)
	{
		for (int i = 0; i < 8; i++)
		{
			Vector3 forward = Vector3.forward;
			forward *= values[i + offset];
			forward = Quaternion.AngleAxis(angle, Vector3.right) * forward;
			forward = Quaternion.AngleAxis((float)i * 45f, Vector3.up) * forward;
			Debug.DrawLine(p1, p1 + forward, Color.red);
		}
	}

	[ContextMenu("Calc")]
	public void Calc()
	{
		for (int i = 0; i < 3; i++)
		{
			float num = Heights[i];
			Vector3 vector = base.transform.position + Vector3.up * num;
			for (int j = 0; j < 8; j++)
			{
				Vector3 forward = Vector3.forward;
				float num2 = -22.5f;
				int num3 = 60;
				float num4 = 0f;
				float num5 = 0f;
				for (int k = 0; k <= num3; k++)
				{
					float t = (float)k / (float)num3;
					float num6 = Mathf.Lerp(0f, 45f, t);
					float angle = num2 + 45f * (float)j + num6;
					Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * forward;
					if (Physics.Raycast(vector + Random.onUnitSphere * 0.05f, direction, out var hitInfo, 500f, VisMask, QueryTriggerInteraction.Ignore))
					{
						num4 += hitInfo.distance;
						num5 = Mathf.Max(num5, hitInfo.distance);
					}
					else
					{
						num4 += 500f;
					}
				}
				MaxVis[8 * i + j] = num5;
				Vector3 vector2 = Quaternion.AngleAxis(45f * (float)j, Vector3.up) * Vector3.forward;
				ref Vector3 reference = ref HitPoints[8 * i + j];
				reference = vector + vector2 * MaxVis[8 * i + j];
			}
		}
	}

	[ContextMenu("CalcNew")]
	public void CalcNew()
	{
		CalcDistsForHeight(Heights[0], MaxDist_Standing);
		CalcDistsForHeight(Heights[1], MaxDist_Crouching);
		CalcDistsForHeight(Heights[2], MaxDist_Prone);
	}

	private void CalcDistsForHeight(float height, float[] dists)
	{
		Vector3 p = base.transform.position + Vector3.up * height;
		CalcDistForRingWithOffset(p, dists, 0f, 0);
		CalcDistForRingWithOffset(p, dists, -10f, 8);
		CalcDistForRingWithOffset(p, dists, 10f, 16);
		CalcDistForRingWithOffset(p, dists, -25f, 24);
		CalcDistForRingWithOffset(p, dists, 25f, 32);
		dists[40] = GetDistSampleSet(p, -89f, 0);
		dists[41] = GetDistSampleSet(p, 89f, 0);
	}

	private void CalcDistForRingWithOffset(Vector3 p1, float[] dists, float xRotAmount, int offSet)
	{
		for (int i = 0; i < 8; i++)
		{
			float num = (dists[offSet + i] = GetDistSampleSet(p1, xRotAmount, i));
		}
	}

	private float GetDistSampleSet(Vector3 p1, float xRotAmount, int y)
	{
		Vector3 forward = Vector3.forward;
		forward = Quaternion.AngleAxis(xRotAmount, Vector3.right) * forward;
		float num = -22.5f;
		int num2 = 60;
		float num3 = 0f;
		float num4 = 0f;
		for (int i = 0; i <= num2; i++)
		{
			float t = (float)i / (float)num2;
			float num5 = Mathf.Lerp(0f, 45f, t);
			float angle = num + 45f * (float)y + num5;
			Vector3 direction = Quaternion.AngleAxis(angle, Vector3.up) * forward;
			if (Physics.Raycast(p1 + Random.onUnitSphere * 0.05f, direction, out var hitInfo, 500f, VisMask, QueryTriggerInteraction.Ignore))
			{
				num4 = Mathf.Max(num4, hitInfo.distance);
				num3 += hitInfo.distance;
			}
			else
			{
				num3 += 500f;
			}
		}
		return num4;
	}
}
