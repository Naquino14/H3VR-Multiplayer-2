public struct DisjointSet
{
	public int[] parent;

	public int[] rank;

	public DisjointSet(int capacity)
	{
		rank = new int[capacity];
		parent = new int[capacity];
		Clear();
	}

	public void Clear()
	{
		for (int i = 0; i < parent.Length; i++)
		{
			parent[i] = i;
		}
		for (int j = 0; j < rank.Length; j++)
		{
			rank[j] = 0;
		}
	}

	public int Find(int i)
	{
		if (parent[i] != i)
		{
			parent[i] = Find(parent[i]);
		}
		return parent[i];
	}

	public void Union(int x, int y)
	{
		int num = Find(x);
		int num2 = Find(y);
		if (num != num2)
		{
			if (rank[num] < rank[num2])
			{
				parent[num] = num2;
				return;
			}
			if (rank[num] > rank[num2])
			{
				parent[num2] = num;
				return;
			}
			parent[num2] = num;
			rank[num]++;
		}
	}
}
