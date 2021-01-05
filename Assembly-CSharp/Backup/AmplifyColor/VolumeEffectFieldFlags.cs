// Decompiled with JetBrains decompiler
// Type: AmplifyColor.VolumeEffectFieldFlags
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Reflection;

namespace AmplifyColor
{
  [Serializable]
  public class VolumeEffectFieldFlags
  {
    public string fieldName;
    public string fieldType;
    public bool blendFlag;

    public VolumeEffectFieldFlags(FieldInfo pi)
    {
      this.fieldName = pi.Name;
      this.fieldType = pi.FieldType.FullName;
    }

    public VolumeEffectFieldFlags(VolumeEffectField field)
    {
      this.fieldName = field.fieldName;
      this.fieldType = field.fieldType;
      this.blendFlag = true;
    }
  }
}
