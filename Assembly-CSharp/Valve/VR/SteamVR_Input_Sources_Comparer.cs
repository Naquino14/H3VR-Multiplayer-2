// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_Sources_Comparer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Valve.VR
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct SteamVR_Input_Sources_Comparer : IEqualityComparer<SteamVR_Input_Sources>
  {
    public bool Equals(SteamVR_Input_Sources x, SteamVR_Input_Sources y) => x == y;

    public int GetHashCode(SteamVR_Input_Sources obj) => (int) obj;
  }
}
