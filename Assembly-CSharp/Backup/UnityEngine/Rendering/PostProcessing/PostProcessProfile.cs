// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessProfile
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class PostProcessProfile : ScriptableObject
  {
    [Tooltip("A list of all settings & overrides.")]
    public List<PostProcessEffectSettings> settings = new List<PostProcessEffectSettings>();
    [NonSerialized]
    public bool isDirty = true;

    private void OnEnable() => this.settings.RemoveAll((Predicate<PostProcessEffectSettings>) (x => (UnityEngine.Object) x == (UnityEngine.Object) null));

    public void Reset() => this.isDirty = true;

    public T AddSettings<T>() where T : PostProcessEffectSettings => (T) this.AddSettings(typeof (T));

    public PostProcessEffectSettings AddSettings(System.Type type)
    {
      PostProcessEffectSettings processEffectSettings = !this.HasSettings(type) ? (PostProcessEffectSettings) ScriptableObject.CreateInstance(type) : throw new InvalidOperationException("Effect already exists in the stack");
      processEffectSettings.hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
      processEffectSettings.name = type.Name;
      processEffectSettings.enabled.value = true;
      this.settings.Add(processEffectSettings);
      this.isDirty = true;
      return processEffectSettings;
    }

    public PostProcessEffectSettings AddSettings(
      PostProcessEffectSettings effect)
    {
      if (this.HasSettings(this.settings.GetType()))
        throw new InvalidOperationException("Effect already exists in the stack");
      this.settings.Add(effect);
      this.isDirty = true;
      return effect;
    }

    public void RemoveSettings<T>() where T : PostProcessEffectSettings => this.RemoveSettings(typeof (T));

    public void RemoveSettings(System.Type type)
    {
      int index1 = -1;
      for (int index2 = 0; index2 < this.settings.Count; ++index2)
      {
        if (this.settings[index2].GetType() == type)
        {
          index1 = index2;
          break;
        }
      }
      if (index1 < 0)
        throw new InvalidOperationException("Effect doesn't exist in the stack");
      this.settings.RemoveAt(index1);
      this.isDirty = true;
    }

    public bool HasSettings<T>() where T : PostProcessEffectSettings => this.HasSettings(typeof (T));

    public bool HasSettings(System.Type type)
    {
      foreach (object setting in this.settings)
      {
        if (setting.GetType() == type)
          return true;
      }
      return false;
    }

    public bool TryGetSettings<T>(out T outSetting) where T : PostProcessEffectSettings
    {
      System.Type type = typeof (T);
      outSetting = (T) null;
      foreach (PostProcessEffectSettings setting in this.settings)
      {
        if (setting.GetType() == type)
        {
          outSetting = (T) setting;
          return true;
        }
      }
      return false;
    }
  }
}
