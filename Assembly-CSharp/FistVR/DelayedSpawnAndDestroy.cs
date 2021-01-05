// Decompiled with JetBrains decompiler
// Type: FistVR.DelayedSpawnAndDestroy
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class DelayedSpawnAndDestroy : MonoBehaviour
  {
    private float m_timeTilDestroy;
    private bool m_isDestroyed;
    public List<DelayedSpawnAndDestroy.DestroyAbleGroup> Groups;

    private void Start() => this.m_timeTilDestroy = 0.0f;

    private void Update()
    {
      this.m_timeTilDestroy += Time.deltaTime;
      for (int index1 = 0; index1 < this.Groups.Count; ++index1)
      {
        if (!this.Groups[index1].HasBeenDestroyed && (double) this.m_timeTilDestroy >= (double) this.Groups[index1].DestroyAtThisPoint)
        {
          this.Groups[index1].HasBeenDestroyed = true;
          for (int index2 = 0; index2 < this.Groups[index1].SpawnThese.Count; ++index2)
            UnityEngine.Object.Instantiate<GameObject>(this.Groups[index1].SpawnThese[index2], this.transform.position, this.transform.rotation);
        }
        if (this.Groups[index1].HasBeenDestroyed && index1 >= this.Groups.Count - 1)
          UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
      }
    }

    [Serializable]
    public class DestroyAbleGroup
    {
      public bool HasBeenDestroyed;
      public float DestroyAtThisPoint = 1f;
      public List<GameObject> SpawnThese;
    }
  }
}
