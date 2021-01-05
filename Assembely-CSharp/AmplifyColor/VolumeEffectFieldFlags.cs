using System;
using System.Reflection;

namespace AmplifyColor
{
	[Serializable]
	public class VolumeEffectFieldFlags
	{
		public string fieldName;

		public string fieldType;

		public bool blendFlag;

		public VolumeEffectFieldFlags(FieldInfo pi)
		{
			fieldName = pi.Name;
			fieldType = pi.FieldType.FullName;
		}

		public VolumeEffectFieldFlags(VolumeEffectField field)
		{
			fieldName = field.fieldName;
			fieldType = field.fieldType;
			blendFlag = true;
		}
	}
}
