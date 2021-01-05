using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Alloy
{
	[Serializable]
	public class MapTextureChannelMapping : BaseTextureChannelMapping
	{
		public bool CanInvert;

		public bool InvertByDefault;

		[EnumFlags]
		public MapChannel InputChannels;

		[EnumFlags]
		public MapChannel OutputChannels;

		public bool RoughnessCorrect;

		public bool OutputVariance;

		public bool HideChannel;

		public TextureValueChannelMode DefaultMode;

		public int MainIndex
		{
			get
			{
				if (OutputChannels.HasFlag(MapChannel.R))
				{
					return 0;
				}
				if (OutputChannels.HasFlag(MapChannel.G))
				{
					return 1;
				}
				if (OutputChannels.HasFlag(MapChannel.B))
				{
					return 2;
				}
				if (OutputChannels.HasFlag(MapChannel.A))
				{
					return 3;
				}
				Debug.LogError(" Packed map does not have any output channels");
				return 0;
			}
		}

		public IEnumerable<int> InputIndices => GetIndices(InputChannels);

		public IEnumerable<int> OutputIndices => GetIndices(OutputChannels);

		public string InputString => GetChannelString(InputChannels);

		public string OutputString => GetChannelString(OutputChannels);

		public bool UseNormals => OutputVariance || RoughnessCorrect;

		private IEnumerable<int> GetIndices(MapChannel channel)
		{
			if (channel.HasFlag(MapChannel.R))
			{
				yield return 0;
			}
			if (channel.HasFlag(MapChannel.G))
			{
				yield return 1;
			}
			if (channel.HasFlag(MapChannel.B))
			{
				yield return 2;
			}
			if (channel.HasFlag(MapChannel.A))
			{
				yield return 3;
			}
		}

		private string GetChannelString(MapChannel channel)
		{
			StringBuilder stringBuilder = new StringBuilder(5);
			if (channel.HasFlag(MapChannel.R))
			{
				stringBuilder.Append('R');
			}
			if (channel.HasFlag(MapChannel.G))
			{
				stringBuilder.Append('G');
			}
			if (channel.HasFlag(MapChannel.B))
			{
				stringBuilder.Append('B');
			}
			if (channel.HasFlag(MapChannel.A))
			{
				stringBuilder.Append('A');
			}
			return stringBuilder.ToString();
		}
	}
}
