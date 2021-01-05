// Decompiled with JetBrains decompiler
// Type: FistVR.RedDotSight
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class RedDotSight : FVRFireArmAttachmentInterface
  {
    private int m_zeroDistanceIndex = 3;
    private float[] m_zeroDistances = new float[7]
    {
      2f,
      5f,
      10f,
      15f,
      25f,
      50f,
      100f
    };
    public Transform TargetAimer;
    public Text ZeroingText;
    private float m_staticDistance = 10f;
    public Transform BackupMuzzle;
    public bool UsesBrightnessSettings;
    public int CurrentBrightnessSetting;
    public Renderer BrightnessRend;
    [ColorUsage(true, true, 0.0f, 30f, 0.125f, 3f)]
    public List<Color> Colors;
    public RedDotSight MigrateFromObj;

    protected override void Awake()
    {
      base.Awake();
      this.Zero();
    }

    private void CycleBrightness()
    {
      ++this.CurrentBrightnessSetting;
      if (this.CurrentBrightnessSetting >= this.Colors.Count)
        this.CurrentBrightnessSetting = 0;
      this.UpdateBrightness();
    }

    private void UpdateBrightness()
    {
      if (!this.UsesBrightnessSettings)
        return;
      this.BrightnessRend.material.SetColor("_Color", this.Colors[this.CurrentBrightnessSetting]);
    }

    public override void UpdateInteraction(FVRViveHand hand)
    {
      Vector2 touchpadAxes = hand.Input.TouchpadAxes;
      if (hand.Input.TouchpadDown && (double) touchpadAxes.magnitude > 0.25)
      {
        if ((double) Vector2.Angle(touchpadAxes, Vector2.left) <= 45.0)
        {
          this.DecreaseZeroDistance();
          this.Zero();
        }
        else if ((double) Vector2.Angle(touchpadAxes, Vector2.right) <= 45.0)
        {
          this.IncreaseZeroDistance();
          this.Zero();
        }
        else if (!this.UsesBrightnessSettings || (double) Vector2.Angle(touchpadAxes, Vector2.up) > 45.0)
          ;
      }
      base.UpdateInteraction(hand);
    }

    private void IncreaseZeroDistance()
    {
      ++this.m_zeroDistanceIndex;
      this.m_zeroDistanceIndex = Mathf.Clamp(this.m_zeroDistanceIndex, 0, this.m_zeroDistances.Length - 1);
    }

    private void DecreaseZeroDistance()
    {
      --this.m_zeroDistanceIndex;
      this.m_zeroDistanceIndex = Mathf.Clamp(this.m_zeroDistanceIndex, 0, this.m_zeroDistances.Length - 1);
    }

    private void Zero()
    {
      if ((Object) this.Attachment != (Object) null && (Object) this.Attachment.curMount != (Object) null && ((Object) this.Attachment.curMount.Parent != (Object) null && this.Attachment.curMount.Parent is FVRFireArm))
      {
        FVRFireArm parent = this.Attachment.curMount.Parent as FVRFireArm;
        this.TargetAimer.LookAt(parent.MuzzlePos.position + parent.MuzzlePos.forward * this.m_zeroDistances[this.m_zeroDistanceIndex], Vector3.up);
      }
      else if ((Object) this.BackupMuzzle != (Object) null)
        this.TargetAimer.LookAt(this.BackupMuzzle.position + this.BackupMuzzle.forward * this.m_zeroDistances[this.m_zeroDistanceIndex], Vector3.up);
      else
        this.TargetAimer.localRotation = Quaternion.identity;
      this.ZeroingText.text = "Zero Distance: " + this.m_zeroDistances[this.m_zeroDistanceIndex].ToString() + "m";
    }

    public override void OnAttach()
    {
      base.OnAttach();
      this.Zero();
    }

    public override void OnDetach()
    {
      base.OnDetach();
      this.Zero();
    }

    [ContextMenu("MigrateFrom")]
    public void MigrateFrom()
    {
      this.UsesBrightnessSettings = true;
      this.Colors = this.MigrateFromObj.Colors;
      this.MigrateFromObj = (RedDotSight) null;
    }
  }
}
