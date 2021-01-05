// Decompiled with JetBrains decompiler
// Type: FistVR.GameObjectWeakLink
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.IO;
using UnityEngine;

namespace FistVR
{
  [Serializable]
  public struct GameObjectWeakLink
  {
    private GameObject m_goCache;
    [SerializeField]
    private string m_assetGUID;
    [SerializeField]
    private string m_filePath;

    public string FilePath => this.m_filePath;

    public static string ToResourcesPath(string assetPath)
    {
      if (assetPath == string.Empty)
        return string.Empty;
      int num = assetPath.IndexOf("/Resources/", StringComparison.Ordinal);
      if (num == -1)
        return string.Empty;
      assetPath = assetPath.Substring(num + "/Resources/".Length);
      assetPath = Path.GetDirectoryName(assetPath) + "/" + Path.GetFileNameWithoutExtension(assetPath);
      return assetPath;
    }

    public void OnBuild()
    {
    }

    public void Preload()
    {
      if (!((UnityEngine.Object) this.m_goCache == (UnityEngine.Object) null))
        return;
      this.m_goCache = Resources.Load<GameObject>(this.m_filePath);
    }

    public GameObject GameObject
    {
      get
      {
        if (string.IsNullOrEmpty(this.m_assetGUID))
          return (GameObject) null;
        if ((UnityEngine.Object) this.m_goCache == (UnityEngine.Object) null)
        {
          this.m_goCache = Resources.Load<GameObject>(this.m_filePath);
          if ((UnityEngine.Object) this.m_goCache == (UnityEngine.Object) null)
            Debug.LogError((object) "Weak linked game object not found!");
        }
        return this.m_goCache;
      }
    }
  }
}
