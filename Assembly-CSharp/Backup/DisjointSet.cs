// Decompiled with JetBrains decompiler
// Type: DisjointSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

public struct DisjointSet
{
  public int[] parent;
  public int[] rank;

  public DisjointSet(int capacity)
  {
    this.rank = new int[capacity];
    this.parent = new int[capacity];
    this.Clear();
  }

  public void Clear()
  {
    for (int index = 0; index < this.parent.Length; ++index)
      this.parent[index] = index;
    for (int index = 0; index < this.rank.Length; ++index)
      this.rank[index] = 0;
  }

  public int Find(int i)
  {
    if (this.parent[i] != i)
      this.parent[i] = this.Find(this.parent[i]);
    return this.parent[i];
  }

  public void Union(int x, int y)
  {
    int index1 = this.Find(x);
    int index2 = this.Find(y);
    if (index1 == index2)
      return;
    if (this.rank[index1] < this.rank[index2])
      this.parent[index1] = index2;
    else if (this.rank[index1] > this.rank[index2])
    {
      this.parent[index2] = index1;
    }
    else
    {
      this.parent[index2] = index1;
      ++this.rank[index1];
    }
  }
}
