// Decompiled with JetBrains decompiler
// Type: Valve.VR.Extras.SteamVR_LaserPointer
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.Extras
{
  public class SteamVR_LaserPointer : MonoBehaviour
  {
    public SteamVR_Behaviour_Pose pose;
    public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");
    public bool active = true;
    public Color color;
    public float thickness = 1f / 500f;
    public Color clickColor = Color.green;
    public GameObject holder;
    public GameObject pointer;
    private bool isActive;
    public bool addRigidBody;
    public Transform reference;
    private Transform previousContact;

    public event PointerEventHandler PointerIn;

    public event PointerEventHandler PointerOut;

    public event PointerEventHandler PointerClick;

    private void Start()
    {
      if ((Object) this.pose == (Object) null)
        this.pose = this.GetComponent<SteamVR_Behaviour_Pose>();
      if ((Object) this.pose == (Object) null)
        Debug.LogError((object) "No SteamVR_Behaviour_Pose component found on this object");
      if ((SteamVR_Action) this.interactWithUI == (SteamVR_Action) null)
        Debug.LogError((object) "No ui interaction action has been set on this component.");
      this.holder = new GameObject();
      this.holder.transform.parent = this.transform;
      this.holder.transform.localPosition = Vector3.zero;
      this.holder.transform.localRotation = Quaternion.identity;
      this.pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
      this.pointer.transform.parent = this.holder.transform;
      this.pointer.transform.localScale = new Vector3(this.thickness, this.thickness, 100f);
      this.pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50f);
      this.pointer.transform.localRotation = Quaternion.identity;
      BoxCollider component = this.pointer.GetComponent<BoxCollider>();
      if (this.addRigidBody)
      {
        if ((bool) (Object) component)
          component.isTrigger = true;
        this.pointer.AddComponent<Rigidbody>().isKinematic = true;
      }
      else if ((bool) (Object) component)
        Object.Destroy((Object) component);
      Material material = new Material(Shader.Find("Unlit/Color"));
      material.SetColor("_Color", this.color);
      this.pointer.GetComponent<MeshRenderer>().material = material;
    }

    public virtual void OnPointerIn(PointerEventArgs e)
    {
      if (this.PointerIn == null)
        return;
      this.PointerIn((object) this, e);
    }

    public virtual void OnPointerClick(PointerEventArgs e)
    {
      if (this.PointerClick == null)
        return;
      this.PointerClick((object) this, e);
    }

    public virtual void OnPointerOut(PointerEventArgs e)
    {
      if (this.PointerOut == null)
        return;
      this.PointerOut((object) this, e);
    }

    private void Update()
    {
      if (!this.isActive)
      {
        this.isActive = true;
        this.transform.GetChild(0).gameObject.SetActive(true);
      }
      float z = 100f;
      RaycastHit hitInfo;
      bool flag = Physics.Raycast(new Ray(this.transform.position, this.transform.forward), out hitInfo);
      if ((bool) (Object) this.previousContact && (Object) this.previousContact != (Object) hitInfo.transform)
      {
        this.OnPointerOut(new PointerEventArgs()
        {
          fromInputSource = this.pose.inputSource,
          distance = 0.0f,
          flags = 0U,
          target = this.previousContact
        });
        this.previousContact = (Transform) null;
      }
      if (flag && (Object) this.previousContact != (Object) hitInfo.transform)
      {
        this.OnPointerIn(new PointerEventArgs()
        {
          fromInputSource = this.pose.inputSource,
          distance = hitInfo.distance,
          flags = 0U,
          target = hitInfo.transform
        });
        this.previousContact = hitInfo.transform;
      }
      if (!flag)
        this.previousContact = (Transform) null;
      if (flag && (double) hitInfo.distance < 100.0)
        z = hitInfo.distance;
      if (flag && this.interactWithUI.GetStateUp(this.pose.inputSource))
        this.OnPointerClick(new PointerEventArgs()
        {
          fromInputSource = this.pose.inputSource,
          distance = hitInfo.distance,
          flags = 0U,
          target = hitInfo.transform
        });
      if ((SteamVR_Action) this.interactWithUI != (SteamVR_Action) null && this.interactWithUI.GetState(this.pose.inputSource))
      {
        this.pointer.transform.localScale = new Vector3(this.thickness * 5f, this.thickness * 5f, z);
        this.pointer.GetComponent<MeshRenderer>().material.color = this.clickColor;
      }
      else
      {
        this.pointer.transform.localScale = new Vector3(this.thickness, this.thickness, z);
        this.pointer.GetComponent<MeshRenderer>().material.color = this.color;
      }
      this.pointer.transform.localPosition = new Vector3(0.0f, 0.0f, z / 2f);
    }
  }
}
