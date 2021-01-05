// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_RingBuffer`1
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace Valve.VR
{
  public class SteamVR_RingBuffer<T>
  {
    protected T[] buffer;
    protected int currentIndex;
    protected T lastElement;
    private bool cleared;

    public SteamVR_RingBuffer(int size)
    {
      this.buffer = new T[size];
      this.currentIndex = 0;
    }

    public void Add(T newElement)
    {
      this.buffer[this.currentIndex] = newElement;
      this.StepForward();
    }

    public virtual void StepForward()
    {
      this.lastElement = this.buffer[this.currentIndex];
      ++this.currentIndex;
      if (this.currentIndex >= this.buffer.Length)
        this.currentIndex = 0;
      this.cleared = false;
    }

    public virtual T GetAtIndex(int atIndex)
    {
      if (atIndex < 0)
        atIndex += this.buffer.Length;
      return this.buffer[atIndex];
    }

    public virtual T GetLast() => this.lastElement;

    public virtual int GetLastIndex()
    {
      int num = this.currentIndex - 1;
      if (num < 0)
        num += this.buffer.Length;
      return num;
    }

    public void Clear()
    {
      if (this.cleared || this.buffer == null)
        return;
      for (int index = 0; index < this.buffer.Length; ++index)
        this.buffer[index] = default (T);
      this.lastElement = default (T);
      this.currentIndex = 0;
      this.cleared = true;
    }
  }
}
