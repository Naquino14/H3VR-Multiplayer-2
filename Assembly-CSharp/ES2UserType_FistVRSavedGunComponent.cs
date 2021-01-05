// Decompiled with JetBrains decompiler
// Type: ES2UserType_FistVRSavedGunComponent
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FistVR;
using UnityEngine;

public class ES2UserType_FistVRSavedGunComponent : ES2Type
{
  public ES2UserType_FistVRSavedGunComponent()
    : base(typeof (SavedGunComponent))
  {
  }

  public override void Write(object obj, ES2Writer writer)
  {
    SavedGunComponent savedGunComponent = (SavedGunComponent) obj;
    writer.Write<int>(savedGunComponent.Index);
    writer.Write<string>(savedGunComponent.ObjectID);
    writer.Write<Vector3>(savedGunComponent.PosOffset);
    writer.Write<Vector3>(savedGunComponent.OrientationForward);
    writer.Write<Vector3>(savedGunComponent.OrientationUp);
    writer.Write<int>(savedGunComponent.ObjectAttachedTo);
    writer.Write<int>(savedGunComponent.MountAttachedTo);
    writer.Write<bool>(savedGunComponent.isFirearm);
    writer.Write<bool>(savedGunComponent.isMagazine);
    writer.Write<bool>(savedGunComponent.isAttachment);
    writer.Write<string, string>(savedGunComponent.Flags);
  }

  public override object Read(ES2Reader reader)
  {
    SavedGunComponent savedGunComponent = new SavedGunComponent();
    this.Read(reader, (object) savedGunComponent);
    return (object) savedGunComponent;
  }

  public override void Read(ES2Reader reader, object c)
  {
    SavedGunComponent savedGunComponent = (SavedGunComponent) c;
    savedGunComponent.Index = reader.Read<int>();
    savedGunComponent.ObjectID = reader.Read<string>();
    savedGunComponent.PosOffset = reader.Read<Vector3>();
    savedGunComponent.OrientationForward = reader.Read<Vector3>();
    savedGunComponent.OrientationUp = reader.Read<Vector3>();
    savedGunComponent.ObjectAttachedTo = reader.Read<int>();
    savedGunComponent.MountAttachedTo = reader.Read<int>();
    savedGunComponent.isFirearm = reader.Read<bool>();
    savedGunComponent.isMagazine = reader.Read<bool>();
    savedGunComponent.isAttachment = reader.Read<bool>();
    savedGunComponent.Flags = reader.ReadDictionary<string, string>();
  }
}
