using UnityEngine;
using UnityEngine.Rendering;

namespace Valve.VR.InteractionSystem
{
	public class TeleportArc : MonoBehaviour
	{
		public int segmentCount = 60;

		public float thickness = 0.01f;

		[Tooltip("The amount of time in seconds to predict the motion of the projectile.")]
		public float arcDuration = 3f;

		[Tooltip("The amount of time in seconds between each segment of the projectile.")]
		public float segmentBreak = 0.025f;

		[Tooltip("The speed at which the line segments of the arc move.")]
		public float arcSpeed = 0.2f;

		public Material material;

		[HideInInspector]
		public int traceLayerMask;

		private LineRenderer[] lineRenderers;

		private float arcTimeOffset;

		private float prevThickness;

		private int prevSegmentCount;

		private bool showArc = true;

		private Vector3 startPos;

		private Vector3 projectileVelocity;

		private bool useGravity = true;

		private Transform arcObjectsTransfrom;

		private bool arcInvalid;

		private float scale = 1f;

		private void Start()
		{
			arcTimeOffset = Time.time;
		}

		private void Update()
		{
			scale = Player.instance.transform.lossyScale.x;
			if (thickness != prevThickness || segmentCount != prevSegmentCount)
			{
				CreateLineRendererObjects();
				prevThickness = thickness;
				prevSegmentCount = segmentCount;
			}
		}

		private void CreateLineRendererObjects()
		{
			if (arcObjectsTransfrom != null)
			{
				Object.Destroy(arcObjectsTransfrom.gameObject);
			}
			GameObject gameObject = new GameObject("ArcObjects");
			arcObjectsTransfrom = gameObject.transform;
			arcObjectsTransfrom.SetParent(base.transform);
			lineRenderers = new LineRenderer[segmentCount];
			for (int i = 0; i < segmentCount; i++)
			{
				GameObject gameObject2 = new GameObject("LineRenderer_" + i);
				gameObject2.transform.SetParent(arcObjectsTransfrom);
				lineRenderers[i] = gameObject2.AddComponent<LineRenderer>();
				lineRenderers[i].receiveShadows = false;
				lineRenderers[i].reflectionProbeUsage = ReflectionProbeUsage.Off;
				lineRenderers[i].lightProbeUsage = LightProbeUsage.Off;
				lineRenderers[i].shadowCastingMode = ShadowCastingMode.Off;
				lineRenderers[i].material = material;
				lineRenderers[i].startWidth = thickness * scale;
				lineRenderers[i].endWidth = thickness * scale;
				lineRenderers[i].enabled = false;
			}
		}

		public void SetArcData(Vector3 position, Vector3 velocity, bool gravity, bool pointerAtBadAngle)
		{
			startPos = position;
			projectileVelocity = velocity;
			useGravity = gravity;
			if (arcInvalid && !pointerAtBadAngle)
			{
				arcTimeOffset = Time.time;
			}
			arcInvalid = pointerAtBadAngle;
		}

		public void Show()
		{
			showArc = true;
			if (lineRenderers == null)
			{
				CreateLineRendererObjects();
			}
		}

		public void Hide()
		{
			if (showArc)
			{
				HideLineSegments(0, segmentCount);
			}
			showArc = false;
		}

		public bool DrawArc(out RaycastHit hitInfo)
		{
			float num = arcDuration / (float)segmentCount;
			float num2 = (Time.time - arcTimeOffset) * arcSpeed;
			if (num2 > num + segmentBreak)
			{
				arcTimeOffset = Time.time;
				num2 = 0f;
			}
			float num3 = num2;
			float num4 = FindProjectileCollision(out hitInfo);
			if (arcInvalid)
			{
				lineRenderers[0].enabled = true;
				lineRenderers[0].SetPosition(0, GetArcPositionAtTime(0f));
				lineRenderers[0].SetPosition(1, GetArcPositionAtTime((!(num4 < num)) ? num : num4));
				HideLineSegments(1, segmentCount);
			}
			else
			{
				int num5 = 0;
				if (num3 > segmentBreak)
				{
					float num6 = num2 - segmentBreak;
					if (num4 < num6)
					{
						num6 = num4;
					}
					DrawArcSegment(0, 0f, num6);
					num5 = 1;
				}
				bool flag = false;
				int num7 = 0;
				if (num3 < num4)
				{
					for (num7 = num5; num7 < segmentCount; num7++)
					{
						float num8 = num3 + num;
						if (num8 >= arcDuration)
						{
							num8 = arcDuration;
							flag = true;
						}
						if (num8 >= num4)
						{
							num8 = num4;
							flag = true;
						}
						DrawArcSegment(num7, num3, num8);
						num3 += num + segmentBreak;
						if (flag || num3 >= arcDuration || num3 >= num4)
						{
							break;
						}
					}
				}
				else
				{
					num7--;
				}
				HideLineSegments(num7 + 1, segmentCount);
			}
			return num4 != float.MaxValue;
		}

		private void DrawArcSegment(int index, float startTime, float endTime)
		{
			lineRenderers[index].enabled = true;
			lineRenderers[index].SetPosition(0, GetArcPositionAtTime(startTime));
			lineRenderers[index].SetPosition(1, GetArcPositionAtTime(endTime));
		}

		public void SetColor(Color color)
		{
			for (int i = 0; i < segmentCount; i++)
			{
				lineRenderers[i].startColor = color;
				lineRenderers[i].endColor = color;
			}
		}

		private float FindProjectileCollision(out RaycastHit hitInfo)
		{
			float num = arcDuration / (float)segmentCount;
			float num2 = 0f;
			hitInfo = default(RaycastHit);
			Vector3 vector = GetArcPositionAtTime(num2);
			for (int i = 0; i < segmentCount; i++)
			{
				float num3 = num2 + num;
				Vector3 arcPositionAtTime = GetArcPositionAtTime(num3);
				if (Physics.Linecast(vector, arcPositionAtTime, out hitInfo, traceLayerMask) && hitInfo.collider.GetComponent<IgnoreTeleportTrace>() == null)
				{
					Util.DrawCross(hitInfo.point, Color.red, 0.5f);
					float num4 = Vector3.Distance(vector, arcPositionAtTime);
					return num2 + num * (hitInfo.distance / num4);
				}
				num2 = num3;
				vector = arcPositionAtTime;
			}
			return float.MaxValue;
		}

		public Vector3 GetArcPositionAtTime(float time)
		{
			Vector3 vector = ((!useGravity) ? Vector3.zero : Physics.gravity);
			return startPos + (projectileVelocity * time + 0.5f * time * time * vector) * scale;
		}

		private void HideLineSegments(int startSegment, int endSegment)
		{
			if (lineRenderers != null)
			{
				for (int i = startSegment; i < endSegment; i++)
				{
					lineRenderers[i].enabled = false;
				}
			}
		}
	}
}
