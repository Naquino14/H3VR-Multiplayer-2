// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_MagDuplicator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TNH_MagDuplicator : MonoBehaviour
  {
    public TNH_Manager M;
    public Transform Spawnpoint_Mag;
    public TNH_ObjectConstructorIcon OCIcon;
    public Transform ScanningVolume;
    public LayerMask ScanningLM;
    public AudioEvent AudEvent_Fail;
    public AudioEvent AudEvent_Spawn;
    private FVRFireArmMagazine m_detectedMag;
    private FVRFireArmClip m_detectedClip;
    private Speedloader m_detectedSL;
    private Collider[] colbuffer;
    private int m_storedCost;
    private float m_scanTick = 1f;

    private void Start() => this.colbuffer = new Collider[50];

    public void Button_Duplicate()
    {
      if ((Object) this.m_detectedMag == (Object) null && (Object) this.m_detectedClip == (Object) null && (Object) this.m_detectedSL == (Object) null)
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Fail, this.transform.position);
      else if (this.M.GetNumTokens() >= this.m_storedCost)
      {
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Spawn, this.transform.position);
        this.M.SubtractTokens(this.m_storedCost);
        this.M.Increment(10, false);
        if ((Object) this.m_detectedMag != (Object) null)
        {
          FVRFireArmMagazine component = Object.Instantiate<GameObject>(this.m_detectedMag.ObjectWrapper.GetGameObject(), this.Spawnpoint_Mag.position, this.Spawnpoint_Mag.rotation).GetComponent<FVRFireArmMagazine>();
          for (int index = 0; index < Mathf.Min(this.m_detectedMag.LoadedRounds.Length, component.LoadedRounds.Length); ++index)
          {
            if (this.m_detectedMag.LoadedRounds[index] != null && (Object) this.m_detectedMag.LoadedRounds[index].LR_Mesh != (Object) null)
            {
              component.LoadedRounds[index].LR_Class = this.m_detectedMag.LoadedRounds[index].LR_Class;
              component.LoadedRounds[index].LR_Mesh = this.m_detectedMag.LoadedRounds[index].LR_Mesh;
              component.LoadedRounds[index].LR_Material = this.m_detectedMag.LoadedRounds[index].LR_Material;
              component.LoadedRounds[index].LR_ObjectWrapper = this.m_detectedMag.LoadedRounds[index].LR_ObjectWrapper;
            }
          }
          component.m_numRounds = this.m_detectedMag.m_numRounds;
          component.UpdateBulletDisplay();
        }
        else if ((Object) this.m_detectedClip != (Object) null)
        {
          FVRFireArmClip component = Object.Instantiate<GameObject>(this.m_detectedClip.ObjectWrapper.GetGameObject(), this.Spawnpoint_Mag.position, this.Spawnpoint_Mag.rotation).GetComponent<FVRFireArmClip>();
          for (int index = 0; index < Mathf.Min(this.m_detectedClip.LoadedRounds.Length, component.LoadedRounds.Length); ++index)
          {
            if (this.m_detectedClip.LoadedRounds[index] != null && (Object) this.m_detectedClip.LoadedRounds[index].LR_Mesh != (Object) null)
            {
              component.LoadedRounds[index].LR_Class = this.m_detectedClip.LoadedRounds[index].LR_Class;
              component.LoadedRounds[index].LR_Mesh = this.m_detectedClip.LoadedRounds[index].LR_Mesh;
              component.LoadedRounds[index].LR_Material = this.m_detectedClip.LoadedRounds[index].LR_Material;
              component.LoadedRounds[index].LR_ObjectWrapper = this.m_detectedClip.LoadedRounds[index].LR_ObjectWrapper;
            }
          }
          component.m_numRounds = this.m_detectedClip.m_numRounds;
          component.UpdateBulletDisplay();
        }
        else
        {
          if (!((Object) this.m_detectedSL != (Object) null))
            return;
          Speedloader component = Object.Instantiate<GameObject>(this.m_detectedSL.ObjectWrapper.GetGameObject(), this.Spawnpoint_Mag.position, this.Spawnpoint_Mag.rotation).GetComponent<Speedloader>();
          for (int index = 0; index < this.m_detectedSL.Chambers.Count; ++index)
          {
            if (this.m_detectedSL.Chambers[index].IsLoaded)
            {
              component.Chambers[index].Load(this.m_detectedSL.Chambers[index].LoadedClass);
            }
            else
            {
              int num = (int) component.Chambers[index].Unload();
            }
          }
        }
      }
      else
        SM.PlayCoreSound(FVRPooledAudioType.UIChirp, this.AudEvent_Fail, this.transform.position);
    }

    private void Update()
    {
      this.m_scanTick -= Time.deltaTime;
      if ((double) this.m_scanTick > 0.0)
        return;
      this.m_scanTick = Random.Range(0.8f, 1f);
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) >= 12.0)
        return;
      this.Scan();
    }

    private void Scan()
    {
      int num = Physics.OverlapBoxNonAlloc(this.ScanningVolume.position, this.ScanningVolume.localScale * 0.5f, this.colbuffer, this.ScanningVolume.rotation, (int) this.ScanningLM, QueryTriggerInteraction.Collide);
      this.m_detectedMag = (FVRFireArmMagazine) null;
      this.m_detectedClip = (FVRFireArmClip) null;
      this.m_detectedSL = (Speedloader) null;
      for (int index = 0; index < num; ++index)
      {
        if ((Object) this.colbuffer[index].attachedRigidbody != (Object) null)
        {
          FVRFireArmMagazine component1 = this.colbuffer[index].attachedRigidbody.gameObject.GetComponent<FVRFireArmMagazine>();
          if ((Object) component1 != (Object) null && (Object) component1.FireArm == (Object) null && (!component1.IsHeld && (Object) component1.QuickbeltSlot == (Object) null))
          {
            this.m_detectedMag = component1;
            break;
          }
          FVRFireArmClip component2 = this.colbuffer[index].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>();
          if ((Object) component2 != (Object) null && (Object) component2.FireArm == (Object) null && (!component2.IsHeld && (Object) component2.QuickbeltSlot == (Object) null))
          {
            this.m_detectedClip = component2;
            break;
          }
          Speedloader component3 = this.colbuffer[index].attachedRigidbody.gameObject.GetComponent<Speedloader>();
          if ((Object) component3 != (Object) null && !component3.IsHeld && (Object) component3.QuickbeltSlot == (Object) null)
          {
            this.m_detectedSL = component3;
            break;
          }
        }
      }
      this.SetCostBasedOnMag();
    }

    private void SetCostBasedOnMag()
    {
      if ((Object) this.m_detectedMag == (Object) null && (Object) this.m_detectedClip == (Object) null && (Object) this.m_detectedSL == (Object) null)
      {
        this.OCIcon.SetOption(TNH_ObjectConstructorIcon.IconState.Cancel, this.OCIcon.Sprite_Cancel, 0);
        this.m_storedCost = 0;
      }
      else
      {
        int cost = 1;
        if ((Object) this.m_detectedMag != (Object) null)
        {
          if (this.m_detectedMag.m_capacity > 100)
            cost = 8;
          else if (this.m_detectedMag.m_capacity > 50)
            cost = 5;
          else if (this.m_detectedMag.m_capacity > 30)
            cost = 3;
          else if (this.m_detectedMag.m_capacity > 15)
            cost = 2;
          switch (AM.GetRoundPower(this.m_detectedMag.RoundType))
          {
            case FVRObject.OTagFirearmRoundPower.Shotgun:
              ++cost;
              break;
            case FVRObject.OTagFirearmRoundPower.Intermediate:
              ++cost;
              break;
            case FVRObject.OTagFirearmRoundPower.FullPower:
              cost += 2;
              break;
            case FVRObject.OTagFirearmRoundPower.AntiMaterial:
              cost += 3;
              break;
            case FVRObject.OTagFirearmRoundPower.Ordnance:
              cost += 4;
              break;
            case FVRObject.OTagFirearmRoundPower.Exotic:
              cost += 3;
              break;
          }
        }
        else if ((Object) this.m_detectedClip != (Object) null)
        {
          switch (AM.GetRoundPower(this.m_detectedClip.RoundType))
          {
            case FVRObject.OTagFirearmRoundPower.Shotgun:
              ++cost;
              break;
            case FVRObject.OTagFirearmRoundPower.Intermediate:
              ++cost;
              break;
            case FVRObject.OTagFirearmRoundPower.FullPower:
              cost += 2;
              break;
            case FVRObject.OTagFirearmRoundPower.AntiMaterial:
              cost += 3;
              break;
            case FVRObject.OTagFirearmRoundPower.Ordnance:
              cost += 2;
              break;
            case FVRObject.OTagFirearmRoundPower.Exotic:
              cost += 3;
              break;
          }
        }
        else if ((Object) this.m_detectedSL != (Object) null)
        {
          switch (AM.GetRoundPower(this.m_detectedSL.Chambers[0].Type))
          {
            case FVRObject.OTagFirearmRoundPower.Shotgun:
              ++cost;
              break;
            case FVRObject.OTagFirearmRoundPower.Intermediate:
              ++cost;
              break;
            case FVRObject.OTagFirearmRoundPower.FullPower:
              cost += 2;
              break;
            case FVRObject.OTagFirearmRoundPower.AntiMaterial:
              cost += 3;
              break;
            case FVRObject.OTagFirearmRoundPower.Ordnance:
              cost += 2;
              break;
            case FVRObject.OTagFirearmRoundPower.Exotic:
              cost += 3;
              break;
          }
        }
        this.OCIcon.SetOption(TNH_ObjectConstructorIcon.IconState.Item, this.OCIcon.Sprite_Accept, cost);
        this.m_storedCost = cost;
      }
    }
  }
}
