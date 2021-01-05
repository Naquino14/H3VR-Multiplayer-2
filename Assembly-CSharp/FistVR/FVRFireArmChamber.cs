// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFireArmChamber
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFireArmChamber : FVRInteractiveObject
  {
    [Header("FireArm Chamber Config")]
    public FVRFireArm Firearm;
    public FireArmRoundType RoundType;
    public FVRFirearmAudioSet OverrideAudioSet;
    [Header("Chamber Params")]
    public bool IsManuallyExtractable;
    public bool IsManuallyChamberable;
    public float ChamberVelocityMultiplier = 1f;
    private FVRFireArmRound m_round;
    [Header("Chamber State")]
    public bool IsAccessible;
    public bool IsFull;
    public bool IsSpent;
    [Header("Proxy Stuff")]
    public GameObject LoadedPhys;
    public Transform ProxyRound;
    public MeshFilter ProxyMesh;
    public MeshRenderer ProxyRenderer;
    public bool DoesAutoChamber;
    public FireArmRoundClass AutoChamberClass = FireArmRoundClass.FMJ;

    public FVRFireArmRound GetRound() => this.m_round;

    [ContextMenu("checkmult")]
    public void checkmult()
    {
      if ((double) this.ChamberVelocityMultiplier == 1.0)
        return;
      Debug.Log((object) (this.transform.root.gameObject.name + " has mult of " + (object) this.ChamberVelocityMultiplier));
    }

    protected override void Awake()
    {
      base.Awake();
      GameObject gameObject = new GameObject("Proxy");
      this.ProxyRound = gameObject.transform;
      this.ProxyRound.SetParent(this.transform);
      this.ProxyRound.localPosition = Vector3.zero;
      this.ProxyRound.localEulerAngles = Vector3.zero;
      this.ProxyMesh = gameObject.AddComponent<MeshFilter>();
      this.ProxyRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    protected override void Start()
    {
      base.Start();
      if (!this.DoesAutoChamber)
        return;
      this.Autochamber(this.AutoChamberClass);
    }

    public void UpdateProxyDisplay()
    {
      if ((Object) this.m_round == (Object) null)
      {
        this.ProxyMesh.mesh = (Mesh) null;
        this.ProxyRenderer.material = (Material) null;
        this.ProxyRenderer.enabled = false;
      }
      else
      {
        if (this.IsSpent)
        {
          if ((Object) this.m_round.FiredRenderer != (Object) null)
          {
            this.ProxyMesh.mesh = this.m_round.FiredRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
            this.ProxyRenderer.material = this.m_round.FiredRenderer.sharedMaterial;
          }
          else
            this.ProxyMesh.mesh = (Mesh) null;
        }
        else
        {
          this.ProxyMesh.mesh = AM.GetRoundMesh(this.m_round.RoundType, this.m_round.RoundClass);
          this.ProxyRenderer.material = AM.GetRoundMaterial(this.m_round.RoundType, this.m_round.RoundClass);
        }
        this.ProxyRenderer.enabled = true;
      }
    }

    public void PlayChamberingAudio()
    {
      if ((Object) this.Firearm != (Object) null)
      {
        this.Firearm.PlayAudioEvent(FirearmAudioEventType.ChamberManual);
      }
      else
      {
        if (!((Object) this.OverrideAudioSet != (Object) null))
          return;
        SM.PlayCoreSound(FVRPooledAudioType.GenericClose, this.OverrideAudioSet.ChamberManual, this.transform.position);
      }
    }

    public override bool IsInteractable() => this.IsManuallyExtractable && this.IsAccessible && this.IsFull;

    public void Autochamber(FireArmRoundClass frClass)
    {
      if (!AM.DoesClassExistForType(frClass, this.RoundType))
        return;
      this.SetRound(AM.GetRoundSelfPrefab(this.RoundType, frClass).GetGameObject().GetComponent<FVRFireArmRound>());
    }

    public void Unload() => this.SetRound((FVRFireArmRound) null);

    public void SetRound(FVRFireArmRound round)
    {
      if ((Object) round != (Object) null)
      {
        this.IsFull = true;
        this.IsSpent = round.IsSpent;
        this.m_round = AM.GetRoundSelfPrefab(round.RoundType, round.RoundClass).GetGameObject().GetComponent<FVRFireArmRound>();
        if ((Object) this.LoadedPhys != (Object) null)
          this.LoadedPhys.SetActive(true);
      }
      else
      {
        this.IsFull = false;
        this.m_round = (FVRFireArmRound) null;
        if ((Object) this.LoadedPhys != (Object) null)
          this.LoadedPhys.SetActive(false);
      }
      this.UpdateProxyDisplay();
    }

    public FVRFireArmRound EjectRound(
      Vector3 EjectionPosition,
      Vector3 EjectionVelocity,
      Vector3 EjectionAngularVelocity,
      bool ForceCaseLessEject = false)
    {
      if (!((Object) this.m_round != (Object) null))
        return (FVRFireArmRound) null;
      bool flag = false;
      if ((Object) this.Firearm != (Object) null)
      {
        flag = true;
        if (this.Firearm.HasImpactController)
          this.Firearm.AudioImpactController.SetCollisionsTickDownMax(0.2f);
      }
      FVRFireArmRound fvrFireArmRound = (FVRFireArmRound) null;
      if (!this.m_round.IsCaseless || ForceCaseLessEject)
      {
        fvrFireArmRound = Object.Instantiate<GameObject>(this.m_round.gameObject, EjectionPosition, this.transform.rotation).GetComponent<FVRFireArmRound>();
        if (flag)
          fvrFireArmRound.SetIFF(GM.CurrentPlayerBody.GetPlayerIFF());
        fvrFireArmRound.RootRigidbody.velocity = Vector3.Lerp(EjectionVelocity * 0.7f, EjectionVelocity, Random.value) + GM.CurrentMovementManager.GetFilteredVel();
        fvrFireArmRound.RootRigidbody.maxAngularVelocity = 200f;
        fvrFireArmRound.RootRigidbody.angularVelocity = Vector3.Lerp(EjectionAngularVelocity * 0.3f, EjectionAngularVelocity, Random.value);
        if (this.IsSpent)
        {
          fvrFireArmRound.SetKillCounting(true);
          fvrFireArmRound.Fire();
        }
      }
      this.SetRound((FVRFireArmRound) null);
      return fvrFireArmRound;
    }

    public bool Fire()
    {
      if (!this.IsFull || !((Object) this.m_round != (Object) null) || this.IsSpent)
        return false;
      this.IsSpent = true;
      this.UpdateProxyDisplay();
      return true;
    }

    public override void BeginInteraction(FVRViveHand hand)
    {
      if (!this.IsManuallyExtractable || !this.IsAccessible || !(this.IsFull & (Object) this.m_round != (Object) null))
        return;
      FVRFireArmRound fvrFireArmRound = this.EjectRound(hand.Input.Pos, Vector3.zero, Vector3.zero);
      if (!((Object) fvrFireArmRound != (Object) null))
        return;
      fvrFireArmRound.BeginInteraction(hand);
      hand.ForceSetInteractable((FVRInteractiveObject) fvrFireArmRound);
      this.SetRound((FVRFireArmRound) null);
    }
  }
}
