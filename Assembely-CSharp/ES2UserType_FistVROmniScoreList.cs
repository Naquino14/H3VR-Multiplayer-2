using FistVR;

public class ES2UserType_FistVROmniScoreList : ES2Type
{
	public ES2UserType_FistVROmniScoreList()
		: base(typeof(OmniScoreList))
	{
	}

	public override void Write(object obj, ES2Writer writer)
	{
		OmniScoreList omniScoreList = (OmniScoreList)obj;
		writer.Write(omniScoreList.SequenceID);
		writer.Write(omniScoreList.Scores);
		writer.Write(omniScoreList.Trophy);
	}

	public override object Read(ES2Reader reader)
	{
		OmniScoreList omniScoreList = new OmniScoreList();
		Read(reader, omniScoreList);
		return omniScoreList;
	}

	public override void Read(ES2Reader reader, object c)
	{
		OmniScoreList omniScoreList = (OmniScoreList)c;
		omniScoreList.SequenceID = reader.Read<string>();
		omniScoreList.Scores = reader.ReadList<OmniScore>();
		omniScoreList.Trophy = reader.Read<int>();
	}
}
