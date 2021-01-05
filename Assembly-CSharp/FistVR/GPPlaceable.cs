// Decompiled with JetBrains decompiler
// Type: FistVR.GPPlaceable
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

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
    public Bounds Extents = new Bounds();
    public bool HasGuide;
    public Transform Guide;
    public Vector3 GuideBias;
    [Header("Display")]
    public GameObject Display_DesignOnly;

    public virtual void Setup(string objectID, int uniqueID)
    {
      this.ObjectID = objectID;
      this.UniqueID = uniqueID;
    }

    public virtual void Init(GPSceneMode mode)
    {
      if (mode == GPSceneMode.Design)
      {
        if (!((Object) this.Display_DesignOnly != (Object) null))
          return;
        this.Display_DesignOnly.SetActive(true);
      }
      else
      {
        if (!((Object) this.Display_DesignOnly != (Object) null))
          return;
        this.Display_DesignOnly.SetActive(false);
      }
    }

    public bool FlagExists(string flag) => this.Flags.ContainsKey(flag);

    public virtual void SetFlag(string flag, string value)
    {
      if (this.Flags.ContainsKey(flag))
        this.Flags[flag] = value;
      else
        this.Flags.Add(flag, value);
    }

    public virtual void AddToInternalList(string flag, bool exclusive)
    {
      if (exclusive && this.InternalList.Contains(flag))
        return;
      this.InternalList.Add(flag);
    }

    public virtual void RemoveFromInternalList(string flag) => this.InternalList.Remove(flag);

    public virtual void Reset()
    {
      this.Flags.Clear();
      this.InternalList.Clear();
    }

    public void SetParamsPanel(bool b)
    {
      if (!((Object) this.ParamsPanel != (Object) null))
        return;
      this.ParamsPanel.SetActive(b);
      this.UpdateParamsDisplay();
    }

    public virtual void UpdateParamsDisplay()
    {
    }

    public virtual GPSavedPlaceable GetSavedForm() => new GPSavedPlaceable()
    {
      ObjectID = this.ObjectID,
      UniqueID = this.UniqueID,
      Position = this.transform.position,
      Rotation = this.transform.rotation,
      Flags = new SerializableStringDictionary(this.Flags),
      InternalList = new List<string>((IEnumerable<string>) this.InternalList)
    };

    public virtual void ConfigureFromSavedPlaceable(GPSavedPlaceable p)
    {
      this.ObjectID = p.ObjectID;
      this.UniqueID = p.UniqueID;
      this.transform.position = p.Position;
      this.transform.rotation = p.Rotation;
      this.Flags.Clear();
      for (int index = 0; index < p.Flags._keys.Length; ++index)
        this.Flags.Add(p.Flags._keys[index], p.Flags.dictionary[p.Flags._keys[index]]);
      this.InternalList.Clear();
      for (int index = 0; index < p.InternalList.Count; ++index)
        this.InternalList.Add(p.InternalList[index]);
    }

    [ContextMenu("GenerateBounds")]
    public void GenerateBounds()
    {
      Vector3 position = this.transform.position;
      Quaternion rotation = this.transform.rotation;
      this.Extents = new Bounds();
      this.transform.position = Vector3.zero;
      this.transform.rotation = Quaternion.identity;
      Collider component1 = this.transform.GetComponent<Collider>();
      if ((Object) component1 != (Object) null)
        this.Extents.Encapsulate(component1.bounds);
      foreach (Component component2 in this.transform)
      {
        Collider component3 = component2.GetComponent<Collider>();
        if ((Object) component3 != (Object) null)
          this.Extents.Encapsulate(component3.bounds);
      }
      this.transform.position = position;
      this.transform.rotation = rotation;
    }
  }
}
