using System.ComponentModel;
using UnityEngine;

[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class ES2_AudioClip : ES2Type
{
	public ES2_AudioClip()
		: base(typeof(AudioClip))
	{
		key = 25;
	}

	public override void Write(object data, ES2Writer writer)
	{
		AudioClip audioClip = (AudioClip)data;
		writer.writer.Write((byte)5);
		float[] array = new float[audioClip.samples * audioClip.channels];
		audioClip.GetData(array, 0);
		writer.writer.Write(audioClip.name);
		writer.writer.Write(audioClip.samples);
		writer.writer.Write(audioClip.channels);
		writer.writer.Write(audioClip.frequency);
		writer.Write(array);
	}

	public override object Read(ES2Reader reader)
	{
		AudioClip audioClip = null;
		string name = string.Empty;
		int lengthSamples = 0;
		int channels = 0;
		int frequency = 0;
		int num = reader.reader.ReadByte();
		for (int i = 0; i < num; i++)
		{
			switch (i)
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
				audioClip = AudioClip.Create(name, lengthSamples, channels, frequency, stream: false);
				audioClip.SetData(reader.ReadArray<float>(new ES2_float()), 0);
				break;
			default:
				return audioClip;
			}
		}
		return audioClip;
	}
}
