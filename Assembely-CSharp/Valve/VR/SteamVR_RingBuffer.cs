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
			buffer = new T[size];
			currentIndex = 0;
		}

		public void Add(T newElement)
		{
			buffer[currentIndex] = newElement;
			StepForward();
		}

		public virtual void StepForward()
		{
			lastElement = buffer[currentIndex];
			currentIndex++;
			if (currentIndex >= buffer.Length)
			{
				currentIndex = 0;
			}
			cleared = false;
		}

		public virtual T GetAtIndex(int atIndex)
		{
			if (atIndex < 0)
			{
				atIndex += buffer.Length;
			}
			return buffer[atIndex];
		}

		public virtual T GetLast()
		{
			return lastElement;
		}

		public virtual int GetLastIndex()
		{
			int num = currentIndex - 1;
			if (num < 0)
			{
				num += buffer.Length;
			}
			return num;
		}

		public void Clear()
		{
			if (!cleared && buffer != null)
			{
				for (int i = 0; i < buffer.Length; i++)
				{
					buffer[i] = default(T);
				}
				lastElement = default(T);
				currentIndex = 0;
				cleared = true;
			}
		}
	}
}
