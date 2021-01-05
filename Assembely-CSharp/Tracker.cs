using System;

public class Tracker<T> where T : ITracked
{
	public static T[] All = new T[32];

	public static int Count;

	public static void Register(T item)
	{
		if (Count >= All.Length)
		{
			Array.Resize(ref All, All.Length * 2);
		}
		All[Count] = item;
		item.Index = Count;
		Count++;
	}

	public static void Deregister(T item)
	{
		if (item.Index != -1)
		{
			Count--;
			if (Count >= 0)
			{
				All[Count].Index = item.Index;
				All[item.Index] = All[Count];
			}
		}
	}
}
