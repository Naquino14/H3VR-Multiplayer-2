// Decompiled with JetBrains decompiler
// Type: FistVR.RotwienerNest_Tendril
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class RotwienerNest_Tendril : MonoBehaviour
  {
    public GameObject Prefab_Nodule;
    public Transform Point_Nodule;
    private RotwienerNest_Nodule n_spawnedNodule;
    public float NoduleSize = 1f;
    public GameObject Geo_Whole;
    public GameObject Geo_Severed;
    public RotwienerNest_Tendril.TendrilState State;
    public RotwienerNest_Tendril ChildTendril;
    private RotwienerNest_Tendril m_parent;

    public void SetTendrilParent()
    {
    }

    public void Init(RotwienerNest_Tendril parent, RotwienerNest_Tendril.TendrilState state)
    {
      this.m_parent = parent;
      if ((Object) this.ChildTendril != (Object) null)
        this.ChildTendril.Init(this, state);
      if (state == RotwienerNest_Tendril.TendrilState.Protected)
      {
        this.SetState(RotwienerNest_Tendril.TendrilState.Protected, true);
      }
      else
      {
        if (state != RotwienerNest_Tendril.TendrilState.Destroyed)
          return;
        this.SetState(RotwienerNest_Tendril.TendrilState.Destroyed, true);
      }
    }

    public void SetState(RotwienerNest_Tendril.TendrilState s, bool isInit)
    {
      this.State = s;
      switch (s)
      {
        case RotwienerNest_Tendril.TendrilState.Protected:
          this.Geo_Whole.SetActive(true);
          this.Geo_Severed.SetActive(false);
          if (isInit)
          {
            GameObject gameObject = Object.Instantiate<GameObject>(this.Prefab_Nodule, this.Point_Nodule.position, this.Point_Nodule.rotation);
            RotwienerNest_Nodule component = gameObject.GetComponent<RotwienerNest_Nodule>();
            gameObject.transform.localScale = new Vector3(this.NoduleSize, this.NoduleSize, this.NoduleSize);
            this.n_spawnedNodule = component;
            this.n_spawnedNodule.SetType(RotwienerNest_Nodule.NoduleType.Tendril, (RotwienerNest) null, this);
          }
          if (!((Object) this.n_spawnedNodule != (Object) null))
            break;
          this.n_spawnedNodule.SetState(RotwienerNest_Nodule.NoduleState.Protected, isInit);
          break;
        case RotwienerNest_Tendril.TendrilState.Exposed:
          this.Geo_Whole.SetActive(true);
          this.Geo_Severed.SetActive(false);
          if (!((Object) this.n_spawnedNodule != (Object) null))
            break;
          this.n_spawnedNodule.SetState(RotwienerNest_Nodule.NoduleState.Unprotected, isInit);
          break;
        case RotwienerNest_Tendril.TendrilState.Destroyed:
          this.Geo_Whole.SetActive(false);
          this.Geo_Severed.SetActive(true);
          if (!((Object) this.n_spawnedNodule != (Object) null))
            break;
          this.n_spawnedNodule.SetState(RotwienerNest_Nodule.NoduleState.Destroyed, isInit);
          break;
      }
    }

    public void Update()
    {
      if (this.State != RotwienerNest_Tendril.TendrilState.Protected || !((Object) this.ChildTendril == (Object) null) && this.ChildTendril.State != RotwienerNest_Tendril.TendrilState.Destroyed)
        return;
      this.SetState(RotwienerNest_Tendril.TendrilState.Exposed, false);
    }

    public void NoduleDestroyed(RotwienerNest_Nodule n)
    {
      this.n_spawnedNodule = (RotwienerNest_Nodule) null;
      Object.Destroy((Object) n.gameObject);
      this.SetState(RotwienerNest_Tendril.TendrilState.Destroyed, false);
    }

    public bool CanSupplyNodulePower() => this.State != RotwienerNest_Tendril.TendrilState.Destroyed;

    public enum TendrilState
    {
      Protected,
      Exposed,
      Destroyed,
    }
  }
}
