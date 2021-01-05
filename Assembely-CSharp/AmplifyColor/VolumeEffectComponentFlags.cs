using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	[Serializable]
	public class VolumeEffectComponentFlags
	{
		public string componentName;

		public List<VolumeEffectFieldFlags> componentFields;

		public bool blendFlag;

		public VolumeEffectComponentFlags(string name)
		{
			componentName = name;
			componentFields = new List<VolumeEffectFieldFlags>();
		}

		public VolumeEffectComponentFlags(VolumeEffectComponent comp)
			: this(comp.componentName)
		{
			blendFlag = true;
			foreach (VolumeEffectField field in comp.fields)
			{
				if (VolumeEffectField.IsValidType(field.fieldType))
				{
					componentFields.Add(new VolumeEffectFieldFlags(field));
				}
			}
		}

		public VolumeEffectComponentFlags(Component c)
			: this(string.Concat(c.GetType(), string.Empty))
		{
			FieldInfo[] fields = c.GetType().GetFields();
			FieldInfo[] array = fields;
			foreach (FieldInfo fieldInfo in array)
			{
				if (VolumeEffectField.IsValidType(fieldInfo.FieldType.FullName))
				{
					componentFields.Add(new VolumeEffectFieldFlags(fieldInfo));
				}
			}
		}

		public void UpdateComponentFlags(VolumeEffectComponent comp)
		{
			foreach (VolumeEffectField field in comp.fields)
			{
				if (componentFields.Find((VolumeEffectFieldFlags s) => s.fieldName == field.fieldName) == null && VolumeEffectField.IsValidType(field.fieldType))
				{
					componentFields.Add(new VolumeEffectFieldFlags(field));
				}
			}
		}

		public void UpdateComponentFlags(Component c)
		{
			FieldInfo[] fields = c.GetType().GetFields();
			FieldInfo[] array = fields;
			foreach (FieldInfo pi in array)
			{
				if (!componentFields.Exists((VolumeEffectFieldFlags s) => s.fieldName == pi.Name) && VolumeEffectField.IsValidType(pi.FieldType.FullName))
				{
					componentFields.Add(new VolumeEffectFieldFlags(pi));
				}
			}
		}

		public string[] GetFieldNames()
		{
			return (from r in componentFields
				where r.blendFlag
				select r.fieldName).ToArray();
		}
	}
}
