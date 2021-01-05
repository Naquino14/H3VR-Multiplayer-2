// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigContainer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class ZosigContainer : MonoBehaviour
  {
    protected bool m_isOpen;
    protected bool m_containsItems;
    protected FVRObject m_storedObject1;
    protected List<GameObject> m_spawnedObjects = new List<GameObject>();
    public bool SetFlagOnOpenDestroy;
    public string FlagToSet;
    public int ValueToSet;

    public void FlagOpen()
    {
      if (!this.SetFlagOnOpenDestroy || !((Object) GM.ZMaster != (Object) null))
        return;
      GM.ZMaster.FlagM.SetFlagMaxBlend(this.FlagToSet, this.ValueToSet);
    }

    public virtual void PlaceObjectsInContainer(FVRObject obj1, int minAmmo = -1, int maxAmmo = 30) => this.m_containsItems = true;

    public void TestForReset()
    {
    }
  }
}
