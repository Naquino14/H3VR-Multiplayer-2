// Decompiled with JetBrains decompiler
// Type: AnvilCallback`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class AnvilCallback<TCall> : AnvilCallbackBase where TCall : UnityEngine.Object
{
  private AnvilCallback<AssetBundle> m_dependancy;
  private List<Action<TCall>> m_completed = new List<Action<TCall>>();
  private TCall m_result;

  public AnvilCallback(AsyncOperation request, AnvilCallback<AssetBundle> bundle)
  {
    this.Request = request;
    this.m_dependancy = bundle;
  }

  public AsyncOperation Request { get; set; }

  public override float Progress
  {
    get
    {
      if (this.IsCompleted)
        return 1f;
      return this.Request == null ? 0.0f : this.Request.progress;
    }
  }

  public TCall Result
  {
    get
    {
      this.CompleteNow();
      return this.m_result;
    }
  }

  public override bool keepWaiting => !this.Pump();

  public override bool Pump()
  {
    if (this.IsCompleted)
      return true;
    if (this.m_dependancy != null && !this.m_dependancy.IsCompleted || (this.Request == null || !(this.Request is AnvilDummyOperation) && !this.Request.isDone))
      return false;
    this.CompleteNow();
    return true;
  }

  public void AddCallback(Action<TCall> completed)
  {
    if (completed == null)
      return;
    if (!this.IsCompleted)
      this.m_completed.Add(completed);
    else
      completed(this.m_result);
  }

  public override void CompleteNow()
  {
    if (this.IsCompleted)
      return;
    this.IsCompleted = true;
    if (this.m_dependancy != null)
      this.m_dependancy.CompleteNow();
    if (this.Request is AssetBundleRequest)
      this.m_result = ((AssetBundleRequest) this.Request).asset as TCall;
    else if (this.Request is AssetBundleCreateRequest)
      this.m_result = (object) ((AssetBundleCreateRequest) this.Request).assetBundle as TCall;
    else if (this.Request is AnvilDummyOperation)
      this.m_result = ((AnvilDummyOperation) this.Request).Result as TCall;
    else
      Debug.LogError((object) ("Anvil: Can't find load type! " + (object) this.Request.GetType()));
    foreach (Action<TCall> action in this.m_completed)
      action(this.m_result);
    this.m_completed.Clear();
  }
}
