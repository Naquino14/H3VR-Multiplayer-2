// Decompiled with JetBrains decompiler
// Type: Anvil.AnvilPrefabSpawn
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FistVR;
using System;
using UnityEngine;

namespace Anvil
{
  [ExecuteInEditMode]
  public class AnvilPrefabSpawn : MonoBehaviour
  {
    public AnvilAsset Prefab;
    public wwTargetManager tm;

    private void Awake()
    {
      if (!Application.isPlaying)
        return;
      AnvilCallback<GameObject> gameObjectAsync = this.Prefab.GetGameObjectAsync();
      if (gameObjectAsync.IsCompleted)
        this.SpawnChild(gameObjectAsync.Result);
      else
        gameObjectAsync.AddCallback(new Action<GameObject>(this.SpawnChild));
    }

    private void SpawnChild(GameObject result) => this.InstantiateAndZero(result);

    public GameObject InstantiateAndZero(GameObject result)
    {
      GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(result, this.transform.position, this.transform.rotation);
      gameObject.SetActive(true);
      if ((UnityEngine.Object) this.tm != (UnityEngine.Object) null)
        gameObject.GetComponent<wwTargetShatterable>().SetManager(this.tm);
      return gameObject;
    }
  }
}
