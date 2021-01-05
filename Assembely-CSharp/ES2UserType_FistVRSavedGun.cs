using System;
using FistVR;

public class ES2UserType_FistVRSavedGun : ES2Type
{
	public ES2UserType_FistVRSavedGun()
		: base(typeof(SavedGun))
	{
	}

	public override void Write(object obj, ES2Writer writer)
	{
		SavedGun savedGun = (SavedGun)obj;
		writer.Write(savedGun.FileName);
		writer.Write(savedGun.Components);
		writer.Write(savedGun.LoadedRoundsInMag);
		writer.Write(savedGun.LoadedRoundsInChambers);
		writer.Write(savedGun.SavedFlags);
		writer.Write(savedGun.DateMade);
	}

	public override object Read(ES2Reader reader)
	{
		SavedGun savedGun = new SavedGun();
		Read(reader, savedGun);
		return savedGun;
	}

	public override void Read(ES2Reader reader, object c)
	{
		SavedGun savedGun = (SavedGun)c;
		savedGun.FileName = reader.Read<string>();
		savedGun.Components = reader.ReadList<SavedGunComponent>();
		savedGun.LoadedRoundsInMag = reader.ReadList<FireArmRoundClass>();
		savedGun.LoadedRoundsInChambers = reader.ReadList<FireArmRoundClass>();
		savedGun.SavedFlags = reader.ReadList<string>();
		savedGun.DateMade = reader.Read<DateTime>();
	}
}
