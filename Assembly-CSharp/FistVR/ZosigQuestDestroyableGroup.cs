// Decompiled with JetBrains decompiler
// Type: FistVR.ZosigQuestDestroyableGroup
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

namespace FistVR
{
  public class ZosigQuestDestroyableGroup : ZosigQuestManager
  {
    public List<ZosigDestroyable> Destroyables;
    private ZosigGameManager m;

    public override void Init(ZosigGameManager M)
    {
      this.m = M;
      for (int index = 0; index < this.Destroyables.Count; ++index)
        this.Destroyables[index].Init(M);
    }
  }
}
