// Decompiled with JetBrains decompiler
// Type: AnvilAsset
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using Anvil;
using UnityEngine;

public abstract class AnvilAsset : ScriptableObject
{
  [SerializeField]
  private AssetID m_anvilPrefab;
  private AnvilCallback<GameObject> m_loadingState;

  public string Guid => this.m_anvilPrefab.Guid;

  public GameObject GetGameObject()
  {
    if (this.m_loadingState == null)
      this.m_loadingState = AnvilManager.LoadAsync(this.m_anvilPrefab);
    return this.m_loadingState.Result;
  }

  public void RefreshCache() => this.m_loadingState = AnvilManager.LoadAsync(this.m_anvilPrefab);

  public AnvilCallback<GameObject> GetGameObjectAsync()
  {
    if (this.m_loadingState == null)
      this.m_loadingState = AnvilManager.LoadAsync(this.m_anvilPrefab);
    return this.m_loadingState;
  }

  public void UpgradeFrom(GameObject go)
  {
  }
}
