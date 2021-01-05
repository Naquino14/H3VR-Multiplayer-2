using FistVR;

public class ES2UserType_FistVROmniScore : ES2Type
{
	public ES2UserType_FistVROmniScore()
		: base(typeof(OmniScore))
	{
	}

	public override void Write(object obj, ES2Writer writer)
	{
		OmniScore omniScore = (OmniScore)obj;
		writer.Write(omniScore.Score);
		writer.Write(omniScore.Name);
	}

	public override object Read(ES2Reader reader)
	{
		OmniScore omniScore = new OmniScore();
		Read(reader, omniScore);
		return omniScore;
	}

	public override void Read(ES2Reader reader, object c)
	{
		OmniScore omniScore = (OmniScore)c;
		omniScore.Score = reader.Read<int>();
		omniScore.Name = reader.Read<string>();
	}
}
