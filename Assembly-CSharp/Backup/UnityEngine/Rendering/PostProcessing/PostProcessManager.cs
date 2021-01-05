// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering.PostProcessing
{
  public sealed class PostProcessManager
  {
    private static PostProcessManager s_Instance;
    private const int k_MaxLayerCount = 32;
    private readonly List<PostProcessVolume>[] m_Volumes;
    private readonly bool[] m_SortNeeded;
    private readonly List<PostProcessEffectSettings> m_BaseSettings;
    private readonly List<Collider> m_TempColliders;
    public readonly Dictionary<System.Type, PostProcessAttribute> settingsTypes;

    private PostProcessManager()
    {
      this.m_Volumes = new List<PostProcessVolume>[32];
      this.m_SortNeeded = new bool[32];
      this.m_BaseSettings = new List<PostProcessEffectSettings>();
      this.m_TempColliders = new List<Collider>(5);
      this.settingsTypes = new Dictionary<System.Type, PostProcessAttribute>();
      this.ReloadBaseTypes();
    }

    public static PostProcessManager instance
    {
      get
      {
        if (PostProcessManager.s_Instance == null)
          PostProcessManager.s_Instance = new PostProcessManager();
        return PostProcessManager.s_Instance;
      }
    }

    private void CleanBaseTypes()
    {
      this.settingsTypes.Clear();
      foreach (UnityEngine.Object baseSetting in this.m_BaseSettings)
        RuntimeUtilities.Destroy(baseSetting);
      this.m_BaseSettings.Clear();
    }

    private void ReloadBaseTypes()
    {
      this.CleanBaseTypes();
      foreach (System.Type type in ((IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies()).SelectMany<Assembly, System.Type>((Func<Assembly, IEnumerable<System.Type>>) (a => ((IEnumerable<System.Type>) a.GetTypes()).Where<System.Type>((Func<System.Type, bool>) (t => t.IsSubclassOf(typeof (PostProcessEffectSettings)) && t.IsDefined(typeof (PostProcessAttribute), false))))))
      {
        this.settingsTypes.Add(type, type.GetAttribute<PostProcessAttribute>());
        PostProcessEffectSettings instance = (PostProcessEffectSettings) ScriptableObject.CreateInstance(type);
        instance.SetAllOverridesTo(true, false);
        this.m_BaseSettings.Add(instance);
      }
    }

    public void GetActiveVolumes(PostProcessLayer layer, List<PostProcessVolume> results)
    {
      int num1 = layer.volumeLayer.value;
      Transform volumeTrigger = layer.volumeTrigger;
      bool flag = (UnityEngine.Object) volumeTrigger == (UnityEngine.Object) null;
      Vector3 position = !flag ? volumeTrigger.position : Vector3.zero;
      for (int index = 0; index < 32; ++index)
      {
        if ((num1 & 1 << index) != 0)
        {
          List<PostProcessVolume> volume = this.m_Volumes[index];
          if (volume != null)
          {
            foreach (PostProcessVolume postProcessVolume in volume)
            {
              if (postProcessVolume.enabled && !((UnityEngine.Object) postProcessVolume.profileRef == (UnityEngine.Object) null) && (double) postProcessVolume.weight > 0.0)
              {
                if (postProcessVolume.isGlobal)
                  results.Add(postProcessVolume);
                else if (!flag)
                {
                  List<Collider> tempColliders = this.m_TempColliders;
                  postProcessVolume.GetComponents<Collider>(tempColliders);
                  if (tempColliders.Count != 0)
                  {
                    float num2 = float.PositiveInfinity;
                    foreach (Collider collider in tempColliders)
                    {
                      if (collider.enabled)
                      {
                        float sqrMagnitude = ((collider.ClosestPoint(position) - position) / 2f).sqrMagnitude;
                        if ((double) sqrMagnitude < (double) num2)
                          num2 = sqrMagnitude;
                      }
                    }
                    tempColliders.Clear();
                    float num3 = postProcessVolume.blendDistance * postProcessVolume.blendDistance;
                    if ((double) num2 <= (double) num3)
                      results.Add(postProcessVolume);
                  }
                }
              }
            }
          }
        }
      }
    }

    public PostProcessVolume GetHighestPriorityVolume(PostProcessLayer layer) => !((UnityEngine.Object) layer == (UnityEngine.Object) null) ? this.GetHighestPriorityVolume(layer.volumeLayer) : throw new ArgumentNullException(nameof (layer));

    public PostProcessVolume GetHighestPriorityVolume(LayerMask mask)
    {
      float num = float.NegativeInfinity;
      PostProcessVolume postProcessVolume1 = (PostProcessVolume) null;
      for (int index = 0; index < 32; ++index)
      {
        if (((int) mask & 1 << index) != 0)
        {
          List<PostProcessVolume> volume = this.m_Volumes[index];
          if (volume != null)
          {
            foreach (PostProcessVolume postProcessVolume2 in volume)
            {
              if ((double) postProcessVolume2.priority > (double) num)
              {
                num = postProcessVolume2.priority;
                postProcessVolume1 = postProcessVolume2;
              }
            }
          }
        }
      }
      return postProcessVolume1;
    }

    public PostProcessVolume QuickVolume(
      int layer,
      float priority,
      params PostProcessEffectSettings[] settings)
    {
      GameObject gameObject = new GameObject();
      gameObject.name = "Quick Volume";
      gameObject.layer = layer;
      gameObject.hideFlags = HideFlags.HideAndDontSave;
      PostProcessVolume postProcessVolume = gameObject.AddComponent<PostProcessVolume>();
      postProcessVolume.priority = priority;
      postProcessVolume.isGlobal = true;
      PostProcessProfile profile = postProcessVolume.profile;
      foreach (PostProcessEffectSettings setting in settings)
        profile.AddSettings(setting);
      return postProcessVolume;
    }

    internal void SetLayerDirty(int layer) => this.m_SortNeeded[layer] = true;

    internal void UpdateVolumeLayer(PostProcessVolume volume, int prevLayer, int newLayer)
    {
      this.Unregister(volume, prevLayer);
      this.Register(volume, newLayer);
    }

    private void Register(PostProcessVolume volume, int layer)
    {
      List<PostProcessVolume> postProcessVolumeList = this.m_Volumes[layer];
      if (postProcessVolumeList == null)
      {
        postProcessVolumeList = new List<PostProcessVolume>();
        this.m_Volumes[layer] = postProcessVolumeList;
      }
      postProcessVolumeList.Add(volume);
      this.SetLayerDirty(layer);
    }

    internal void Register(PostProcessVolume volume)
    {
      int layer = volume.gameObject.layer;
      this.Register(volume, layer);
    }

    private void Unregister(PostProcessVolume volume, int layer) => this.m_Volumes[layer]?.Remove(volume);

    internal void Unregister(PostProcessVolume volume)
    {
      int layer = volume.gameObject.layer;
      this.Unregister(volume, layer);
    }

    internal void UpdateSettings(PostProcessLayer postProcessLayer)
    {
      postProcessLayer.OverrideSettings(this.m_BaseSettings, 1f);
      int num1 = postProcessLayer.volumeLayer.value;
      Transform volumeTrigger = postProcessLayer.volumeTrigger;
      bool flag = (UnityEngine.Object) volumeTrigger == (UnityEngine.Object) null;
      Vector3 position = !flag ? volumeTrigger.position : Vector3.zero;
      for (int index = 0; index < 32; ++index)
      {
        if ((num1 & 1 << index) != 0)
        {
          List<PostProcessVolume> volume = this.m_Volumes[index];
          if (volume != null)
          {
            if (this.m_SortNeeded[index])
            {
              PostProcessManager.SortByPriority(volume);
              this.m_SortNeeded[index] = false;
            }
            foreach (PostProcessVolume postProcessVolume in volume)
            {
              if (postProcessVolume.enabled && !((UnityEngine.Object) postProcessVolume.profileRef == (UnityEngine.Object) null) && (double) postProcessVolume.weight > 0.0)
              {
                List<PostProcessEffectSettings> settings = postProcessVolume.profileRef.settings;
                if (postProcessVolume.isGlobal)
                  postProcessLayer.OverrideSettings(settings, postProcessVolume.weight);
                else if (!flag)
                {
                  List<Collider> tempColliders = this.m_TempColliders;
                  postProcessVolume.GetComponents<Collider>(tempColliders);
                  if (tempColliders.Count != 0)
                  {
                    float num2 = float.PositiveInfinity;
                    foreach (Collider collider in tempColliders)
                    {
                      if (collider.enabled)
                      {
                        float sqrMagnitude = ((collider.ClosestPoint(position) - position) / 2f).sqrMagnitude;
                        if ((double) sqrMagnitude < (double) num2)
                          num2 = sqrMagnitude;
                      }
                    }
                    tempColliders.Clear();
                    float num3 = postProcessVolume.blendDistance * postProcessVolume.blendDistance;
                    if ((double) num2 <= (double) num3)
                    {
                      float num4 = 1f;
                      if ((double) num3 > 0.0)
                        num4 = (float) (1.0 - (double) num2 / (double) num3);
                      postProcessLayer.OverrideSettings(settings, num4 * postProcessVolume.weight);
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    private static void SortByPriority(List<PostProcessVolume> volumes)
    {
      for (int index1 = 1; index1 < volumes.Count; ++index1)
      {
        PostProcessVolume volume = volumes[index1];
        int index2;
        for (index2 = index1 - 1; index2 >= 0 && (double) volumes[index2].priority > (double) volume.priority; --index2)
          volumes[index2 + 1] = volumes[index2];
        volumes[index2 + 1] = volume;
      }
    }
  }
}
