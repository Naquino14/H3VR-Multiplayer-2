// Decompiled with JetBrains decompiler
// Type: ftUniqueIDRegistry
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;

public static class ftUniqueIDRegistry
{
  public static Dictionary<int, int> Mapping = new Dictionary<int, int>();
  public static Dictionary<int, int> MappingInv = new Dictionary<int, int>();

  public static void Deregister(int id)
  {
    int instanceId = ftUniqueIDRegistry.GetInstanceId(id);
    if (instanceId < 0)
      return;
    ftUniqueIDRegistry.MappingInv.Remove(instanceId);
    ftUniqueIDRegistry.Mapping.Remove(id);
  }

  public static void Register(int id, int value)
  {
    if (ftUniqueIDRegistry.Mapping.ContainsKey(id))
      return;
    ftUniqueIDRegistry.Mapping[id] = value;
    ftUniqueIDRegistry.MappingInv[value] = id;
  }

  public static int GetInstanceId(int id)
  {
    int num;
    return !ftUniqueIDRegistry.Mapping.TryGetValue(id, out num) ? -1 : num;
  }

  public static int GetUID(int instanceId) => ftUniqueIDRegistry.MappingInv[instanceId];
}
