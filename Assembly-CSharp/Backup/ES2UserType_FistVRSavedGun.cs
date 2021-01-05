// Decompiled with JetBrains decompiler
// Type: ES2UserType_FistVRSavedGun
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FistVR;
using System;

public class ES2UserType_FistVRSavedGun : ES2Type
{
  public ES2UserType_FistVRSavedGun()
    : base(typeof (SavedGun))
  {
  }

  public override void Write(object obj, ES2Writer writer)
  {
    SavedGun savedGun = (SavedGun) obj;
    writer.Write<string>(savedGun.FileName);
    writer.Write<SavedGunComponent>(savedGun.Components);
    writer.Write<FireArmRoundClass>(savedGun.LoadedRoundsInMag);
    writer.Write<FireArmRoundClass>(savedGun.LoadedRoundsInChambers);
    writer.Write<string>(savedGun.SavedFlags);
    writer.Write<DateTime>(savedGun.DateMade);
  }

  public override object Read(ES2Reader reader)
  {
    SavedGun savedGun = new SavedGun();
    this.Read(reader, (object) savedGun);
    return (object) savedGun;
  }

  public override void Read(ES2Reader reader, object c)
  {
    SavedGun savedGun = (SavedGun) c;
    savedGun.FileName = reader.Read<string>();
    savedGun.Components = reader.ReadList<SavedGunComponent>();
    savedGun.LoadedRoundsInMag = reader.ReadList<FireArmRoundClass>();
    savedGun.LoadedRoundsInChambers = reader.ReadList<FireArmRoundClass>();
    savedGun.SavedFlags = reader.ReadList<string>();
    savedGun.DateMade = reader.Read<DateTime>();
  }
}
