using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor
{
	[Serializable]
	public class VolumeEffect
	{
		public AmplifyColorBase gameObject;

		public List<VolumeEffectComponent> components;

		public VolumeEffect(AmplifyColorBase effect)
		{
			gameObject = effect;
			components = new List<VolumeEffectComponent>();
		}

		public static VolumeEffect BlendValuesToVolumeEffect(VolumeEffectFlags flags, VolumeEffect volume1, VolumeEffect volume2, float blend)
		{
			VolumeEffect volumeEffect = new VolumeEffect(volume1.gameObject);
			foreach (VolumeEffectComponentFlags component in flags.components)
			{
				if (!component.blendFlag)
				{
					continue;
				}
				VolumeEffectComponent volumeEffectComponent = volume1.FindEffectComponent(component.componentName);
				VolumeEffectComponent volumeEffectComponent2 = volume2.FindEffectComponent(component.componentName);
				if (volumeEffectComponent == null || volumeEffectComponent2 == null)
				{
					continue;
				}
				VolumeEffectComponent volumeEffectComponent3 = new VolumeEffectComponent(volumeEffectComponent.componentName);
				foreach (VolumeEffectFieldFlags componentField in component.componentFields)
				{
					if (!componentField.blendFlag)
					{
						continue;
					}
					VolumeEffectField volumeEffectField = volumeEffectComponent.FindEffectField(componentField.fieldName);
					VolumeEffectField volumeEffectField2 = volumeEffectComponent2.FindEffectField(componentField.fieldName);
					if (volumeEffectField != null && volumeEffectField2 != null)
					{
						VolumeEffectField volumeEffectField3 = new VolumeEffectField(volumeEffectField.fieldName, volumeEffectField.fieldType);
						switch (volumeEffectField3.fieldType)
						{
						case "System.Single":
							volumeEffectField3.valueSingle = Mathf.Lerp(volumeEffectField.valueSingle, volumeEffectField2.valueSingle, blend);
							break;
						case "System.Boolean":
							volumeEffectField3.valueBoolean = volumeEffectField2.valueBoolean;
							break;
						case "UnityEngine.Vector2":
							volumeEffectField3.valueVector2 = Vector2.Lerp(volumeEffectField.valueVector2, volumeEffectField2.valueVector2, blend);
							break;
						case "UnityEngine.Vector3":
							volumeEffectField3.valueVector3 = Vector3.Lerp(volumeEffectField.valueVector3, volumeEffectField2.valueVector3, blend);
							break;
						case "UnityEngine.Vector4":
							volumeEffectField3.valueVector4 = Vector4.Lerp(volumeEffectField.valueVector4, volumeEffectField2.valueVector4, blend);
							break;
						case "UnityEngine.Color":
							volumeEffectField3.valueColor = Color.Lerp(volumeEffectField.valueColor, volumeEffectField2.valueColor, blend);
							break;
						}
						volumeEffectComponent3.fields.Add(volumeEffectField3);
					}
				}
				volumeEffect.components.Add(volumeEffectComponent3);
			}
			return volumeEffect;
		}

		public VolumeEffectComponent AddComponent(Component c, VolumeEffectComponentFlags compFlags)
		{
			if (compFlags == null)
			{
				VolumeEffectComponent volumeEffectComponent = new VolumeEffectComponent(string.Concat(c.GetType(), string.Empty));
				components.Add(volumeEffectComponent);
				return volumeEffectComponent;
			}
			VolumeEffectComponent volumeEffectComponent2;
			if ((volumeEffectComponent2 = FindEffectComponent(string.Concat(c.GetType(), string.Empty))) != null)
			{
				volumeEffectComponent2.UpdateComponent(c, compFlags);
				return volumeEffectComponent2;
			}
			VolumeEffectComponent volumeEffectComponent3 = new VolumeEffectComponent(c, compFlags);
			components.Add(volumeEffectComponent3);
			return volumeEffectComponent3;
		}

		public void RemoveEffectComponent(VolumeEffectComponent comp)
		{
			components.Remove(comp);
		}

		public void UpdateVolume()
		{
			if (gameObject == null)
			{
				return;
			}
			VolumeEffectFlags effectFlags = gameObject.EffectFlags;
			foreach (VolumeEffectComponentFlags component2 in effectFlags.components)
			{
				if (component2.blendFlag)
				{
					Component component = gameObject.GetComponent(component2.componentName);
					if (component != null)
					{
						AddComponent(component, component2);
					}
				}
			}
		}

		public void SetValues(AmplifyColorBase targetColor)
		{
			VolumeEffectFlags effectFlags = targetColor.EffectFlags;
			GameObject gameObject = targetColor.gameObject;
			foreach (VolumeEffectComponentFlags component2 in effectFlags.components)
			{
				if (!component2.blendFlag)
				{
					continue;
				}
				Component component = gameObject.GetComponent(component2.componentName);
				VolumeEffectComponent volumeEffectComponent = FindEffectComponent(component2.componentName);
				if (component == null || volumeEffectComponent == null)
				{
					continue;
				}
				foreach (VolumeEffectFieldFlags componentField in component2.componentFields)
				{
					if (!componentField.blendFlag)
					{
						continue;
					}
					FieldInfo field = component.GetType().GetField(componentField.fieldName);
					VolumeEffectField volumeEffectField = volumeEffectComponent.FindEffectField(componentField.fieldName);
					if (field != null && volumeEffectField != null)
					{
						switch (field.FieldType.FullName)
						{
						case "System.Single":
							field.SetValue(component, volumeEffectField.valueSingle);
							break;
						case "System.Boolean":
							field.SetValue(component, volumeEffectField.valueBoolean);
							break;
						case "UnityEngine.Vector2":
							field.SetValue(component, volumeEffectField.valueVector2);
							break;
						case "UnityEngine.Vector3":
							field.SetValue(component, volumeEffectField.valueVector3);
							break;
						case "UnityEngine.Vector4":
							field.SetValue(component, volumeEffectField.valueVector4);
							break;
						case "UnityEngine.Color":
							field.SetValue(component, volumeEffectField.valueColor);
							break;
						}
					}
				}
			}
		}

		public void BlendValues(AmplifyColorBase targetColor, VolumeEffect other, float blendAmount)
		{
			VolumeEffectFlags effectFlags = targetColor.EffectFlags;
			GameObject gameObject = targetColor.gameObject;
			for (int i = 0; i < effectFlags.components.Count; i++)
			{
				VolumeEffectComponentFlags volumeEffectComponentFlags = effectFlags.components[i];
				if (!volumeEffectComponentFlags.blendFlag)
				{
					continue;
				}
				Component component = gameObject.GetComponent(volumeEffectComponentFlags.componentName);
				VolumeEffectComponent volumeEffectComponent = FindEffectComponent(volumeEffectComponentFlags.componentName);
				VolumeEffectComponent volumeEffectComponent2 = other.FindEffectComponent(volumeEffectComponentFlags.componentName);
				if (component == null || volumeEffectComponent == null || volumeEffectComponent2 == null)
				{
					continue;
				}
				for (int j = 0; j < volumeEffectComponentFlags.componentFields.Count; j++)
				{
					VolumeEffectFieldFlags volumeEffectFieldFlags = volumeEffectComponentFlags.componentFields[j];
					if (!volumeEffectFieldFlags.blendFlag)
					{
						continue;
					}
					FieldInfo field = component.GetType().GetField(volumeEffectFieldFlags.fieldName);
					VolumeEffectField volumeEffectField = volumeEffectComponent.FindEffectField(volumeEffectFieldFlags.fieldName);
					VolumeEffectField volumeEffectField2 = volumeEffectComponent2.FindEffectField(volumeEffectFieldFlags.fieldName);
					if (field != null && volumeEffectField != null && volumeEffectField2 != null)
					{
						switch (field.FieldType.FullName)
						{
						case "System.Single":
							field.SetValue(component, Mathf.Lerp(volumeEffectField.valueSingle, volumeEffectField2.valueSingle, blendAmount));
							break;
						case "System.Boolean":
							field.SetValue(component, volumeEffectField2.valueBoolean);
							break;
						case "UnityEngine.Vector2":
							field.SetValue(component, Vector2.Lerp(volumeEffectField.valueVector2, volumeEffectField2.valueVector2, blendAmount));
							break;
						case "UnityEngine.Vector3":
							field.SetValue(component, Vector3.Lerp(volumeEffectField.valueVector3, volumeEffectField2.valueVector3, blendAmount));
							break;
						case "UnityEngine.Vector4":
							field.SetValue(component, Vector4.Lerp(volumeEffectField.valueVector4, volumeEffectField2.valueVector4, blendAmount));
							break;
						case "UnityEngine.Color":
							field.SetValue(component, Color.Lerp(volumeEffectField.valueColor, volumeEffectField2.valueColor, blendAmount));
							break;
						}
					}
				}
			}
		}

		public VolumeEffectComponent FindEffectComponent(string compName)
		{
			for (int i = 0; i < components.Count; i++)
			{
				if (components[i].componentName == compName)
				{
					return components[i];
				}
			}
			return null;
		}

		public static Component[] ListAcceptableComponents(AmplifyColorBase go)
		{
			if (go == null)
			{
				return new Component[0];
			}
			Component[] source = go.GetComponents(typeof(Component));
			return source.Where((Component comp) => comp != null && !string.Concat(comp.GetType(), string.Empty).StartsWith("UnityEngine.") && comp.GetType() != typeof(AmplifyColorBase)).ToArray();
		}

		public string[] GetComponentNames()
		{
			return components.Select((VolumeEffectComponent r) => r.componentName).ToArray();
		}
	}
}
