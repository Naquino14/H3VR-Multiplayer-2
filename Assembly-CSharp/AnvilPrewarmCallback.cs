// Decompiled with JetBrains decompiler
// Type: AnvilPrewarmCallback
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using Anvil;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AnvilPrewarmCallback : AnvilCallbackBase
{
  private List<Action> m_completed = new List<Action>();
  private AnvilCallback<AssetBundle> m_bundleCallback;
  private AnvilCallback<GameObject>[] m_gameCallbacks;

  public AnvilPrewarmCallback(string bundleName)
  {
    AnvilPrewarmCallback anvilPrewarmCallback = this;
    this.m_bundleCallback = AnvilManager.GetBundleAsync(bundleName);
    if (this.m_bundleCallback == null || this.m_bundleCallback.Request is AnvilDummyOperation)
      this.IsCompleted = true;
    else
      this.m_bundleCallback.AddCallback((Action<AssetBundle>) (bundle =>
      {
        string[] allAssetNames = bundle.GetAllAssetNames();
        anvilPrewarmCallback.m_gameCallbacks = new AnvilCallback<GameObject>[allAssetNames.Length];
        for (int index = 0; index < allAssetNames.Length; ++index)
          anvilPrewarmCallback.m_gameCallbacks[index] = AnvilManager.LoadAsync(new AssetID()
          {
            AssetName = allAssetNames[index],
            Bundle = bundleName
          });
      }));
  }

  public override bool keepWaiting => !this.IsCompleted;

  public override float Progress
  {
    get
    {
      if (this.IsCompleted)
        return 1f;
      if (this.m_gameCallbacks == null)
        return 0.0f;
      float num = 0.0f;
      for (int index = 0; index < this.m_gameCallbacks.Length; ++index)
        num += this.m_gameCallbacks[index].Progress;
      return num / (float) this.m_gameCallbacks.Length;
    }
  }

  public override void CompleteNow()
  {
    if (this.IsCompleted)
      return;
    this.IsCompleted = true;
    this.m_bundleCallback.CompleteNow();
    for (int index = 0; index < this.m_gameCallbacks.Length; ++index)
      this.m_gameCallbacks[index].CompleteNow();
    foreach (Action action in this.m_completed)
      action();
    this.m_completed.Clear();
  }

  public override bool Pump()
  {
    if (this.IsCompleted)
      return true;
    if (this.m_bundleCallback.Pump())
    {
      for (int index = 0; index < this.m_gameCallbacks.Length; ++index)
      {
        if (!this.m_gameCallbacks[index].Pump())
          return false;
      }
    }
    this.CompleteNow();
    return true;
  }

  public void AddCallback(Action completed)
  {
    if (completed == null)
      return;
    if (this.IsCompleted)
      completed();
    else
      this.m_completed.Add(completed);
  }
}
