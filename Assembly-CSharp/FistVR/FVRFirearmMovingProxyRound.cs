// Decompiled with JetBrains decompiler
// Type: FistVR.FVRFirearmMovingProxyRound
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class FVRFirearmMovingProxyRound : MonoBehaviour
  {
    public bool IsFull;
    public bool IsSpent;
    public FVRFireArmRound Round;
    public Transform ProxyRound;
    public MeshFilter ProxyMesh;
    public MeshRenderer ProxyRenderer;

    public void Init(Transform t)
    {
      this.ProxyRound = this.transform;
      this.ProxyRound.SetParent(t);
      this.ProxyRound.localPosition = Vector3.zero;
      this.ProxyRound.localEulerAngles = Vector3.zero;
      this.ProxyMesh = this.gameObject.AddComponent<MeshFilter>();
      this.ProxyRenderer = this.gameObject.AddComponent<MeshRenderer>();
    }

    public void UpdateProxyDisplay()
    {
      if ((Object) this.Round == (Object) null)
      {
        this.ProxyMesh.mesh = (Mesh) null;
        this.ProxyRenderer.material = (Material) null;
        this.ProxyRenderer.enabled = false;
      }
      else
      {
        if (this.IsSpent)
        {
          this.ProxyMesh.mesh = this.Round.FiredRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh;
          this.ProxyRenderer.material = this.Round.FiredRenderer.sharedMaterial;
        }
        else
        {
          this.ProxyMesh.mesh = AM.GetRoundMesh(this.Round.RoundType, this.Round.RoundClass);
          this.ProxyRenderer.material = AM.GetRoundMaterial(this.Round.RoundType, this.Round.RoundClass);
        }
        this.ProxyRenderer.enabled = true;
      }
    }

    public void ClearProxy()
    {
      this.Round = (FVRFireArmRound) null;
      this.IsFull = false;
      this.IsSpent = true;
      this.UpdateProxyDisplay();
    }

    public void SetFromPrefabReference(GameObject go)
    {
      this.Round = go.GetComponent<FVRFireArmRound>();
      this.IsFull = true;
      this.IsSpent = false;
      this.UpdateProxyDisplay();
    }
  }
}
