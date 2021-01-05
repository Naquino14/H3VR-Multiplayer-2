// Decompiled with JetBrains decompiler
// Type: FistVR.GamePlannerManipulator
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class GamePlannerManipulator : MonoBehaviour
  {
    public GamePlannerPanel Panel;
    public Transform ControlledObject;
    public LayerMask LM_CastMask;
    private RaycastHit m_hit;
    public List<GameObject> ManipModeButtonGroups = new List<GameObject>();

    public void SetControlledObject(Transform t) => this.ControlledObject = t;

    public void AxisPress(string s)
    {
      if (s == null)
        return;
      // ISSUE: reference to a compiler-generated field
      if (GamePlannerManipulator.\u003C\u003Ef__switch\u0024map0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        GamePlannerManipulator.\u003C\u003Ef__switch\u0024map0 = new Dictionary<string, int>(14)
        {
          {
            "Nudge_XPlus",
            0
          },
          {
            "Nudge_XMinus",
            1
          },
          {
            "Nudge_YPlus",
            2
          },
          {
            "Nudge_YMinus",
            3
          },
          {
            "Nudge_ZPlus",
            4
          },
          {
            "Nudge_ZMinus",
            5
          },
          {
            "Cast_XPlus",
            6
          },
          {
            "Cast_XMinus",
            7
          },
          {
            "Cast_YPlus",
            8
          },
          {
            "Cast_YMinus",
            9
          },
          {
            "Cast_ZPlus",
            10
          },
          {
            "Cast_ZMinus",
            11
          },
          {
            "Rotate_YPlus",
            12
          },
          {
            "Rotate_YMinus",
            13
          }
        };
      }
      int num;
      // ISSUE: reference to a compiler-generated field
      if (!GamePlannerManipulator.\u003C\u003Ef__switch\u0024map0.TryGetValue(s, out num))
        return;
      switch (num)
      {
        case 0:
          this.MoveControlledObject(Vector3.right);
          break;
        case 1:
          this.MoveControlledObject(Vector3.left);
          break;
        case 2:
          this.MoveControlledObject(Vector3.up);
          break;
        case 3:
          this.MoveControlledObject(Vector3.down);
          break;
        case 4:
          this.MoveControlledObject(Vector3.forward);
          break;
        case 5:
          this.MoveControlledObject(Vector3.back);
          break;
        case 6:
          this.ShuntControlledObject(Vector3.right);
          break;
        case 7:
          this.ShuntControlledObject(Vector3.left);
          break;
        case 8:
          this.ShuntControlledObject(Vector3.up);
          break;
        case 9:
          this.ShuntControlledObject(Vector3.down);
          break;
        case 10:
          this.ShuntControlledObject(Vector3.forward);
          break;
        case 11:
          this.ShuntControlledObject(Vector3.back);
          break;
        case 12:
          this.RotateControlledObject(Vector3.up, true);
          break;
        case 13:
          this.RotateControlledObject(Vector3.down, true);
          break;
      }
    }

    private void MoveControlledObject(Vector3 dir)
    {
      if (this.Panel.ManipAxis_Nudge != GamePlannerPanel.ManipulatorAxis.World)
        dir = this.ControlledObject.right * dir.x + this.ControlledObject.up * dir.y + this.ControlledObject.forward * dir.z;
      this.ControlledObject.position += dir * this.Panel.GetManipNudgeInterval();
      this.transform.position = this.ControlledObject.position;
    }

    private void RotateControlledObject(Vector3 dir, bool isWorld) => this.ControlledObject.Rotate(dir * this.Panel.GetManipRotateInterval());

    public void ResetRotation()
    {
      if (!((Object) this.ControlledObject != (Object) null))
        return;
      this.ControlledObject.rotation = Quaternion.identity;
    }

    private void ShuntControlledObject(Vector3 dir)
    {
      Bounds bounds = new Bounds();
      Collider component1 = this.ControlledObject.GetComponent<Collider>();
      bool flag = false;
      if ((Object) component1 != (Object) null)
      {
        bounds = new Bounds(component1.bounds.center, component1.bounds.size);
        Debug.Log((object) ("base" + (object) bounds));
        flag = true;
      }
      foreach (Component component2 in this.ControlledObject)
      {
        Collider component3 = component2.GetComponent<Collider>();
        if (flag)
        {
          if ((Object) component3 != (Object) null)
          {
            bounds.Encapsulate(component3.bounds);
            Debug.Log((object) bounds);
          }
        }
        else
        {
          bounds = new Bounds(component1.bounds.center, component1.bounds.size);
          Debug.Log((object) ("base" + (object) bounds));
          flag = true;
        }
      }
      float manipShuntInterval = this.Panel.GetManipShuntInterval();
      if (this.Panel.ManipAxis_Nudge != GamePlannerPanel.ManipulatorAxis.World)
        dir = this.ControlledObject.right * dir.x + this.ControlledObject.up * dir.y + this.ControlledObject.forward * dir.z;
      Vector3 vector3 = manipShuntInterval * dir;
      if (Physics.BoxCast(bounds.center, bounds.extents * 0.5f, dir, out this.m_hit, Quaternion.identity, manipShuntInterval, (int) this.LM_CastMask, QueryTriggerInteraction.Ignore))
      {
        this.ControlledObject.position += this.m_hit.distance * dir;
        this.transform.position = this.ControlledObject.position;
      }
      else
      {
        this.ControlledObject.position += vector3;
        this.transform.position = this.ControlledObject.position;
      }
    }

    private void Start()
    {
    }

    public void UpdateManipulatorFrame(GamePlannerPanel.ManipulatorMode mode)
    {
      for (int index = 0; index < this.ManipModeButtonGroups.Count; ++index)
      {
        if (mode == (GamePlannerPanel.ManipulatorMode) index)
          this.ManipModeButtonGroups[index].SetActive(true);
        else
          this.ManipModeButtonGroups[index].SetActive(false);
      }
      switch (this.Panel.ManipMode)
      {
        case GamePlannerPanel.ManipulatorMode.Nudge:
          if (this.Panel.ManipAxis_Nudge == GamePlannerPanel.ManipulatorAxis.World)
          {
            this.transform.rotation = Quaternion.identity;
            break;
          }
          this.transform.rotation = this.ControlledObject.rotation;
          break;
        case GamePlannerPanel.ManipulatorMode.Shunt:
          if (this.Panel.ManipAxis_Shunt == GamePlannerPanel.ManipulatorAxis.World)
          {
            this.transform.rotation = Quaternion.identity;
            break;
          }
          this.transform.rotation = this.ControlledObject.rotation;
          break;
        case GamePlannerPanel.ManipulatorMode.Rotate:
          if (this.Panel.ManipAxis_Rotate == GamePlannerPanel.ManipulatorAxis.World)
          {
            this.transform.rotation = Quaternion.identity;
            break;
          }
          this.transform.rotation = this.ControlledObject.rotation;
          break;
      }
    }

    private void Update()
    {
      if (!((Object) this.ControlledObject != (Object) null))
        ;
    }
  }
}
