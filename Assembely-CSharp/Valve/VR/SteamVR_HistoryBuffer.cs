using System;
using UnityEngine;

namespace Valve.VR
{
	public class SteamVR_HistoryBuffer : SteamVR_RingBuffer<SteamVR_HistoryStep>
	{
		public SteamVR_HistoryBuffer(int size)
			: base(size)
		{
		}

		public void Update(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
		{
			if (buffer[currentIndex] == null)
			{
				buffer[currentIndex] = new SteamVR_HistoryStep();
			}
			buffer[currentIndex].position = position;
			buffer[currentIndex].rotation = rotation;
			buffer[currentIndex].velocity = velocity;
			buffer[currentIndex].angularVelocity = angularVelocity;
			buffer[currentIndex].timeInTicks = DateTime.Now.Ticks;
			StepForward();
		}

		public float GetVelocityMagnitudeTrend(int toIndex = -1, int fromIndex = -1)
		{
			if (toIndex == -1)
			{
				toIndex = currentIndex - 1;
			}
			if (toIndex < 0)
			{
				toIndex += buffer.Length;
			}
			if (fromIndex == -1)
			{
				fromIndex = toIndex - 1;
			}
			if (fromIndex < 0)
			{
				fromIndex += buffer.Length;
			}
			SteamVR_HistoryStep steamVR_HistoryStep = buffer[toIndex];
			SteamVR_HistoryStep steamVR_HistoryStep2 = buffer[fromIndex];
			if (IsValid(steamVR_HistoryStep) && IsValid(steamVR_HistoryStep2))
			{
				return steamVR_HistoryStep.velocity.sqrMagnitude - steamVR_HistoryStep2.velocity.sqrMagnitude;
			}
			return 0f;
		}

		public bool IsValid(SteamVR_HistoryStep step)
		{
			return step != null && step.timeInTicks != -1;
		}

		public int GetTopVelocity(int forFrames, int addFrames = 0)
		{
			int num = currentIndex;
			float num2 = 0f;
			int num3 = currentIndex;
			while (forFrames > 0)
			{
				forFrames--;
				num3--;
				if (num3 < 0)
				{
					num3 = buffer.Length - 1;
				}
				SteamVR_HistoryStep step = buffer[num3];
				if (!IsValid(step))
				{
					break;
				}
				float sqrMagnitude = buffer[num3].velocity.sqrMagnitude;
				if (sqrMagnitude > num2)
				{
					num = num3;
					num2 = sqrMagnitude;
				}
			}
			num += addFrames;
			if (num >= buffer.Length)
			{
				num -= buffer.Length;
			}
			return num;
		}

		public void GetAverageVelocities(out Vector3 velocity, out Vector3 angularVelocity, int forFrames, int startFrame = -1)
		{
			velocity = Vector3.zero;
			angularVelocity = Vector3.zero;
			if (startFrame == -1)
			{
				startFrame = currentIndex - 1;
			}
			if (startFrame < 0)
			{
				startFrame = buffer.Length - 1;
			}
			int num = startFrame - forFrames;
			if (num < 0)
			{
				num += buffer.Length;
			}
			Vector3 zero = Vector3.zero;
			Vector3 zero2 = Vector3.zero;
			float num2 = 0f;
			int num3 = startFrame;
			while (forFrames > 0)
			{
				forFrames--;
				num3--;
				if (num3 < 0)
				{
					num3 = buffer.Length - 1;
				}
				SteamVR_HistoryStep steamVR_HistoryStep = buffer[num3];
				if (!IsValid(steamVR_HistoryStep))
				{
					break;
				}
				num2 += 1f;
				zero += steamVR_HistoryStep.velocity;
				zero2 += steamVR_HistoryStep.angularVelocity;
			}
			velocity = zero / num2;
			angularVelocity = zero2 / num2;
		}
	}
}
