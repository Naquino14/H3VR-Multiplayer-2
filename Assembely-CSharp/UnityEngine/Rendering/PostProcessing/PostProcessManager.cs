using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityEngine.Rendering.PostProcessing
{
	public sealed class PostProcessManager
	{
		private static PostProcessManager s_Instance;

		private const int k_MaxLayerCount = 32;

		private readonly List<PostProcessVolume>[] m_Volumes;

		private readonly bool[] m_SortNeeded;

		private readonly List<PostProcessEffectSettings> m_BaseSettings;

		private readonly List<Collider> m_TempColliders;

		public readonly Dictionary<Type, PostProcessAttribute> settingsTypes;

		public static PostProcessManager instance
		{
			get
			{
				if (s_Instance == null)
				{
					s_Instance = new PostProcessManager();
				}
				return s_Instance;
			}
		}

		private PostProcessManager()
		{
			m_Volumes = new List<PostProcessVolume>[32];
			m_SortNeeded = new bool[32];
			m_BaseSettings = new List<PostProcessEffectSettings>();
			m_TempColliders = new List<Collider>(5);
			settingsTypes = new Dictionary<Type, PostProcessAttribute>();
			ReloadBaseTypes();
		}

		private void CleanBaseTypes()
		{
			settingsTypes.Clear();
			foreach (PostProcessEffectSettings baseSetting in m_BaseSettings)
			{
				RuntimeUtilities.Destroy(baseSetting);
			}
			m_BaseSettings.Clear();
		}

		private void ReloadBaseTypes()
		{
			CleanBaseTypes();
			IEnumerable<Type> enumerable = AppDomain.CurrentDomain.GetAssemblies().SelectMany((Assembly a) => from t in a.GetTypes()
				where t.IsSubclassOf(typeof(PostProcessEffectSettings)) && t.IsDefined(typeof(PostProcessAttribute), inherit: false)
				select t);
			foreach (Type item in enumerable)
			{
				settingsTypes.Add(item, item.GetAttribute<PostProcessAttribute>());
				PostProcessEffectSettings postProcessEffectSettings = (PostProcessEffectSettings)ScriptableObject.CreateInstance(item);
				postProcessEffectSettings.SetAllOverridesTo(state: true, excludeEnabled: false);
				m_BaseSettings.Add(postProcessEffectSettings);
			}
		}

		public void GetActiveVolumes(PostProcessLayer layer, List<PostProcessVolume> results)
		{
			int value = layer.volumeLayer.value;
			Transform volumeTrigger = layer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = ((!flag) ? volumeTrigger.position : Vector3.zero);
			for (int i = 0; i < 32; i++)
			{
				if ((value & (1 << i)) == 0)
				{
					continue;
				}
				List<PostProcessVolume> list = m_Volumes[i];
				if (list == null)
				{
					continue;
				}
				foreach (PostProcessVolume item in list)
				{
					if (!item.enabled || item.profileRef == null || item.weight <= 0f)
					{
						continue;
					}
					if (item.isGlobal)
					{
						results.Add(item);
					}
					else
					{
						if (flag)
						{
							continue;
						}
						List<Collider> tempColliders = m_TempColliders;
						item.GetComponents(tempColliders);
						if (tempColliders.Count == 0)
						{
							continue;
						}
						float num = float.PositiveInfinity;
						foreach (Collider item2 in tempColliders)
						{
							if (item2.enabled)
							{
								Vector3 vector2 = item2.ClosestPoint(vector);
								float sqrMagnitude = ((vector2 - vector) / 2f).sqrMagnitude;
								if (sqrMagnitude < num)
								{
									num = sqrMagnitude;
								}
							}
						}
						tempColliders.Clear();
						float num2 = item.blendDistance * item.blendDistance;
						if (num <= num2)
						{
							results.Add(item);
						}
					}
				}
			}
		}

		public PostProcessVolume GetHighestPriorityVolume(PostProcessLayer layer)
		{
			if (layer == null)
			{
				throw new ArgumentNullException("layer");
			}
			return GetHighestPriorityVolume(layer.volumeLayer);
		}

		public PostProcessVolume GetHighestPriorityVolume(LayerMask mask)
		{
			float num = float.NegativeInfinity;
			PostProcessVolume result = null;
			for (int i = 0; i < 32; i++)
			{
				if (((int)mask & (1 << i)) == 0)
				{
					continue;
				}
				List<PostProcessVolume> list = m_Volumes[i];
				if (list == null)
				{
					continue;
				}
				foreach (PostProcessVolume item in list)
				{
					if (item.priority > num)
					{
						num = item.priority;
						result = item;
					}
				}
			}
			return result;
		}

		public PostProcessVolume QuickVolume(int layer, float priority, params PostProcessEffectSettings[] settings)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = "Quick Volume";
			gameObject.layer = layer;
			gameObject.hideFlags = HideFlags.HideAndDontSave;
			GameObject gameObject2 = gameObject;
			PostProcessVolume postProcessVolume = gameObject2.AddComponent<PostProcessVolume>();
			postProcessVolume.priority = priority;
			postProcessVolume.isGlobal = true;
			PostProcessProfile profile = postProcessVolume.profile;
			foreach (PostProcessEffectSettings effect in settings)
			{
				profile.AddSettings(effect);
			}
			return postProcessVolume;
		}

		internal void SetLayerDirty(int layer)
		{
			m_SortNeeded[layer] = true;
		}

		internal void UpdateVolumeLayer(PostProcessVolume volume, int prevLayer, int newLayer)
		{
			Unregister(volume, prevLayer);
			Register(volume, newLayer);
		}

		private void Register(PostProcessVolume volume, int layer)
		{
			List<PostProcessVolume> list = m_Volumes[layer];
			if (list == null)
			{
				list = new List<PostProcessVolume>();
				m_Volumes[layer] = list;
			}
			list.Add(volume);
			SetLayerDirty(layer);
		}

		internal void Register(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			Register(volume, layer);
		}

		private void Unregister(PostProcessVolume volume, int layer)
		{
			m_Volumes[layer]?.Remove(volume);
		}

		internal void Unregister(PostProcessVolume volume)
		{
			int layer = volume.gameObject.layer;
			Unregister(volume, layer);
		}

		internal void UpdateSettings(PostProcessLayer postProcessLayer)
		{
			postProcessLayer.OverrideSettings(m_BaseSettings, 1f);
			int value = postProcessLayer.volumeLayer.value;
			Transform volumeTrigger = postProcessLayer.volumeTrigger;
			bool flag = volumeTrigger == null;
			Vector3 vector = ((!flag) ? volumeTrigger.position : Vector3.zero);
			for (int i = 0; i < 32; i++)
			{
				if ((value & (1 << i)) == 0)
				{
					continue;
				}
				List<PostProcessVolume> list = m_Volumes[i];
				if (list == null)
				{
					continue;
				}
				if (m_SortNeeded[i])
				{
					SortByPriority(list);
					m_SortNeeded[i] = false;
				}
				foreach (PostProcessVolume item in list)
				{
					if (!item.enabled || item.profileRef == null || item.weight <= 0f)
					{
						continue;
					}
					List<PostProcessEffectSettings> settings = item.profileRef.settings;
					if (item.isGlobal)
					{
						postProcessLayer.OverrideSettings(settings, item.weight);
					}
					else
					{
						if (flag)
						{
							continue;
						}
						List<Collider> tempColliders = m_TempColliders;
						item.GetComponents(tempColliders);
						if (tempColliders.Count == 0)
						{
							continue;
						}
						float num = float.PositiveInfinity;
						foreach (Collider item2 in tempColliders)
						{
							if (item2.enabled)
							{
								Vector3 vector2 = item2.ClosestPoint(vector);
								float sqrMagnitude = ((vector2 - vector) / 2f).sqrMagnitude;
								if (sqrMagnitude < num)
								{
									num = sqrMagnitude;
								}
							}
						}
						tempColliders.Clear();
						float num2 = item.blendDistance * item.blendDistance;
						if (!(num > num2))
						{
							float num3 = 1f;
							if (num2 > 0f)
							{
								num3 = 1f - num / num2;
							}
							postProcessLayer.OverrideSettings(settings, num3 * item.weight);
						}
					}
				}
			}
		}

		private static void SortByPriority(List<PostProcessVolume> volumes)
		{
			for (int i = 1; i < volumes.Count; i++)
			{
				PostProcessVolume postProcessVolume = volumes[i];
				int num = i - 1;
				while (num >= 0 && volumes[num].priority > postProcessVolume.priority)
				{
					volumes[num + 1] = volumes[num];
					num--;
				}
				volumes[num + 1] = postProcessVolume;
			}
		}
	}
}
