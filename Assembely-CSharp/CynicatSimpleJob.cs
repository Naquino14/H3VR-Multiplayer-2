using System.Threading;
using UnityEngine;

public abstract class CynicatSimpleJob
{
	public bool isAlive;

	public bool isRunning;

	public bool isScheduled;

	public Thread thread;

	private AutoResetEvent completedEvent = new AutoResetEvent(initialState: false);

	private AutoResetEvent scheduleEvent = new AutoResetEvent(initialState: false);

	public void Start()
	{
		if (thread != null && thread.IsAlive)
		{
			thread.Abort();
		}
		thread = new Thread(ThreadLoop);
		thread.Name = "My Worker Thread";
		ClearData();
		isAlive = true;
		isRunning = false;
		isScheduled = false;
		completedEvent.Reset();
		scheduleEvent.Reset();
		OnStart();
		thread.Start();
	}

	public void StartNow()
	{
		ClearData();
		isAlive = true;
		OnStart();
	}

	public void WaitForResults()
	{
		if (!isAlive)
		{
			Debug.Log("Attempted to WaitForResults() on a job that isn't alive, this would cause a thread freeze, do not do this.");
			return;
		}
		if (!isScheduled)
		{
			Debug.Log("Attempted to WaitForResults() on a job that hasn't been scheduled yet, this would cause a thread freeze, do not do this.");
			return;
		}
		completedEvent.WaitOne();
		isScheduled = false;
	}

	public void Schedule()
	{
		if (isRunning)
		{
			Debug.Log("Tried to schedule a job while it was still running, call WaitForResults() before scheduling it again!");
			return;
		}
		isScheduled = true;
		isRunning = true;
		OnSchedule();
		scheduleEvent.Set();
	}

	public void ScheduleNow()
	{
		isScheduled = true;
		isRunning = true;
		OnSchedule();
		OnExecute();
		isRunning = false;
		completedEvent.Set();
	}

	private void ThreadLoop()
	{
		while (true)
		{
			scheduleEvent.WaitOne();
			OnExecute();
			isRunning = false;
			completedEvent.Set();
		}
	}

	public void Abort()
	{
		if (thread != null && thread.IsAlive)
		{
			thread.Abort();
			thread = null;
		}
		isRunning = false;
		isAlive = false;
	}

	protected abstract void OnStart();

	public abstract void ClearData();

	protected abstract void OnExecute();

	protected abstract void OnSchedule();
}
