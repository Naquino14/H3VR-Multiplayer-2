// Decompiled with JetBrains decompiler
// Type: PTargetPiece
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FistVR;
using System;
using UnityEngine;

[Serializable]
public class PTargetPiece : MonoBehaviour, IOnPTargetChangeUnsafe, IFVRDamageable
{
  private const float RANDOMIZE_COLLIDER_SCALE = 0.002f;
  [HideInInspector]
  public int componentLabel = 1;
  [HideInInspector]
  public int lastComponentLabel = 1;
  public PTarget parentTarget;
  public IPTargetUnsafeFunctions unsafeFunctions;
  public MeshFilter meshFilter;
  public BoxCollider boxCollider;
  [HideInInspector]
  public int initialLabel = -1;
  [NonSerialized]
  private Mesh mesh;
  [NonSerialized]
  private bool isAttachedToTarget = true;
  [NonSerialized]
  private bool initialized;
  [NonSerialized]
  private bool firstUpdate = true;
  [NonSerialized]
  private float thickness = 0.05f;

  public void Initialize()
  {
    if (this.initialized)
      return;
    this.initialized = true;
    if ((UnityEngine.Object) this.parentTarget == (UnityEngine.Object) null)
    {
      MonoBehaviour.print((object) "No Parent target found, Please make sure the parentTarget variable is assigned before initializing the target piece.");
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
    else
    {
      this.unsafeFunctions = (IPTargetUnsafeFunctions) this.parentTarget;
      if ((UnityEngine.Object) this.meshFilter == (UnityEngine.Object) null)
        this.meshFilter = this.GetComponent<MeshFilter>();
      if ((UnityEngine.Object) this.meshFilter == (UnityEngine.Object) null)
      {
        UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
      else
      {
        if ((UnityEngine.Object) this.boxCollider == (UnityEngine.Object) null)
          this.boxCollider = this.GetComponent<BoxCollider>();
        if ((UnityEngine.Object) this.boxCollider == (UnityEngine.Object) null)
        {
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
        }
        else
        {
          this.thickness = this.parentTarget.targetPieceColliderThickness + UnityEngine.Random.Range(-1f / 500f, 1f / 500f);
          this.parentTarget.onTargetChange.Add((IOnPTargetChangeUnsafe) this);
          this.mesh = new Mesh();
          this.meshFilter.sharedMesh = this.mesh;
          this.lastComponentLabel = this.componentLabel;
          this.unsafeFunctions.ApplyLabelMesh(this.componentLabel, this.mesh);
        }
      }
    }
  }

  private void OnDestroy()
  {
    if (!((UnityEngine.Object) this.parentTarget != (UnityEngine.Object) null) || this.parentTarget.onTargetChange == null)
      return;
    this.parentTarget.onTargetChange.Remove((IOnPTargetChangeUnsafe) this);
  }

  void IOnPTargetChangeUnsafe.OnTargetChange()
  {
    this.Initialize();
    if ((UnityEngine.Object) this.parentTarget == (UnityEngine.Object) null)
      return;
    if (!this.firstUpdate)
    {
      int labelFromLastLabel = this.unsafeFunctions.GetCurrentLabelFromLastLabel(this.componentLabel);
      if (labelFromLastLabel != this.componentLabel)
        this.lastComponentLabel = this.componentLabel;
      this.componentLabel = labelFromLastLabel;
    }
    else
      this.initialLabel = this.unsafeFunctions.GetCurrentLabelFromLastLabel(this.componentLabel);
    this.firstUpdate = false;
    if (this.componentLabel == 0)
    {
      UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
    }
    else
    {
      this.unsafeFunctions.ApplyLabelMesh(this.componentLabel, this.mesh);
      Rect labelRect = this.unsafeFunctions.GetLabelRect(this.componentLabel);
      Vector3 center1 = this.boxCollider.center;
      Vector3 size1 = this.boxCollider.size;
      Vector3 center2 = (Vector3) labelRect.center;
      Vector3 size2 = (Vector3) labelRect.size;
      size2.z = this.thickness;
      if ((!Mathf.Approximately(center1.x, center2.x) || !Mathf.Approximately(center1.y, center2.y) || (!Mathf.Approximately(center1.z, center2.z) || !Mathf.Approximately(size1.x, size2.x)) || !Mathf.Approximately(size1.y, size2.y) ? 0 : (Mathf.Approximately(size1.z, size2.z) ? 1 : 0)) == 0)
      {
        this.boxCollider.center = (Vector3) labelRect.center;
        this.boxCollider.size = size2;
      }
      bool flag = this.unsafeFunctions.IsAttached(this.componentLabel);
      if (!this.isAttachedToTarget || flag)
        return;
      this.transform.SetParent((Transform) null);
      Rigidbody component = this.gameObject.GetComponent<Rigidbody>();
      component.isKinematic = false;
      component.mass = this.parentTarget.targetPieceMass;
      component.drag = this.parentTarget.targetPieceDrag;
      component.angularDrag = this.parentTarget.targetPieceAngularDrag;
      component.maxAngularVelocity = this.parentTarget.targetPieceMaxAngularVelocity;
      float angularVelocityScale = this.parentTarget.targetPieceRandomAngularVelocityScale;
      component.angularVelocity = new Vector3(UnityEngine.Random.Range(-angularVelocityScale, angularVelocityScale), UnityEngine.Random.Range(-angularVelocityScale, angularVelocityScale), UnityEngine.Random.Range(-angularVelocityScale, angularVelocityScale));
      this.isAttachedToTarget = flag;
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.matrix = this.transform.localToWorldMatrix;
    BoxCollider component = this.GetComponent<BoxCollider>();
    if ((UnityEngine.Object) component == (UnityEngine.Object) null)
      return;
    Gizmos.color = new Color(0.7f, 0.9f, 0.1f);
    Gizmos.DrawWireCube(component.center, component.size);
    Gizmos.color = new Color(0.7f, 0.9f, 0.1f, 0.25f);
    Gizmos.DrawWireCube(component.center, component.size);
  }

  public void AddBullet(Vector3 rayOrigin, Vector3 rayDir, float damageSize)
  {
    int decalIndex = 0;
    if ((UnityEngine.Object) this.parentTarget != (UnityEngine.Object) null)
    {
      PTargetProfile targetProfile = this.parentTarget.targetProfile;
      if ((UnityEngine.Object) targetProfile != (UnityEngine.Object) null)
        decalIndex = UnityEngine.Random.Range(0, targetProfile.bulletDecals.Length);
    }
    this.AddBullet(rayOrigin, rayDir, damageSize, decalIndex);
  }

  public void AddBullet(Vector3 rayOrigin, Vector3 rayDir, float damageSize, int decalIndex)
  {
    Transform transform = this.transform;
    Vector3 forward = transform.forward;
    if ((double) Vector3.Dot(rayDir, forward) < 0.0 && this.isAttachedToTarget)
      return;
    Vector3 position1 = transform.position;
    float enter;
    if (!new Plane(forward, position1).Raycast(new Ray(rayOrigin, rayDir), out enter))
      return;
    Vector3 position2 = rayOrigin + rayDir * enter;
    Vector2 uv = this.parentTarget.GetUV(transform.InverseTransformPoint(position2));
    this.parentTarget.AddBullet(this.componentLabel, decalIndex, uv, 0.0f, damageSize);
  }

  void IFVRDamageable.Damage(Damage dam)
  {
    Transform transform = this.transform;
    Vector3 strikeDir = dam.strikeDir;
    this.AddBullet(dam.point - strikeDir, strikeDir, dam.damageSize);
  }
}
