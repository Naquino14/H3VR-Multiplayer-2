// Decompiled with JetBrains decompiler
// Type: AmplifyColor.VolumeEffectComponentFlags
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
  [Serializable]
  public class VolumeEffectComponentFlags
  {
    public string componentName;
    public List<VolumeEffectFieldFlags> componentFields;
    public bool blendFlag;

    public VolumeEffectComponentFlags(string name)
    {
      this.componentName = name;
      this.componentFields = new List<VolumeEffectFieldFlags>();
    }

    public VolumeEffectComponentFlags(VolumeEffectComponent comp)
      : this(comp.componentName)
    {
      this.blendFlag = true;
      foreach (VolumeEffectField field in comp.fields)
      {
        if (VolumeEffectField.IsValidType(field.fieldType))
          this.componentFields.Add(new VolumeEffectFieldFlags(field));
      }
    }

    public VolumeEffectComponentFlags(Component c)
      : this(c.GetType().ToString() + string.Empty)
    {
      foreach (FieldInfo field in c.GetType().GetFields())
      {
        if (VolumeEffectField.IsValidType(field.FieldType.FullName))
          this.componentFields.Add(new VolumeEffectFieldFlags(field));
      }
    }

    public void UpdateComponentFlags(VolumeEffectComponent comp)
    {
      foreach (VolumeEffectField field1 in comp.fields)
      {
        VolumeEffectField field = field1;
        if (this.componentFields.Find((Predicate<VolumeEffectFieldFlags>) (s => s.fieldName == field.fieldName)) == null && VolumeEffectField.IsValidType(field.fieldType))
          this.componentFields.Add(new VolumeEffectFieldFlags(field));
      }
    }

    public void UpdateComponentFlags(Component c)
    {
      foreach (FieldInfo field in c.GetType().GetFields())
      {
        FieldInfo pi = field;
        if (!this.componentFields.Exists((Predicate<VolumeEffectFieldFlags>) (s => s.fieldName == pi.Name)) && VolumeEffectField.IsValidType(pi.FieldType.FullName))
          this.componentFields.Add(new VolumeEffectFieldFlags(pi));
      }
    }

    public string[] GetFieldNames() => this.componentFields.Where<VolumeEffectFieldFlags>((Func<VolumeEffectFieldFlags, bool>) (r => r.blendFlag)).Select<VolumeEffectFieldFlags, string>((Func<VolumeEffectFieldFlags, string>) (r => r.fieldName)).ToArray<string>();
  }
}
