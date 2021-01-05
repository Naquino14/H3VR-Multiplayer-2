// Decompiled with JetBrains decompiler
// Type: CynicatSimpleJob
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Threading;
using UnityEngine;

public abstract class CynicatSimpleJob
{
  public bool isAlive;
  public bool isRunning;
  public bool isScheduled;
  public Thread thread;
  private AutoResetEvent completedEvent = new AutoResetEvent(false);
  private AutoResetEvent scheduleEvent = new AutoResetEvent(false);

  public void Start()
  {
    if (this.thread != null && this.thread.IsAlive)
      this.thread.Abort();
    this.thread = new Thread(new ThreadStart(this.ThreadLoop));
    this.thread.Name = "My Worker Thread";
    this.ClearData();
    this.isAlive = true;
    this.isRunning = false;
    this.isScheduled = false;
    this.completedEvent.Reset();
    this.scheduleEvent.Reset();
    this.OnStart();
    this.thread.Start();
  }

  public void StartNow()
  {
    this.ClearData();
    this.isAlive = true;
    this.OnStart();
  }

  public void WaitForResults()
  {
    if (!this.isAlive)
      Debug.Log((object) "Attempted to WaitForResults() on a job that isn't alive, this would cause a thread freeze, do not do this.");
    else if (!this.isScheduled)
    {
      Debug.Log((object) "Attempted to WaitForResults() on a job that hasn't been scheduled yet, this would cause a thread freeze, do not do this.");
    }
    else
    {
      this.completedEvent.WaitOne();
      this.isScheduled = false;
    }
  }

  public void Schedule()
  {
    if (this.isRunning)
    {
      Debug.Log((object) "Tried to schedule a job while it was still running, call WaitForResults() before scheduling it again!");
    }
    else
    {
      this.isScheduled = true;
      this.isRunning = true;
      this.OnSchedule();
      this.scheduleEvent.Set();
    }
  }

  public void ScheduleNow()
  {
    this.isScheduled = true;
    this.isRunning = true;
    this.OnSchedule();
    this.OnExecute();
    this.isRunning = false;
    this.completedEvent.Set();
  }

  private void ThreadLoop()
  {
    while (true)
    {
      this.scheduleEvent.WaitOne();
      this.OnExecute();
      this.isRunning = false;
      this.completedEvent.Set();
    }
  }

  public void Abort()
  {
    if (this.thread != null && this.thread.IsAlive)
    {
      this.thread.Abort();
      this.thread = (Thread) null;
    }
    this.isRunning = false;
    this.isAlive = false;
  }

  protected abstract void OnStart();

  public abstract void ClearData();

  protected abstract void OnExecute();

  protected abstract void OnSchedule();
}
