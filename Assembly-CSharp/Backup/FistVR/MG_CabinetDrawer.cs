// Decompiled with JetBrains decompiler
// Type: FistVR.MG_CabinetDrawer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class MG_CabinetDrawer : FVRInteractiveObject
  {
    [Header("Cabinet Params")]
    public Rigidbody RB;
    private Vector3 m_appliedForce = Vector3.zero;
    public Transform ItemPoint;
    public float XZRangeFromPoint = 0.1f;
    public bool CanBeSpawnedInto = true;

    protected override void Awake()
    {
      base.Awake();
      this.RB = this.GetComponent<Rigidbody>();
    }

    public void Init()
    {
      this.transform.localPosition = new Vector3(0.0f, this.transform.localPosition.y, Mathf.Clamp(Random.Range(-0.2f, 0.3f), 0.2f, 0.5f));
      this.ItemPoint.localEulerAngles = new Vector3(0.0f, Random.Range(0.0f, 360f), -90f);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      base.UpdateInteraction(hand);
      this.m_appliedForce = hand.transform.position - this.transform.position;
      this.m_appliedForce = Vector3.Project(this.m_appliedForce, this.transform.forward);
      float num = Mathf.Clamp(this.m_appliedForce.magnitude * 2f, 0.0f, 1f);
      this.m_appliedForce = Vector3.ClampMagnitude(this.m_appliedForce, num * num);
    }

    public override void EndInteraction(FVRViveHand hand)
    {
      this.m_appliedForce = Vector3.zero;
      base.EndInteraction(hand);
    }

    protected override void FVRFixedUpdate()
    {
      base.FVRFixedUpdate();
      if (!this.IsHeld)
        return;
      this.RB.AddForce(this.m_appliedForce, ForceMode.Impulse);
    }

    public void SpawnIntoCabinet(GameObject go)
    {
      go.transform.position = new Vector3(this.ItemPoint.position.x + Random.Range(-this.XZRangeFromPoint, this.XZRangeFromPoint), this.ItemPoint.position.y, this.ItemPoint.position.z + Random.Range(-this.XZRangeFromPoint, this.XZRangeFromPoint));
      go.transform.rotation = this.ItemPoint.rotation;
      this.CanBeSpawnedInto = false;
    }

    public void SpawnIntoCabinet(GameObject[] gos)
    {
      for (int index = 0; index < gos.Length; ++index)
      {
        gos[index].transform.position = new Vector3(this.ItemPoint.position.x + Random.Range(-this.XZRangeFromPoint, this.XZRangeFromPoint), this.ItemPoint.position.y, this.ItemPoint.position.z + Random.Range(-this.XZRangeFromPoint, this.XZRangeFromPoint));
        gos[index].transform.rotation = this.ItemPoint.rotation;
      }
      this.CanBeSpawnedInto = false;
    }
  }
}
