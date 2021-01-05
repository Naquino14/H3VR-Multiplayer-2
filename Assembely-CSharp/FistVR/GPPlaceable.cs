using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
	public class GPPlaceable : MonoBehaviour
	{
		public string ObjectID;

		public int UniqueID;

		public Dictionary<string, string> Flags = new Dictionary<string, string>();

		public List<string> InternalList = new List<string>();

		[Header("Extras")]
		public GameObject ParamsPanel;

		[Header("Guides")]
		public Bounds Extents = default(Bounds);

		public bool HasGuide;

		public Transform Guide;

		public Vector3 GuideBias;

		[Header("Display")]
		public GameObject Display_DesignOnly;

		public virtual void Setup(string objectID, int uniqueID)
		{
			ObjectID = objectID;
			UniqueID = uniqueID;
		}

		public virtual void Init(GPSceneMode mode)
		{
			if (mode == GPSceneMode.Design)
			{
				if (Display_DesignOnly != null)
				{
					Display_DesignOnly.SetActive(value: true);
				}
			}
			else if (Display_DesignOnly != null)
			{
				Display_DesignOnly.SetActive(value: false);
			}
		}

		public bool FlagExists(string flag)
		{
			return Flags.ContainsKey(flag);
		}

		public virtual void SetFlag(string flag, string value)
		{
			if (Flags.ContainsKey(flag))
			{
				Flags[flag] = value;
			}
			else
			{
				Flags.Add(flag, value);
			}
		}

		public virtual void AddToInternalList(string flag, bool exclusive)
		{
			if (!exclusive || !InternalList.Contains(flag))
			{
				InternalList.Add(flag);
			}
		}

		public virtual void RemoveFromInternalList(string flag)
		{
			InternalList.Remove(flag);
		}

		public virtual void Reset()
		{
			Flags.Clear();
			InternalList.Clear();
		}

		public void SetParamsPanel(bool b)
		{
			if (ParamsPanel != null)
			{
				ParamsPanel.SetActive(b);
				UpdateParamsDisplay();
			}
		}

		public virtual void UpdateParamsDisplay()
		{
		}

		public virtual GPSavedPlaceable GetSavedForm()
		{
			GPSavedPlaceable gPSavedPlaceable = new GPSavedPlaceable();
			gPSavedPlaceable.ObjectID = ObjectID;
			gPSavedPlaceable.UniqueID = UniqueID;
			gPSavedPlaceable.Position = base.transform.position;
			gPSavedPlaceable.Rotation = base.transform.rotation;
			gPSavedPlaceable.Flags = new SerializableStringDictionary(Flags);
			gPSavedPlaceable.InternalList = new List<string>(InternalList);
			return gPSavedPlaceable;
		}

		public virtual void ConfigureFromSavedPlaceable(GPSavedPlaceable p)
		{
			ObjectID = p.ObjectID;
			UniqueID = p.UniqueID;
			base.transform.position = p.Position;
			base.transform.rotation = p.Rotation;
			Flags.Clear();
			for (int i = 0; i < p.Flags._keys.Length; i++)
			{
				Flags.Add(p.Flags._keys[i], p.Flags.dictionary[p.Flags._keys[i]]);
			}
			InternalList.Clear();
			for (int j = 0; j < p.InternalList.Count; j++)
			{
				InternalList.Add(p.InternalList[j]);
			}
		}

		[ContextMenu("GenerateBounds")]
		public void GenerateBounds()
		{
			Vector3 position = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			Extents = default(Bounds);
			base.transform.position = Vector3.zero;
			base.transform.rotation = Quaternion.identity;
			Collider component = base.transform.GetComponent<Collider>();
			if (component != null)
			{
				Extents.Encapsulate(component.bounds);
			}
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					Collider component2 = transform.GetComponent<Collider>();
					if (component2 != null)
					{
						Extents.Encapsulate(component2.bounds);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = enumerator as IDisposable) != null)
				{
					disposable.Dispose();
				}
			}
			base.transform.position = position;
			base.transform.rotation = rotation;
		}
	}
}
