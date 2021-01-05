// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessEffectSettings
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering.PostProcessing
{
  [Serializable]
  public class PostProcessEffectSettings : ScriptableObject
  {
    public bool active;
    public BoolParameter enabled;
    internal ReadOnlyCollection<ParameterOverride> parameters;

    public PostProcessEffectSettings()
    {
      BoolParameter boolParameter = new BoolParameter();
      boolParameter.overrideState = true;
      boolParameter.value = false;
      this.enabled = boolParameter;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    private void OnEnable() => this.parameters = ((IEnumerable<FieldInfo>) this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)).Where<FieldInfo>((Func<FieldInfo, bool>) (t => t.FieldType.IsSubclassOf(typeof (ParameterOverride)))).OrderBy<FieldInfo, int>((Func<FieldInfo, int>) (t => t.MetadataToken)).Select<FieldInfo, ParameterOverride>((Func<FieldInfo, ParameterOverride>) (t => (ParameterOverride) t.GetValue((object) this))).ToList<ParameterOverride>().AsReadOnly();

    public void SetAllOverridesTo(bool state, bool excludeEnabled = true)
    {
      foreach (ParameterOverride parameter in this.parameters)
      {
        if (!excludeEnabled || parameter != this.enabled)
          parameter.overrideState = state;
      }
    }

    public virtual bool IsEnabledAndSupported(PostProcessRenderContext context) => this.enabled.value;

    public int GetHash()
    {
      int num = 17;
      foreach (ParameterOverride parameter in this.parameters)
        num = num * 23 + parameter.GetHash();
      return num;
    }
  }
}
