// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.BalloonColliders
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR.InteractionSystem
{
  public class BalloonColliders : MonoBehaviour
  {
    public GameObject[] colliders;
    private Vector3[] colliderLocalPositions;
    private Quaternion[] colliderLocalRotations;
    private Rigidbody rb;

    private void Awake()
    {
      this.rb = this.GetComponent<Rigidbody>();
      this.colliderLocalPositions = new Vector3[this.colliders.Length];
      this.colliderLocalRotations = new Quaternion[this.colliders.Length];
      for (int index = 0; index < this.colliders.Length; ++index)
      {
        this.colliderLocalPositions[index] = this.colliders[index].transform.localPosition;
        this.colliderLocalRotations[index] = this.colliders[index].transform.localRotation;
        this.colliders[index].name = this.gameObject.name + "." + this.colliders[index].name;
      }
    }

    private void OnEnable()
    {
      for (int index = 0; index < this.colliders.Length; ++index)
      {
        this.colliders[index].transform.SetParent(this.transform);
        this.colliders[index].transform.localPosition = this.colliderLocalPositions[index];
        this.colliders[index].transform.localRotation = this.colliderLocalRotations[index];
        this.colliders[index].transform.SetParent((Transform) null);
        FixedJoint fixedJoint = this.colliders[index].AddComponent<FixedJoint>();
        fixedJoint.connectedBody = this.rb;
        fixedJoint.breakForce = float.PositiveInfinity;
        fixedJoint.breakTorque = float.PositiveInfinity;
        fixedJoint.enableCollision = false;
        fixedJoint.enablePreprocessing = true;
        this.colliders[index].SetActive(true);
      }
    }

    private void OnDisable()
    {
      for (int index = 0; index < this.colliders.Length; ++index)
      {
        if ((Object) this.colliders[index] != (Object) null)
        {
          Object.Destroy((Object) this.colliders[index].GetComponent<FixedJoint>());
          this.colliders[index].SetActive(false);
        }
      }
    }

    private void OnDestroy()
    {
      for (int index = 0; index < this.colliders.Length; ++index)
        Object.Destroy((Object) this.colliders[index]);
    }
  }
}
