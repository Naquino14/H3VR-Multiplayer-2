using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AICoverPointManager : MonoBehaviour
{
	public float OctreeSize = 200f;

	public float DefaultSearchRange = 10f;

	public float SearchRangeStepSize = 5f;

	private PointOctree<AICoverPoint> Octree;

	public List<AICoverPoint> MyCoverPoints;

	public float CellSize = 1f;

	private NavMeshPath path;

	public bool PopulateAtInit;

	public void Init()
	{
		Octree = new PointOctree<AICoverPoint>(OctreeSize, base.transform.position, CellSize);
		for (int i = 0; i < MyCoverPoints.Count; i++)
		{
			Octree.Add(MyCoverPoints[i], MyCoverPoints[i].transform.position);
		}
		if (PopulateAtInit)
		{
			AICoverPoint[] array = Object.FindObjectsOfType<AICoverPoint>();
			for (int j = 0; j < array.Length; j++)
			{
				Octree.Add(array[j], array[j].transform.position);
			}
		}
		path = new NavMeshPath();
	}

	private float GetPseudoDistance(Vector3 p1, Vector3 p2)
	{
		return Vector3.Distance(p1, p2);
	}

	public bool GetBestTacticalPoint(float searchRange, Vector3 currentPos, Vector3 targetPoint, Vector3 goalPos, Vector3 takeCoverFromDir, Vector2 minMaxRangeToTarget, bool usesTargetPoint, bool usesGoalPoint, bool usesTakeCoverFromDir, out AICoverPoint cp, AICoverPoint curPoint, out float nextSearchRange)
	{
		cp = null;
		nextSearchRange = searchRange;
		List<AICoverPoint> list = new List<AICoverPoint>();
		if (Octree.GetNearbyNonAlloc(currentPos, searchRange, list))
		{
			for (int num = list.Count - 1; num >= 0; num--)
			{
				if (list[num].IsClaimed && list[num] != curPoint)
				{
					list.RemoveAt(num);
				}
				else if (!list[num].IsActive)
				{
					list.RemoveAt(num);
				}
			}
			if (list.Count > 45)
			{
				if (searchRange < SearchRangeStepSize * 2f)
				{
					nextSearchRange = (searchRange *= 0.5f);
				}
				else
				{
					nextSearchRange = searchRange - SearchRangeStepSize;
				}
			}
			else if (list.Count < 8)
			{
				nextSearchRange = searchRange + SearchRangeStepSize;
			}
			if (list.Count < 1)
			{
				return false;
			}
			float num2 = -1f;
			float num3 = 10f;
			if (usesTargetPoint)
			{
				num3 = GetPseudoDistance(currentPos, targetPoint);
			}
			for (int i = 0; i < list.Count; i++)
			{
				AICoverPoint aICoverPoint = list[i];
				aICoverPoint.Scratch = 0f;
				float num4 = 0f;
				path.ClearCorners();
				if (NavMesh.CalculatePath(currentPos, aICoverPoint.Pos, -1, path))
				{
					float num5 = 0f;
					if (path.status != NavMeshPathStatus.PathInvalid && path.corners.Length > 1)
					{
						for (int j = 1; j < path.corners.Length; j++)
						{
							num5 += Vector3.Distance(path.corners[j - 1], path.corners[j]);
						}
					}
					num4 = num5;
				}
				else
				{
					num4 = GetPseudoDistance(currentPos, aICoverPoint.Pos);
				}
				aICoverPoint.Scratch += Mathf.Clamp(20f - num4, 0f, 20f);
				if (num4 > searchRange + SearchRangeStepSize * 2f)
				{
					continue;
				}
				if (usesTargetPoint)
				{
					float pseudoDistance = GetPseudoDistance(targetPoint, aICoverPoint.Pos);
					if (pseudoDistance > minMaxRangeToTarget.x && pseudoDistance < minMaxRangeToTarget.y)
					{
						aICoverPoint.Scratch += 20f;
					}
					else if (pseudoDistance > minMaxRangeToTarget.y)
					{
						float num6 = (minMaxRangeToTarget.y - Mathf.Abs(pseudoDistance - minMaxRangeToTarget.y)) / minMaxRangeToTarget.y * 10f;
						aICoverPoint.Scratch += num6;
					}
					else if (minMaxRangeToTarget.x > pseudoDistance)
					{
						float num7 = Mathf.Clamp(1f - (minMaxRangeToTarget.x - pseudoDistance) / minMaxRangeToTarget.x, 0f, 1f) * 10f;
						aICoverPoint.Scratch += num7;
					}
					float maxVisToPoint = aICoverPoint.GetMaxVisToPoint(targetPoint, 0);
					if (pseudoDistance < maxVisToPoint)
					{
						aICoverPoint.Scratch += 60f;
						float maxVisToPoint2 = aICoverPoint.GetMaxVisToPoint(targetPoint, 2);
						if (pseudoDistance > maxVisToPoint2)
						{
							aICoverPoint.Scratch += 10f;
							float maxVisToPoint3 = aICoverPoint.GetMaxVisToPoint(targetPoint, 1);
							if (pseudoDistance > maxVisToPoint3)
							{
								aICoverPoint.Scratch += 30f;
							}
						}
					}
					if (usesTakeCoverFromDir && pseudoDistance < num3)
					{
						aICoverPoint.Scratch -= 30f;
					}
				}
				if (usesGoalPoint)
				{
					float pseudoDistance2 = GetPseudoDistance(goalPos, aICoverPoint.Pos);
					aICoverPoint.Scratch += Mathf.Clamp(100f - pseudoDistance2, 0f, 100f) * 0.3f;
				}
				if (usesTakeCoverFromDir)
				{
					float maxVisFromDir = aICoverPoint.GetMaxVisFromDir(takeCoverFromDir, 0);
					if (maxVisFromDir <= 1f)
					{
						aICoverPoint.Scratch += 30f;
					}
					else if (maxVisFromDir <= 3f)
					{
						aICoverPoint.Scratch += 20f;
					}
					else if (maxVisFromDir <= 10f)
					{
						aICoverPoint.Scratch += 10f;
					}
				}
				if (aICoverPoint.Scratch > num2)
				{
					num2 = aICoverPoint.Scratch;
					cp = aICoverPoint;
				}
			}
			if (cp != null)
			{
				return true;
			}
			return false;
		}
		nextSearchRange = searchRange + SearchRangeStepSize;
		return false;
	}
}
