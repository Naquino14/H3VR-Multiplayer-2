// Decompiled with JetBrains decompiler
// Type: FistVR.RessemblerOTron
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class RessemblerOTron : MonoBehaviour
  {
    private int m_numMeatCoresLoaded;
    public List<Renderer> MeatCoreIndiciators;
    public ParticleSystem PFX_GrindInsert;
    public AudioEvent AudEvent_Insert;
    public AudioEvent AudEvent_Fail;
    public AudioEvent AudEvent_Success;
    public FVRObject RessemblerCore;
    public Transform SpawnPoint;
    [Header("Accordian")]
    public Transform Accordian;
    private float m_accordianLerp;
    private bool m_isAnimating;
    public AnimationCurve AccordianCurve;
    public float AccordianSpeed = 1f;
    public AudioEvent AudEvent_Accordian;
    private bool m_insertedCoreThisFrame;
    private int[] ValueByType = new int[8]
    {
      2,
      2,
      2,
      2,
      3,
      3,
      4,
      5
    };

    private void OnTriggerEnter(Collider col) => this.TestCollider(col);

    private void TestCollider(Collider col)
    {
      if ((Object) col.attachedRigidbody == (Object) null)
        return;
      bool flag = false;
      RotrwMeatCore component1 = col.attachedRigidbody.gameObject.GetComponent<RotrwMeatCore>();
      RotrwMeatCore.CoreType type = component1.Type;
      if ((Object) component1 != (Object) null && !this.m_insertedCoreThisFrame)
      {
        if (this.m_numMeatCoresLoaded >= 10)
        {
          this.EjectIngredient((FVRPhysicalObject) component1);
        }
        else
        {
          this.GrindEffect();
          Object.Destroy((Object) component1.gameObject);
          this.MeatCoreInserted(type);
        }
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

    private void GrindEffect()
    {
      this.PFX_GrindInsert.Emit(20);
      SM.PlayGenericSound(this.AudEvent_Insert, this.transform.position);
    }

    private void MeatCoreInserted(RotrwMeatCore.CoreType ctype)
    {
      this.m_numMeatCoresLoaded += this.ValueByType[(int) ctype];
      this.m_insertedCoreThisFrame = true;
      this.UpdateIndicators();
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

    private void UpdateIndicators()
    {
      for (int index = 0; index < this.MeatCoreIndiciators.Count; ++index)
        this.MeatCoreIndiciators[index].enabled = index < this.m_numMeatCoresLoaded;
    }

    public void Grind(int derp)
    {
      if (this.m_numMeatCoresLoaded < 10)
      {
        SM.PlayGenericSound(this.AudEvent_Fail, this.transform.position);
      }
      else
      {
        SM.PlayGenericSound(this.AudEvent_Success, this.transform.position);
        SM.PlayGenericSound(this.AudEvent_Accordian, this.transform.position);
        this.m_accordianLerp = 0.0f;
        this.m_isAnimating = true;
        Object.Instantiate<GameObject>(this.RessemblerCore.GetGameObject(), this.SpawnPoint.position, this.SpawnPoint.rotation).GetComponent<RW_Powerup>();
        if ((Object) GM.ZMaster != (Object) null)
          GM.ZMaster.FlagM.AddToFlag("s_c", 1);
        this.m_numMeatCoresLoaded = 0;
        this.UpdateIndicators();
      }
    }

    private void Update()
    {
      this.m_insertedCoreThisFrame = false;
      this.Accordianing();
    }

    private void Accordianing()
    {
      if (!this.m_isAnimating)
        return;
      this.m_accordianLerp += Time.deltaTime;
      this.Accordian.localScale = new Vector3(1f, this.AccordianCurve.Evaluate(this.m_accordianLerp), 1f);
      if ((double) this.m_accordianLerp <= 1.0)
        return;
      this.m_isAnimating = false;
    }
  }
}
