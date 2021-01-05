// Decompiled with JetBrains decompiler
// Type: FistVR.RotwienerNest
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RotwienerNest : ZosigQuestManager
  {
    public List<GameObject> Prefab_Nodules_Final;
    public List<Transform> NoduleSpawnPoints;
    private List<RotwienerNest_Nodule> m_finalNodules = new List<RotwienerNest_Nodule>();
    public List<RotwienerNest_Tendril> RootTendrils;
    public RotwienerNest.NestState State;
    [Header("Flag details")]
    public string AssociatedFlag;
    public int AssociatedFlagValue = 1;
    private bool m_isDestroyed;
    private ZosigGameManager M;
    [Header("StateGeo")]
    public GameObject Geo_Protected;
    public GameObject Geo_Exposed;
    public GameObject Geo_Destroyed;
    public Transform Bounce_Outer;
    public Transform Bounce_Inner;
    [Header("DestructionEvent")]
    public List<Transform> DestructionSpawnPoint;
    public List<GameObject> DestructionSpawns;
    private float m_bounceX;
    private float m_bounceY;
    private float m_bounceZ;
    private float m_checkTick = 0.1f;

    private void Bounce()
    {
      if (this.State == RotwienerNest.NestState.Protected)
      {
        this.m_bounceX = Mathf.Repeat(this.m_bounceX + Time.deltaTime * 0.3f, 1f);
        this.m_bounceY = Mathf.Repeat(this.m_bounceY + Time.deltaTime * 0.3f, 1f);
        this.m_bounceZ = Mathf.Repeat(this.m_bounceZ + Time.deltaTime * 0.3f, 1f);
        this.Bounce_Outer.localScale = new Vector3((float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceX * 3.14159274101257 * 2.0)) * 0.025000000372529), (float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceY * 3.14159274101257 * 2.0)) * 0.025000000372529), (float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceZ * 3.14159274101257 * 2.0)) * 0.025000000372529));
      }
      else
      {
        if (this.State != RotwienerNest.NestState.Exposed)
          return;
        this.m_bounceX = Mathf.Repeat(this.m_bounceX + Time.deltaTime * 0.5f, 1f);
        this.m_bounceY = Mathf.Repeat(this.m_bounceY + Time.deltaTime * 0.5f, 1f);
        this.m_bounceZ = Mathf.Repeat(this.m_bounceZ + Time.deltaTime * 0.5f, 1f);
        this.Bounce_Inner.localScale = new Vector3((float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceX * 3.14159274101257 * 2.0)) * 0.025000000372529), (float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceY * 3.14159274101257 * 2.0)) * 0.0500000007450581), (float) (1.0 + (double) Mathf.Sin((float) ((double) this.m_bounceZ * 3.14159274101257 * 2.0)) * 0.025000000372529));
      }
    }

    public override void Init(ZosigGameManager m)
    {
      this.M = m;
      this.InitializeFromFlagM();
      this.m_bounceY = 0.33f;
      this.m_bounceZ = 0.66f;
    }

    private void Test1() => this.SetState(RotwienerNest.NestState.Exposed, false);

    private void Test2() => this.SetState(RotwienerNest.NestState.Destroyed, false);

    private void InitializeFromFlagM()
    {
      if (this.M.FlagM.GetFlagValue(this.AssociatedFlag) >= this.AssociatedFlagValue)
      {
        if (GM.ZMaster.IsVerboseDebug)
          Debug.Log((object) (this.gameObject.name + " set to destroyed by flag:" + this.AssociatedFlag));
        for (int index = 0; index < this.RootTendrils.Count; ++index)
          this.RootTendrils[index].Init((RotwienerNest_Tendril) null, RotwienerNest_Tendril.TendrilState.Destroyed);
        this.SetState(RotwienerNest.NestState.Destroyed, true);
      }
      else
      {
        for (int index = 0; index < this.RootTendrils.Count; ++index)
          this.RootTendrils[index].Init((RotwienerNest_Tendril) null, RotwienerNest_Tendril.TendrilState.Protected);
        this.SetState(RotwienerNest.NestState.Protected, true);
      }
    }

    private void SetState(RotwienerNest.NestState s, bool isInit)
    {
      if (!isInit && this.State == s)
        return;
      this.State = s;
      switch (s)
      {
        case RotwienerNest.NestState.Protected:
          this.Geo_Protected.SetActive(true);
          this.Geo_Exposed.SetActive(false);
          this.Geo_Destroyed.SetActive(false);
          break;
        case RotwienerNest.NestState.Exposed:
          this.Geo_Protected.SetActive(false);
          this.Geo_Exposed.SetActive(true);
          this.Geo_Destroyed.SetActive(false);
          for (int index = 0; index < this.NoduleSpawnPoints.Count; ++index)
          {
            RotwienerNest_Nodule component = Object.Instantiate<GameObject>(this.Prefab_Nodules_Final[Random.Range(0, this.Prefab_Nodules_Final.Count)], this.NoduleSpawnPoints[index].position, this.NoduleSpawnPoints[index].rotation).GetComponent<RotwienerNest_Nodule>();
            component.SetState(RotwienerNest_Nodule.NoduleState.Naked, isInit);
            component.SetType(RotwienerNest_Nodule.NoduleType.Core, this, (RotwienerNest_Tendril) null);
            this.m_finalNodules.Add(component);
          }
          break;
        case RotwienerNest.NestState.Destroyed:
          this.Geo_Protected.SetActive(false);
          this.Geo_Exposed.SetActive(false);
          this.Geo_Destroyed.SetActive(true);
          if (this.M.FlagM.GetFlagValue(this.AssociatedFlag) < this.AssociatedFlagValue)
            this.M.FlagM.SetFlag(this.AssociatedFlag, this.AssociatedFlagValue);
          if (isInit)
            break;
          for (int index = 0; index < this.DestructionSpawns.Count; ++index)
            Object.Instantiate<GameObject>(this.DestructionSpawns[index], this.DestructionSpawnPoint[index].position, this.DestructionSpawnPoint[index].rotation);
          break;
      }
    }

    private void Update()
    {
      this.m_checkTick -= Time.deltaTime;
      if ((double) this.m_checkTick <= 0.0)
      {
        this.m_checkTick = Random.Range(0.1f, 0.3f);
        this.CheckState();
      }
      this.Bounce();
    }

    private void CheckState()
    {
      if (this.State == RotwienerNest.NestState.Protected)
      {
        if (!this.areAllTendrilsDestroyed())
          return;
        this.SetState(RotwienerNest.NestState.Exposed, false);
      }
      else
      {
        if (this.State != RotwienerNest.NestState.Exposed || !this.areAllNodulesDestroyed())
          return;
        this.SetState(RotwienerNest.NestState.Destroyed, false);
      }
    }

    private bool areAllNodulesDestroyed() => this.m_finalNodules.Count < 1;

    public void NoduleDestroyed(RotwienerNest_Nodule n)
    {
      this.m_finalNodules.Remove(n);
      Object.Destroy((Object) n.gameObject);
    }

    private bool areAllTendrilsDestroyed()
    {
      for (int index = 0; index < this.RootTendrils.Count; ++index)
      {
        if (this.RootTendrils[index].CanSupplyNodulePower())
          return false;
      }
      return true;
    }

    public enum NestState
    {
      Protected,
      Exposed,
      Destroyed,
    }
  }
}
