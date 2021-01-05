// Decompiled with JetBrains decompiler
// Type: AnvilManager
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using Anvil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class AnvilManager : MonoBehaviour
{
  private static AnvilManager.LoadingHolder<string> m_bundles = new AnvilManager.LoadingHolder<string>();
  private static AnvilManager.LoadingHolder<string> m_prewarms = new AnvilManager.LoadingHolder<string>();
  private static AnvilManager.LoadingHolder<AssetID> m_assets = new AnvilManager.LoadingHolder<AssetID>();
  public static AnvilManager Instance;
  private const bool UseBundles = true;

  private void Awake() => AnvilManager.Instance = this;

  public static AnvilPrewarmCallback PreloadBundleAsync(string bundle) => AnvilManager.PreloadBundleAsyncInternal(bundle);

  private static AnvilPrewarmCallback PreloadBundleAsyncInternal(string bundle)
  {
    AnvilCallbackBase cb1;
    if (AnvilManager.m_prewarms.TryGetValue(bundle, out cb1))
      return cb1 as AnvilPrewarmCallback;
    AnvilCallbackBase cb2 = (AnvilCallbackBase) new AnvilPrewarmCallback(bundle);
    AnvilManager.m_prewarms.Add(bundle, cb2);
    return (AnvilPrewarmCallback) cb2;
  }

  public static AnvilCallback<AssetBundle> GetBundleAsync(string bundle) => AnvilManager.GetAssetBundleAsyncInternal(bundle);

  private static AnvilCallback<AssetBundle> GetAssetBundleAsyncInternal(
    string bundle)
  {
    AnvilCallbackBase cb1;
    if (AnvilManager.m_bundles.TryGetValue(bundle, out cb1))
      return cb1 as AnvilCallback<AssetBundle>;
    string path = Path.Combine(Application.streamingAssetsPath, bundle);
    AsyncOperation request;
    if (!File.Exists(path))
    {
      UnityEngine.Debug.LogError((object) ("Anvil: Couldn't find bundle " + path));
      request = (AsyncOperation) new AnvilDummyOperation((UnityEngine.Object) null);
    }
    else
      request = (AsyncOperation) AssetBundle.LoadFromFileAsync(path);
    AnvilCallbackBase cb2 = (AnvilCallbackBase) new AnvilCallback<AssetBundle>(request, (AnvilCallback<AssetBundle>) null);
    AnvilManager.m_bundles.Add(bundle, cb2);
    return (AnvilCallback<AssetBundle>) cb2;
  }

  public static AnvilCallback<GameObject> LoadAsync(AssetID assetID)
  {
    AnvilCallbackBase cb1;
    if (AnvilManager.m_assets.TryGetValue(assetID, out cb1))
      return cb1 as AnvilCallback<GameObject>;
    AnvilCallback<AssetBundle> bundleAsyncInternal = AnvilManager.GetAssetBundleAsyncInternal(assetID.Bundle);
    AnvilCallbackBase cb2;
    if (bundleAsyncInternal.IsCompleted)
    {
      cb2 = (AnvilCallbackBase) new AnvilCallback<GameObject>((AsyncOperation) null, (AnvilCallback<AssetBundle>) null);
      ((AnvilCallback<GameObject>) cb2).Request = AnvilManager.GetCallbackRequest(assetID, bundleAsyncInternal.Result);
    }
    else
    {
      cb2 = (AnvilCallbackBase) new AnvilCallback<GameObject>((AsyncOperation) null, bundleAsyncInternal);
      AnvilCallback<GameObject> tempCB = (AnvilCallback<GameObject>) cb2;
      bundleAsyncInternal.AddCallback((Action<AssetBundle>) (bundle => tempCB.Request = AnvilManager.GetCallbackRequest(assetID, bundle)));
    }
    AnvilManager.m_assets.Add(assetID, cb2);
    return (AnvilCallback<GameObject>) cb2;
  }

  private static AsyncOperation GetCallbackRequest(
    AssetID assetID,
    AssetBundle bundle)
  {
    return (UnityEngine.Object) bundle == (UnityEngine.Object) null ? (AsyncOperation) new AnvilDummyOperation((UnityEngine.Object) null) : (AsyncOperation) bundle.LoadAssetAsync(assetID.AssetName, typeof (UnityEngine.Object));
  }

  private void Update()
  {
    if (!Application.isPlaying)
      return;
    AnvilManager.AnvilUpdate();
  }

  public static void AnvilUpdate()
  {
    AnvilManager.m_bundles.PumpLoading();
    AnvilManager.m_assets.PumpLoading();
    AnvilManager.m_prewarms.PumpLoading();
  }

  [DebuggerHidden]
  private IEnumerator RunDriven(IEnumerator routine) => (IEnumerator) new AnvilManager.\u003CRunDriven\u003Ec__Iterator0()
  {
    routine = routine
  };

  public static void Run(IEnumerator routine)
  {
    if (!Application.isPlaying)
      UnityEngine.Debug.LogError((object) "Can't run Anvil routines out of playmode!");
    else
      AnvilManager.Instance.StartCoroutine(AnvilManager.Instance.RunDriven(routine));
  }

  private class LoadingHolder<TKey>
  {
    private Dictionary<TKey, AnvilCallbackBase> m_lookup = new Dictionary<TKey, AnvilCallbackBase>();
    private List<AnvilCallbackBase> m_loading = new List<AnvilCallbackBase>();

    public void Add(TKey key, AnvilCallbackBase cb)
    {
      this.m_lookup.Add(key, cb);
      this.m_loading.Add(cb);
    }

    public void PumpLoading()
    {
      for (int index = this.m_loading.Count - 1; index >= 0; --index)
      {
        if (this.m_loading[index].Pump())
          this.m_loading.RemoveAt(index);
      }
    }

    public bool TryGetValue(TKey key, out AnvilCallbackBase cb) => this.m_lookup.TryGetValue(key, out cb);
  }
}
