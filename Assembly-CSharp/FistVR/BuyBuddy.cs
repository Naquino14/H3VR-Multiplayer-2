// Decompiled with JetBrains decompiler
// Type: FistVR.BuyBuddy
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class BuyBuddy : MonoBehaviour
  {
    public Text ItemReadout;
    public List<Image> MeatCoresRequiredArray;
    public List<Sprite> MeatCoreSprites;
    private FVRObject m_storedObject1;
    private FVRObject m_storedObject2;
    public Transform CasePosition;
    public Transform CasePosition_Small;
    public FVRObject LargeCase;
    public FVRObject SmallCase;
    private List<GameObject> m_spawnedObjects = new List<GameObject>();
    private ZosigContainer_WeaponCase guncase;
    [Header("Audio")]
    public AudioEvent AudEvent_Buy;
    public ParticleSystem PFX_GrindInsert;
    public AudioEvent AudEvent_Insert;
    private bool m_insertedCoreThisFrame;
    public ZosigFlagManager F;
    private List<int> m_numCoresLeftToUnlock = new List<int>()
    {
      0,
      0,
      0,
      0,
      0,
      0,
      0,
      0
    };
    private string m_flagWhenPurchased = string.Empty;
    private float reFireLimit = 0.1f;

    public void ConfigureBuddy(
      ObjectTableDef tableDef,
      bool isLargeCase,
      string flag,
      List<int> pricetag)
    {
      this.F = GM.ZMaster.FlagM;
      this.m_flagWhenPurchased = flag;
      Transform transform = this.CasePosition;
      GameObject gameObject1;
      if (isLargeCase)
      {
        gameObject1 = this.LargeCase.GetGameObject();
      }
      else
      {
        gameObject1 = this.SmallCase.GetGameObject();
        transform = this.CasePosition_Small;
      }
      GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject1, transform.position, transform.rotation);
      if ((Object) gameObject2.GetComponent<ZosigContainer>() != (Object) null)
      {
        ZosigContainer component = gameObject2.GetComponent<ZosigContainer>();
        component.PlaceObjectsInContainer((FVRObject) null);
        this.guncase = component as ZosigContainer_WeaponCase;
        this.guncase.Cover.LockCase();
      }
      ObjectTable objectTable = new ObjectTable();
      objectTable.Initialize(tableDef);
      FVRObject randomObject = objectTable.GetRandomObject();
      int minAmmoCapacity = tableDef.MinAmmoCapacity;
      int maxAmmoCapacity = tableDef.MaxAmmoCapacity;
      this.guncase.PlaceObjectsInContainer((FVRObject) null, -1, 30);
      this.guncase.LoadIntoCrate(randomObject, minAmmoCapacity, maxAmmoCapacity);
      if (this.F.GetFlagValue(flag) > 0)
      {
        this.CasePurchased();
      }
      else
      {
        for (int index = 0; index < pricetag.Count; ++index)
          this.m_numCoresLeftToUnlock[index] = pricetag[index];
      }
      this.ItemReadout.text = tableDef.name;
      this.UpdatePriceDisplay();
    }

    private void GrindEffect()
    {
      this.PFX_GrindInsert.Emit(20);
      SM.PlayGenericSound(this.AudEvent_Insert, this.transform.position);
    }

    private void MeatCoreInserted(RotrwMeatCore.CoreType t)
    {
      this.m_insertedCoreThisFrame = true;
      this.m_numCoresLeftToUnlock[(int) t] = this.m_numCoresLeftToUnlock[(int) t] - 1;
      this.UpdatePriceDisplay();
      this.CheckIfPurchased();
    }

    private void CheckIfPurchased()
    {
      bool flag = true;
      for (int index = 0; index < this.m_numCoresLeftToUnlock.Count; ++index)
      {
        if (this.m_numCoresLeftToUnlock[index] > 0)
        {
          flag = false;
          break;
        }
      }
      if (!flag)
        return;
      SM.PlayGenericSound(this.AudEvent_Buy, this.transform.position);
      this.CasePurchased();
      GM.ZMaster.FlagM.SetFlag(this.m_flagWhenPurchased, 1);
    }

    private bool NeedsType(RotrwMeatCore.CoreType t) => this.m_numCoresLeftToUnlock[(int) t] > 0;

    private void OnTriggerEnter(Collider col)
    {
      if ((double) this.reFireLimit > 0.0)
        return;
      this.TestCollider(col);
    }

    private void Update()
    {
      this.m_insertedCoreThisFrame = false;
      if ((double) this.reFireLimit < 0.0)
        return;
      this.reFireLimit -= Time.deltaTime;
    }

    private void TestCollider(Collider col)
    {
      if ((Object) col.attachedRigidbody == (Object) null)
        return;
      bool flag = false;
      RotrwMeatCore component1 = col.attachedRigidbody.gameObject.GetComponent<RotrwMeatCore>();
      RotrwMeatCore.CoreType type = component1.Type;
      if ((Object) component1 != (Object) null && !this.m_insertedCoreThisFrame)
      {
        if (this.NeedsType(component1.Type))
        {
          this.GrindEffect();
          Object.Destroy((Object) component1.gameObject);
          this.MeatCoreInserted(type);
        }
        else
          this.EjectIngredient((FVRPhysicalObject) component1);
        flag = true;
      }
      if (flag)
        return;
      if ((Object) col.attachedRigidbody.GetComponent<FVRPhysicalObject>() != (Object) null)
      {
        FVRPhysicalObject component2 = col.attachedRigidbody.gameObject.GetComponent<FVRPhysicalObject>();
        if (component2.IsHeld)
        {
          FVRViveHand hand = component2.m_hand;
          component2.EndInteraction(hand);
          hand.ForceSetInteractable((FVRInteractiveObject) null);
        }
      }
      col.attachedRigidbody.velocity = Vector3.up * 4f + Random.onUnitSphere * 1.5f;
    }

    private void EjectIngredient(FVRPhysicalObject obj)
    {
      if (obj.IsHeld)
      {
        FVRViveHand hand = obj.m_hand;
        obj.EndInteraction(hand);
        hand.ForceSetInteractable((FVRInteractiveObject) null);
      }
      obj.RootRigidbody.velocity = Vector3.up * 4f + Random.onUnitSphere * 1.5f;
    }

    private void CasePurchased() => this.guncase.Cover.UnlockCase();

    private void UpdatePriceDisplay()
    {
      int index1 = 0;
      for (int index2 = this.m_numCoresLeftToUnlock.Count - 1; index2 >= 0; --index2)
      {
        if (this.m_numCoresLeftToUnlock[index2] >= 1)
        {
          for (int index3 = 0; index3 < this.m_numCoresLeftToUnlock[index2] && index1 < this.MeatCoresRequiredArray.Count; ++index3)
          {
            this.MeatCoresRequiredArray[index1].gameObject.SetActive(true);
            this.MeatCoresRequiredArray[index1].sprite = this.MeatCoreSprites[index2];
            ++index1;
          }
        }
      }
      for (int index2 = index1; index2 < this.MeatCoresRequiredArray.Count; ++index2)
        this.MeatCoresRequiredArray[index2].gameObject.SetActive(false);
    }
  }
}
