// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVREnumEqualityComparer`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Valve.VR
{
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  internal struct SteamVREnumEqualityComparer<TEnum> : IEqualityComparer<TEnum> where TEnum : struct
  {
    public bool Equals(TEnum firstEnum, TEnum secondEnum) => SteamVREnumEqualityComparer<TEnum>.BoxAvoidance.ToInt(firstEnum) == SteamVREnumEqualityComparer<TEnum>.BoxAvoidance.ToInt(secondEnum);

    public int GetHashCode(TEnum firstEnum) => SteamVREnumEqualityComparer<TEnum>.BoxAvoidance.ToInt(firstEnum);

    private static class BoxAvoidance
    {
      private static readonly Func<TEnum, int> _wrapper = ((Expression<Func<TEnum, int>>) (@enum => {checked {(int) @enum;}})).Compile();

      public static int ToInt(TEnum enu) => SteamVREnumEqualityComparer<TEnum>.BoxAvoidance._wrapper(enu);
    }
  }
}
