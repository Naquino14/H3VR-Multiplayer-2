// Decompiled with JetBrains decompiler
// Type: AICoverPointManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
    this.Octree = new PointOctree<AICoverPoint>(this.OctreeSize, this.transform.position, this.CellSize);
    for (int index = 0; index < this.MyCoverPoints.Count; ++index)
      this.Octree.Add(this.MyCoverPoints[index], this.MyCoverPoints[index].transform.position);
    if (this.PopulateAtInit)
    {
      AICoverPoint[] objectsOfType = Object.FindObjectsOfType<AICoverPoint>();
      for (int index = 0; index < objectsOfType.Length; ++index)
        this.Octree.Add(objectsOfType[index], objectsOfType[index].transform.position);
    }
    this.path = new NavMeshPath();
  }

  private float GetPseudoDistance(Vector3 p1, Vector3 p2) => Vector3.Distance(p1, p2);

  public bool GetBestTacticalPoint(
    float searchRange,
    Vector3 currentPos,
    Vector3 targetPoint,
    Vector3 goalPos,
    Vector3 takeCoverFromDir,
    Vector2 minMaxRangeToTarget,
    bool usesTargetPoint,
    bool usesGoalPoint,
    bool usesTakeCoverFromDir,
    out AICoverPoint cp,
    AICoverPoint curPoint,
    out float nextSearchRange)
  {
    cp = (AICoverPoint) null;
    nextSearchRange = searchRange;
    List<AICoverPoint> nearBy = new List<AICoverPoint>();
    if (this.Octree.GetNearbyNonAlloc(currentPos, searchRange, nearBy))
    {
      for (int index = nearBy.Count - 1; index >= 0; --index)
      {
        if (nearBy[index].IsClaimed && (Object) nearBy[index] != (Object) curPoint)
          nearBy.RemoveAt(index);
        else if (!nearBy[index].IsActive)
          nearBy.RemoveAt(index);
      }
      if (nearBy.Count > 45)
        nextSearchRange = (double) searchRange >= (double) this.SearchRangeStepSize * 2.0 ? searchRange - this.SearchRangeStepSize : (searchRange *= 0.5f);
      else if (nearBy.Count < 8)
        nextSearchRange = searchRange + this.SearchRangeStepSize;
      if (nearBy.Count < 1)
        return false;
      float num1 = -1f;
      float num2 = 10f;
      if (usesTargetPoint)
        num2 = this.GetPseudoDistance(currentPos, targetPoint);
      for (int index1 = 0; index1 < nearBy.Count; ++index1)
      {
        AICoverPoint aiCoverPoint = nearBy[index1];
        aiCoverPoint.Scratch = 0.0f;
        this.path.ClearCorners();
        float num3;
        if (NavMesh.CalculatePath(currentPos, aiCoverPoint.Pos, -1, this.path))
        {
          float num4 = 0.0f;
          if (this.path.status != NavMeshPathStatus.PathInvalid && this.path.corners.Length > 1)
          {
            for (int index2 = 1; index2 < this.path.corners.Length; ++index2)
              num4 += Vector3.Distance(this.path.corners[index2 - 1], this.path.corners[index2]);
          }
          num3 = num4;
        }
        else
          num3 = this.GetPseudoDistance(currentPos, aiCoverPoint.Pos);
        aiCoverPoint.Scratch += Mathf.Clamp(20f - num3, 0.0f, 20f);
        if ((double) num3 <= (double) searchRange + (double) this.SearchRangeStepSize * 2.0)
        {
          if (usesTargetPoint)
          {
            float pseudoDistance = this.GetPseudoDistance(targetPoint, aiCoverPoint.Pos);
            if ((double) pseudoDistance > (double) minMaxRangeToTarget.x && (double) pseudoDistance < (double) minMaxRangeToTarget.y)
              aiCoverPoint.Scratch += 20f;
            else if ((double) pseudoDistance > (double) minMaxRangeToTarget.y)
            {
              float num4 = (float) (((double) minMaxRangeToTarget.y - (double) Mathf.Abs(pseudoDistance - minMaxRangeToTarget.y)) / (double) minMaxRangeToTarget.y * 10.0);
              aiCoverPoint.Scratch += num4;
            }
            else if ((double) minMaxRangeToTarget.x > (double) pseudoDistance)
            {
              float num4 = Mathf.Clamp((float) (1.0 - ((double) minMaxRangeToTarget.x - (double) pseudoDistance) / (double) minMaxRangeToTarget.x), 0.0f, 1f) * 10f;
              aiCoverPoint.Scratch += num4;
            }
            float maxVisToPoint1 = aiCoverPoint.GetMaxVisToPoint(targetPoint, 0);
            if ((double) pseudoDistance < (double) maxVisToPoint1)
            {
              aiCoverPoint.Scratch += 60f;
              float maxVisToPoint2 = aiCoverPoint.GetMaxVisToPoint(targetPoint, 2);
              if ((double) pseudoDistance > (double) maxVisToPoint2)
              {
                aiCoverPoint.Scratch += 10f;
                float maxVisToPoint3 = aiCoverPoint.GetMaxVisToPoint(targetPoint, 1);
                if ((double) pseudoDistance > (double) maxVisToPoint3)
                  aiCoverPoint.Scratch += 30f;
              }
            }
            if (usesTakeCoverFromDir && (double) pseudoDistance < (double) num2)
              aiCoverPoint.Scratch -= 30f;
          }
          if (usesGoalPoint)
          {
            float pseudoDistance = this.GetPseudoDistance(goalPos, aiCoverPoint.Pos);
            aiCoverPoint.Scratch += Mathf.Clamp(100f - pseudoDistance, 0.0f, 100f) * 0.3f;
          }
          if (usesTakeCoverFromDir)
          {
            float maxVisFromDir = aiCoverPoint.GetMaxVisFromDir(takeCoverFromDir, 0);
            if ((double) maxVisFromDir <= 1.0)
              aiCoverPoint.Scratch += 30f;
            else if ((double) maxVisFromDir <= 3.0)
              aiCoverPoint.Scratch += 20f;
            else if ((double) maxVisFromDir <= 10.0)
              aiCoverPoint.Scratch += 10f;
          }
          if ((double) aiCoverPoint.Scratch > (double) num1)
          {
            num1 = aiCoverPoint.Scratch;
            cp = aiCoverPoint;
          }
        }
      }
      return (Object) cp != (Object) null;
    }
    nextSearchRange = searchRange + this.SearchRangeStepSize;
    return false;
  }
}
