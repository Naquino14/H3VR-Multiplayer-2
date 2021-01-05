// Decompiled with JetBrains decompiler
// Type: Alloy.MapTextureChannelMapping
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

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
        if (this.OutputChannels.HasFlag((Enum) MapChannel.R))
          return 0;
        if (this.OutputChannels.HasFlag((Enum) MapChannel.G))
          return 1;
        if (this.OutputChannels.HasFlag((Enum) MapChannel.B))
          return 2;
        if (this.OutputChannels.HasFlag((Enum) MapChannel.A))
          return 3;
        UnityEngine.Debug.LogError((object) " Packed map does not have any output channels");
        return 0;
      }
    }

    [DebuggerHidden]
    private IEnumerable<int> GetIndices(MapChannel channel)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      MapTextureChannelMapping.\u003CGetIndices\u003Ec__Iterator0 indicesCIterator0 = new MapTextureChannelMapping.\u003CGetIndices\u003Ec__Iterator0()
      {
        channel = channel
      };
      // ISSUE: reference to a compiler-generated field
      indicesCIterator0.\u0024PC = -2;
      return (IEnumerable<int>) indicesCIterator0;
    }

    public IEnumerable<int> InputIndices => this.GetIndices(this.InputChannels);

    public IEnumerable<int> OutputIndices => this.GetIndices(this.OutputChannels);

    private string GetChannelString(MapChannel channel)
    {
      StringBuilder stringBuilder = new StringBuilder(5);
      if (channel.HasFlag((Enum) MapChannel.R))
        stringBuilder.Append('R');
      if (channel.HasFlag((Enum) MapChannel.G))
        stringBuilder.Append('G');
      if (channel.HasFlag((Enum) MapChannel.B))
        stringBuilder.Append('B');
      if (channel.HasFlag((Enum) MapChannel.A))
        stringBuilder.Append('A');
      return stringBuilder.ToString();
    }

    public string InputString => this.GetChannelString(this.InputChannels);

    public string OutputString => this.GetChannelString(this.OutputChannels);

    public bool UseNormals => this.OutputVariance || this.RoughnessCorrect;
  }
}
