// Decompiled with JetBrains decompiler
// Type: ES2_AudioClip
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.ComponentModel;
using UnityEngine;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ES2_AudioClip : ES2Type
{
  public ES2_AudioClip()
    : base(typeof (AudioClip))
    => this.key = (byte) 25;

  public override void Write(object data, ES2Writer writer)
  {
    AudioClip audioClip = (AudioClip) data;
    writer.writer.Write((byte) 5);
    float[] data1 = new float[audioClip.samples * audioClip.channels];
    audioClip.GetData(data1, 0);
    writer.writer.Write(audioClip.name);
    writer.writer.Write(audioClip.samples);
    writer.writer.Write(audioClip.channels);
    writer.writer.Write(audioClip.frequency);
    writer.Write<float>(data1);
  }

  public override object Read(ES2Reader reader)
  {
    AudioClip audioClip = (AudioClip) null;
    string name = string.Empty;
    int lengthSamples = 0;
    int channels = 0;
    int frequency = 0;
    int num = (int) reader.reader.ReadByte();
    for (int index = 0; index < num; ++index)
    {
      switch (index)
      {
        case 0:
          name = reader.reader.ReadString();
          break;
        case 1:
          lengthSamples = reader.reader.ReadInt32();
          break;
        case 2:
          channels = reader.reader.ReadInt32();
          break;
        case 3:
          frequency = reader.reader.ReadInt32();
          break;
        case 4:
          audioClip = AudioClip.Create(name, lengthSamples, channels, frequency, false);
          audioClip.SetData(reader.ReadArray<float>((ES2Type) new ES2_float()), 0);
          break;
        default:
          return (object) audioClip;
      }
    }
    return (object) audioClip;
  }
}
