// Decompiled with JetBrains decompiler
// Type: FistVR.Reloadamatic
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class Reloadamatic : MonoBehaviour
  {
    public Transform Accordian;
    private float m_accordianLerp;
    private bool m_isAnimating;
    public AnimationCurve AccordianCurve;
    public float AccordianSpeed = 1f;
    public Text DisplayText;
    public AudioEvent AudEvent_Accordian;
    public AudioEvent AudEvent_ReloadSuccess;
    public AudioEvent AudEvent_ReloadFailure;
    public bool SpawnsDefault = true;
    private Dictionary<FireArmRoundType, FireArmRoundClass> m_decidedTypes = new Dictionary<FireArmRoundType, FireArmRoundClass>();
    private bool m_hasDispensibleRound;
    private FireArmRoundType m_currentDispenseType;
    private AttachableFirearmPhysicalObject m_detectedAttachedFirearm;
    private FVRFireArmMagazine m_detectedMagazine;
    private FVRFireArmClip m_detectedClip;
    private Speedloader m_detectedSpeedloader;
    public Transform SpawnedRoundPoint;
    public LayerMask LM_MagDetectOverlay;
    public Transform MagDetectCenter;
    public Vector3 MagDetectExtends;
    private Collider[] colbuffer;
    private float m_scanTick = 1f;

    public void Start()
    {
      this.UpdateDisplay(true, FireArmRoundType.a9_18_Makarov, FireArmRoundClass.JHP);
      this.colbuffer = new Collider[50];
    }

    public void SetSpawnsDefault(bool b) => this.SpawnsDefault = b;

    private FireArmRoundClass GetClassFromType(FireArmRoundType t)
    {
      if (!this.m_decidedTypes.ContainsKey(t))
      {
        if (this.SpawnsDefault)
        {
          FireArmRoundClass defaultRoundClass = AM.GetDefaultRoundClass(t);
          this.m_decidedTypes.Add(t, defaultRoundClass);
        }
        else
        {
          FireArmRoundClass defaultRoundClass = AM.GetRandomNonDefaultRoundClass(t);
          this.m_decidedTypes.Add(t, defaultRoundClass);
        }
      }
      return this.m_decidedTypes[t];
    }

    public void UpdateDisplay(
      bool shouldScan,
      FireArmRoundType RoundType,
      FireArmRoundClass RoundClass)
    {
      if (shouldScan)
        this.DisplayText.text = "Place Gun, Mag, Clip\nor Speedloader On Bed";
      else
        this.DisplayText.text = AM.GetFullRoundName(RoundType, RoundClass);
    }

    public void DispenseRound(int v)
    {
      if (this.m_hasDispensibleRound)
      {
        Object.Instantiate<GameObject>(AM.GetRoundSelfPrefab(this.m_currentDispenseType, this.GetClassFromType(this.m_currentDispenseType)).GetGameObject(), this.SpawnedRoundPoint.position, this.SpawnedRoundPoint.rotation);
        this.m_accordianLerp = 0.0f;
        this.m_isAnimating = true;
        SM.PlayGenericSound(this.AudEvent_Accordian, this.transform.position);
      }
      else
        SM.PlayGenericSound(this.AudEvent_ReloadFailure, this.transform.position);
    }

    public void Reload(int v)
    {
      this.Scan();
      bool flag = false;
      if ((Object) this.m_detectedMagazine != (Object) null)
      {
        this.m_detectedMagazine.ReloadMagWithType(this.GetClassFromType(this.m_currentDispenseType));
        flag = true;
      }
      else if ((Object) this.m_detectedClip != (Object) null)
      {
        this.m_detectedClip.ReloadClipWithType(this.GetClassFromType(this.m_currentDispenseType));
        flag = true;
      }
      else if ((Object) this.m_detectedSpeedloader != (Object) null)
      {
        this.m_detectedSpeedloader.ReloadClipWithType(this.GetClassFromType(this.m_currentDispenseType));
        flag = true;
      }
      if (flag)
      {
        this.m_accordianLerp = 0.0f;
        this.m_isAnimating = true;
        SM.PlayGenericSound(this.AudEvent_Accordian, this.transform.position);
        SM.PlayGenericSound(this.AudEvent_ReloadSuccess, this.transform.position);
      }
      else
        SM.PlayGenericSound(this.AudEvent_ReloadFailure, this.transform.position);
    }

    private void Update()
    {
      this.Accordianing();
      this.m_scanTick -= Time.deltaTime;
      if ((double) this.m_scanTick > 0.0)
        return;
      this.m_scanTick = Random.Range(0.8f, 1f);
      if ((double) Vector3.Distance(this.transform.position, GM.CurrentPlayerBody.transform.position) >= 12.0)
        return;
      this.Scan();
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

    private void ClearScanned()
    {
      this.m_hasDispensibleRound = false;
      this.m_detectedAttachedFirearm = (AttachableFirearmPhysicalObject) null;
      this.m_detectedMagazine = (FVRFireArmMagazine) null;
      this.m_detectedClip = (FVRFireArmClip) null;
      this.m_detectedSpeedloader = (Speedloader) null;
    }

    private void SetScanned(FVRFireArm f)
    {
      this.m_hasDispensibleRound = true;
      this.SetDispenseType(f.RoundType);
    }

    private void SetScanned(AttachableFirearmPhysicalObject af)
    {
      this.m_hasDispensibleRound = true;
      this.SetDispenseType(af.FA.RoundType);
      this.m_detectedAttachedFirearm = af;
    }

    private void SetScanned(FVRFireArmMagazine m)
    {
      this.m_hasDispensibleRound = true;
      this.SetDispenseType(m.RoundType);
      this.m_detectedMagazine = m;
    }

    private void SetScanned(FVRFireArmClip c)
    {
      this.m_hasDispensibleRound = true;
      this.SetDispenseType(c.RoundType);
      this.m_detectedClip = c;
    }

    private void SetScanned(Speedloader s)
    {
      this.m_hasDispensibleRound = true;
      this.SetDispenseType(s.Chambers[0].Type);
      this.m_detectedSpeedloader = s;
    }

    private void SetDispenseType(FireArmRoundType t)
    {
      this.m_currentDispenseType = t;
      this.UpdateDisplay(false, t, this.GetClassFromType(t));
    }

    private void Scan()
    {
      int num = Physics.OverlapBoxNonAlloc(this.MagDetectCenter.position, this.MagDetectExtends, this.colbuffer, this.MagDetectCenter.rotation, (int) this.LM_MagDetectOverlay, QueryTriggerInteraction.Collide);
      this.ClearScanned();
      for (int index = 0; index < num; ++index)
      {
        if ((Object) this.colbuffer[index].attachedRigidbody != (Object) null)
        {
          FVRFireArm component1 = this.colbuffer[index].attachedRigidbody.gameObject.GetComponent<FVRFireArm>();
          if ((Object) component1 != (Object) null)
          {
            if ((Object) component1.Magazine == (Object) null)
            {
              this.SetScanned(component1);
              break;
            }
            this.SetScanned(component1.Magazine);
            break;
          }
          AttachableFirearmPhysicalObject component2 = this.colbuffer[index].attachedRigidbody.gameObject.GetComponent<AttachableFirearmPhysicalObject>();
          if ((Object) component2 != (Object) null)
          {
            this.SetScanned(component2);
            break;
          }
          FVRFireArmMagazine component3 = this.colbuffer[index].attachedRigidbody.gameObject.GetComponent<FVRFireArmMagazine>();
          if ((Object) component3 != (Object) null && (Object) component3.FireArm == (Object) null)
          {
            this.SetScanned(component3);
            break;
          }
          FVRFireArmClip component4 = this.colbuffer[index].attachedRigidbody.gameObject.GetComponent<FVRFireArmClip>();
          if ((Object) component4 != (Object) null && (Object) component4.FireArm == (Object) null)
          {
            this.SetScanned(component4);
            break;
          }
          Speedloader component5 = this.colbuffer[index].attachedRigidbody.gameObject.GetComponent<Speedloader>();
          if ((Object) component5 != (Object) null)
          {
            this.SetScanned(component5);
            break;
          }
        }
      }
    }
  }
}
