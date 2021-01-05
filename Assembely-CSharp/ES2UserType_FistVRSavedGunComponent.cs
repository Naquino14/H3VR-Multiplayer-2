using FistVR;
using UnityEngine;

public class ES2UserType_FistVRSavedGunComponent : ES2Type
{
	public ES2UserType_FistVRSavedGunComponent()
		: base(typeof(SavedGunComponent))
	{
	}

	public override void Write(object obj, ES2Writer writer)
	{
		SavedGunComponent savedGunComponent = (SavedGunComponent)obj;
		writer.Write(savedGunComponent.Index);
		writer.Write(savedGunComponent.ObjectID);
		writer.Write(savedGunComponent.PosOffset);
		writer.Write(savedGunComponent.OrientationForward);
		writer.Write(savedGunComponent.OrientationUp);
		writer.Write(savedGunComponent.ObjectAttachedTo);
		writer.Write(savedGunComponent.MountAttachedTo);
		writer.Write(savedGunComponent.isFirearm);
		writer.Write(savedGunComponent.isMagazine);
		writer.Write(savedGunComponent.isAttachment);
		writer.Write(savedGunComponent.Flags);
	}

	public override object Read(ES2Reader reader)
	{
		SavedGunComponent savedGunComponent = new SavedGunComponent();
		Read(reader, savedGunComponent);
		return savedGunComponent;
	}

	public override void Read(ES2Reader reader, object c)
	{
		SavedGunComponent savedGunComponent = (SavedGunComponent)c;
		savedGunComponent.Index = reader.Read<int>();
		savedGunComponent.ObjectID = reader.Read<string>();
		savedGunComponent.PosOffset = reader.Read<Vector3>();
		savedGunComponent.OrientationForward = reader.Read<Vector3>();
		savedGunComponent.OrientationUp = reader.Read<Vector3>();
		savedGunComponent.ObjectAttachedTo = reader.Read<int>();
		savedGunComponent.MountAttachedTo = reader.Read<int>();
		savedGunComponent.isFirearm = reader.Read<bool>();
		savedGunComponent.isMagazine = reader.Read<bool>();
		savedGunComponent.isAttachment = reader.Read<bool>();
		savedGunComponent.Flags = reader.ReadDictionary<string, string>();
	}
}
