// Decompiled with JetBrains decompiler
// Type: AnvilImportPaths
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnvilImportPaths : ScriptableObject
{
  [SerializeField]
  private List<string> m_paths = new List<string>();
  [SerializeField]
  private List<string> m_bundleNames = new List<string>();
  private static AnvilImportPaths s_instance;
  public ShaderVariantCollection ShaderCollection;

  private void SetPathStates()
  {
    this.m_paths.Sort((Comparison<string>) ((a, b) => b.Length.CompareTo(a.Length)));
    this.m_bundleNames = this.m_paths.Select<string, string>((Func<string, string>) (p => p.Replace('/', '_').ToLower())).ToList<string>();
  }

  public void AddPath(string path)
  {
    this.m_paths.Add(path);
    this.SetPathStates();
  }

  public void RemovePath(string path)
  {
    this.m_paths.Remove(path);
    this.SetPathStates();
  }

  public string GetBundle(string path)
  {
    string str = "assets";
    for (int index = 0; index < this.m_paths.Count; ++index)
    {
      string path1 = this.m_paths[index];
      if (path.StartsWith(path1))
        return this.m_bundleNames[index];
    }
    return str;
  }

  public bool Contains(string assetPath) => this.m_paths.Contains(assetPath);
}
