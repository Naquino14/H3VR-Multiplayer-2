// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.PostProcessEventComparer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace UnityEngine.Rendering.PostProcessing
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct PostProcessEventComparer : IEqualityComparer<PostProcessEvent>
  {
    public bool Equals(PostProcessEvent x, PostProcessEvent y) => x == y;

    public int GetHashCode(PostProcessEvent obj) => (int) obj;
  }
}
