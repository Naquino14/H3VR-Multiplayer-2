// Decompiled with JetBrains decompiler
// Type: FistVR.AIBodyPiece
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class AIBodyPiece : FVRDestroyableObject
  {
    [Header("AI Body Piece Config")]
    public AIDamagePlate[] DamagePlates;
    public bool UsesPlateSystem = true;
    public bool IsPlateDamaged;
    public bool IsPlateDisabled;

    public override void Awake() => base.Awake();

    public override void Update()
    {
      base.Update();
      if (!this.UsesPlateSystem)
        return;
      this.CheckPlateDamage();
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void CheckPlateDamage()
    {
      int num = 0;
      for (int index = 0; index < this.DamagePlates.Length; ++index)
      {
        if (this.DamagePlates[index].IsDown)
          ++num;
      }
      if (num > 0)
        this.SetPlateDamaged(true);
      else
        this.SetPlateDamaged(false);
      if (num > 1)
        this.SetPlateDisabled(true);
      else
        this.SetPlateDisabled(false);
    }

    public void ResetAllPlates()
    {
      for (int index = 0; index < this.DamagePlates.Length; ++index)
        this.DamagePlates[index].Reset();
      this.IsPlateDamaged = false;
      this.IsPlateDisabled = false;
    }

    public virtual bool SetPlateDamaged(bool b)
    {
      if (this.IsPlateDamaged == b)
        return false;
      this.IsPlateDamaged = b;
      return true;
    }

    public virtual bool SetPlateDisabled(bool b)
    {
      if (this.IsPlateDisabled == b)
        return false;
      this.IsPlateDisabled = b;
      return true;
    }
  }
}
