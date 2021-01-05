// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Camera
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.VR;

namespace Valve.VR
{
  [RequireComponent(typeof (Camera))]
  public class SteamVR_Camera : MonoBehaviour
  {
    [SerializeField]
    private Transform _head;
    [SerializeField]
    private Transform _ears;
    public bool wireframe;
    private static Hashtable values;
    private const string eyeSuffix = " (eye)";
    private const string earsSuffix = " (ears)";
    private const string headSuffix = " (head)";
    private const string originSuffix = " (origin)";

    public Transform head => this._head;

    public Transform offset => this._head;

    public Transform origin => this._head.parent;

    public Camera camera { get; private set; }

    public Transform ears => this._ears;

    public Ray GetRay() => new Ray(this._head.position, this._head.forward);

    public static float sceneResolutionScale
    {
      get => VRSettings.renderScale;
      set
      {
        if ((double) value == 0.0)
          return;
        VRSettings.renderScale = value;
      }
    }

    private void OnDisable() => SteamVR_Render.Remove(this);

    private void OnEnable()
    {
      if (SteamVR.instance == null)
      {
        if ((UnityEngine.Object) this.head != (UnityEngine.Object) null)
          this.head.GetComponent<SteamVR_TrackedObject>().enabled = false;
        this.enabled = false;
      }
      else
      {
        Transform transform = this.transform;
        if ((UnityEngine.Object) this.head != (UnityEngine.Object) transform)
        {
          this.Expand();
          transform.parent = this.origin;
          while (this.head.childCount > 0)
            this.head.GetChild(0).parent = transform;
          this.head.parent = transform;
          this.head.localPosition = Vector3.zero;
          this.head.localRotation = Quaternion.identity;
          this.head.localScale = Vector3.one;
          this.head.gameObject.SetActive(false);
          this._head = transform;
        }
        if ((UnityEngine.Object) this.ears == (UnityEngine.Object) null)
        {
          SteamVR_Ears componentInChildren = this.transform.GetComponentInChildren<SteamVR_Ears>();
          if ((UnityEngine.Object) componentInChildren != (UnityEngine.Object) null)
            this._ears = componentInChildren.transform;
        }
        if ((UnityEngine.Object) this.ears != (UnityEngine.Object) null)
          this.ears.GetComponent<SteamVR_Ears>().vrcam = this;
        SteamVR_Render.Add(this);
      }
    }

    private void Awake()
    {
      this.camera = this.GetComponent<Camera>();
      this.ForceLast();
    }

    public void ForceLast()
    {
      if (SteamVR_Camera.values != null)
      {
        foreach (DictionaryEntry dictionaryEntry in SteamVR_Camera.values)
          (dictionaryEntry.Key as FieldInfo).SetValue((object) this, dictionaryEntry.Value);
        SteamVR_Camera.values = (Hashtable) null;
      }
      else
      {
        foreach (Component component in this.GetComponents<Component>())
        {
          SteamVR_Camera steamVrCamera = component as SteamVR_Camera;
          if ((UnityEngine.Object) steamVrCamera != (UnityEngine.Object) null && (UnityEngine.Object) steamVrCamera != (UnityEngine.Object) this)
            UnityEngine.Object.DestroyImmediate((UnityEngine.Object) steamVrCamera);
        }
        Component[] components = this.GetComponents<Component>();
        if (!((UnityEngine.Object) this != (UnityEngine.Object) components[components.Length - 1]))
          return;
        SteamVR_Camera.values = new Hashtable();
        foreach (FieldInfo field in this.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
          if (field.IsPublic || field.IsDefined(typeof (SerializeField), true))
            SteamVR_Camera.values[(object) field] = field.GetValue((object) this);
        }
        GameObject gameObject = this.gameObject;
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this);
        gameObject.AddComponent<SteamVR_Camera>().ForceLast();
      }
    }

    public string baseName => this.name.EndsWith(" (eye)") ? this.name.Substring(0, this.name.Length - " (eye)".Length) : this.name;

    public void Expand()
    {
      Transform transform = this.transform.parent;
      if ((UnityEngine.Object) transform == (UnityEngine.Object) null)
      {
        transform = new GameObject(this.name + " (origin)").transform;
        transform.localPosition = this.transform.localPosition;
        transform.localRotation = this.transform.localRotation;
        transform.localScale = this.transform.localScale;
      }
      if ((UnityEngine.Object) this.head == (UnityEngine.Object) null)
      {
        this._head = new GameObject(this.name + " (head)", new System.Type[1]
        {
          typeof (SteamVR_TrackedObject)
        }).transform;
        this.head.parent = transform;
        this.head.position = this.transform.position;
        this.head.rotation = this.transform.rotation;
        this.head.localScale = Vector3.one;
        this.head.tag = this.tag;
      }
      if ((UnityEngine.Object) this.transform.parent != (UnityEngine.Object) this.head)
      {
        this.transform.parent = this.head;
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
        this.transform.localScale = Vector3.one;
        while (this.transform.childCount > 0)
          this.transform.GetChild(0).parent = this.head;
        GUILayer component1 = this.GetComponent<GUILayer>();
        if ((UnityEngine.Object) component1 != (UnityEngine.Object) null)
        {
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component1);
          this.head.gameObject.AddComponent<GUILayer>();
        }
        AudioListener component2 = this.GetComponent<AudioListener>();
        if ((UnityEngine.Object) component2 != (UnityEngine.Object) null)
        {
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component2);
          this._ears = new GameObject(this.name + " (ears)", new System.Type[1]
          {
            typeof (SteamVR_Ears)
          }).transform;
          this.ears.parent = this._head;
          this.ears.localPosition = Vector3.zero;
          this.ears.localRotation = Quaternion.identity;
          this.ears.localScale = Vector3.one;
        }
      }
      if (this.name.EndsWith(" (eye)"))
        return;
      this.name += " (eye)";
    }

    public void Collapse()
    {
      this.transform.parent = (Transform) null;
      while (this.head.childCount > 0)
        this.head.GetChild(0).parent = this.transform;
      GUILayer component = this.head.GetComponent<GUILayer>();
      if ((UnityEngine.Object) component != (UnityEngine.Object) null)
      {
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) component);
        this.gameObject.AddComponent<GUILayer>();
      }
      if ((UnityEngine.Object) this.ears != (UnityEngine.Object) null)
      {
        while (this.ears.childCount > 0)
          this.ears.GetChild(0).parent = this.transform;
        UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.ears.gameObject);
        this._ears = (Transform) null;
        this.gameObject.AddComponent(typeof (AudioListener));
      }
      if ((UnityEngine.Object) this.origin != (UnityEngine.Object) null)
      {
        if (this.origin.name.EndsWith(" (origin)"))
        {
          Transform origin = this.origin;
          while (origin.childCount > 0)
            origin.GetChild(0).parent = origin.parent;
          UnityEngine.Object.DestroyImmediate((UnityEngine.Object) origin.gameObject);
        }
        else
          this.transform.parent = this.origin;
      }
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.head.gameObject);
      this._head = (Transform) null;
      if (!this.name.EndsWith(" (eye)"))
        return;
      this.name = this.name.Substring(0, this.name.Length - " (eye)".Length);
    }
  }
}
