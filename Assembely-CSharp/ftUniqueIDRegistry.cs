using System.Collections.Generic;

public static class ftUniqueIDRegistry
{
	public static Dictionary<int, int> Mapping = new Dictionary<int, int>();

	public static Dictionary<int, int> MappingInv = new Dictionary<int, int>();

	public static void Deregister(int id)
	{
		int instanceId = GetInstanceId(id);
		if (instanceId >= 0)
		{
			MappingInv.Remove(instanceId);
			Mapping.Remove(id);
		}
	}

	public static void Register(int id, int value)
	{
		if (!Mapping.ContainsKey(id))
		{
			Mapping[id] = value;
			MappingInv[value] = id;
		}
	}

	public static int GetInstanceId(int id)
	{
		if (!Mapping.TryGetValue(id, out var value))
		{
			return -1;
		}
		return value;
	}

	public static int GetUID(int instanceId)
	{
		return MappingInv[instanceId];
	}
}
