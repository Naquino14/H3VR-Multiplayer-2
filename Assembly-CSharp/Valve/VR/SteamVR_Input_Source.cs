// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Input_Source
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace Valve.VR
{
  public static class SteamVR_Input_Source
  {
    private static Dictionary<SteamVR_Input_Sources, ulong> inputSourceHandlesBySource = new Dictionary<SteamVR_Input_Sources, ulong>((IEqualityComparer<SteamVR_Input_Sources>) new SteamVR_Input_Sources_Comparer());
    private static Dictionary<ulong, SteamVR_Input_Sources> inputSourceSourcesByHandle = new Dictionary<ulong, SteamVR_Input_Sources>();
    private static System.Type enumType = typeof (SteamVR_Input_Sources);
    private static System.Type descriptionType = typeof (DescriptionAttribute);
    private static SteamVR_Input_Sources[] allSources;

    public static ulong GetHandle(SteamVR_Input_Sources inputSource) => SteamVR_Input_Source.inputSourceHandlesBySource.ContainsKey(inputSource) ? SteamVR_Input_Source.inputSourceHandlesBySource[inputSource] : 0UL;

    public static SteamVR_Input_Sources GetSource(ulong handle) => SteamVR_Input_Source.inputSourceSourcesByHandle.ContainsKey(handle) ? SteamVR_Input_Source.inputSourceSourcesByHandle[handle] : SteamVR_Input_Sources.Any;

    public static SteamVR_Input_Sources[] GetAllSources()
    {
      if (SteamVR_Input_Source.allSources == null)
        SteamVR_Input_Source.allSources = (SteamVR_Input_Sources[]) Enum.GetValues(typeof (SteamVR_Input_Sources));
      return SteamVR_Input_Source.allSources;
    }

    private static string GetPath(string inputSourceEnumName) => ((DescriptionAttribute) SteamVR_Input_Source.enumType.GetMember(inputSourceEnumName)[0].GetCustomAttributes(SteamVR_Input_Source.descriptionType, false)[0]).Description;

    public static void Initialize()
    {
      List<SteamVR_Input_Sources> steamVrInputSourcesList = new List<SteamVR_Input_Sources>();
      string[] names = Enum.GetNames(SteamVR_Input_Source.enumType);
      SteamVR_Input_Source.inputSourceHandlesBySource = new Dictionary<SteamVR_Input_Sources, ulong>((IEqualityComparer<SteamVR_Input_Sources>) new SteamVR_Input_Sources_Comparer());
      SteamVR_Input_Source.inputSourceSourcesByHandle = new Dictionary<ulong, SteamVR_Input_Sources>();
      for (int index = 0; index < names.Length; ++index)
      {
        string path = SteamVR_Input_Source.GetPath(names[index]);
        ulong pHandle = 0;
        EVRInputError inputSourceHandle = OpenVR.Input.GetInputSourceHandle(path, ref pHandle);
        if (inputSourceHandle != EVRInputError.None)
          Debug.LogError((object) ("<b>[SteamVR]</b> GetInputSourceHandle (" + path + ") error: " + inputSourceHandle.ToString()));
        if (names[index] == SteamVR_Input_Sources.Any.ToString())
        {
          SteamVR_Input_Source.inputSourceHandlesBySource.Add((SteamVR_Input_Sources) index, 0UL);
          SteamVR_Input_Source.inputSourceSourcesByHandle.Add(0UL, (SteamVR_Input_Sources) index);
        }
        else
        {
          SteamVR_Input_Source.inputSourceHandlesBySource.Add((SteamVR_Input_Sources) index, pHandle);
          SteamVR_Input_Source.inputSourceSourcesByHandle.Add(pHandle, (SteamVR_Input_Sources) index);
        }
        steamVrInputSourcesList.Add((SteamVR_Input_Sources) index);
      }
      SteamVR_Input_Source.allSources = steamVrInputSourcesList.ToArray();
    }
  }
}
