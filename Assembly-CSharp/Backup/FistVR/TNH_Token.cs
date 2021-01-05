// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_Token
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class TNH_Token : MonoBehaviour
  {
    public TNH_Manager M;
    public Transform Bounds;
    public Transform Display;
    private float m_scale = 0.01f;
    private bool canCollect;
    public GameObject PickupEffect;
    private float yRot;
    private bool m_isCollected;

    private void Start()
    {
    }

    private void Update()
    {
      this.Display.localPosition = new Vector3(0.0f, Mathf.Sin(Time.time * 2f) * 0.05f, 0.0f);
      if (this.canCollect)
      {
        Vector3 position1 = GM.CurrentPlayerBody.LeftHand.position;
        Vector3 position2 = GM.CurrentPlayerBody.RightHand.position;
        if (this.IsPointInBounds(position1) || this.IsPointInBounds(position2))
          this.Collect();
      }
      if ((double) this.m_scale < 1.0)
      {
        this.m_scale = Mathf.MoveTowards(this.m_scale, 1f, Time.deltaTime * 3f);
        this.Display.localScale = new Vector3(this.m_scale, this.m_scale, this.m_scale);
        this.Display.localEulerAngles = new Vector3(0.0f, this.m_scale * 720f, 0.0f);
      }
      else
      {
        this.canCollect = true;
        this.yRot += Time.deltaTime * 90f;
        this.yRot = Mathf.Repeat(this.yRot, 360f);
        this.Display.localEulerAngles = new Vector3(0.0f, this.yRot, 0.0f);
      }
    }

    private void Collect()
    {
      if (this.m_isCollected)
        return;
      this.m_isCollected = true;
      this.M.AddTokens(1, true);
      this.M.EnqueueTokenLine(1);
      Object.Instantiate<GameObject>(this.PickupEffect, this.transform.position, this.transform.rotation);
      Object.Destroy((Object) this.gameObject);
    }

    public bool IsPointInBounds(Vector3 p) => this.TestVolumeBool(this.Bounds, p);

    public bool TestVolumeBool(Transform t, Vector3 pos)
    {
      bool flag = true;
      Vector3 vector3 = t.InverseTransformPoint(pos);
      if ((double) Mathf.Abs(vector3.x) > 0.5 || (double) Mathf.Abs(vector3.y) > 0.5 || (double) Mathf.Abs(vector3.z) > 0.5)
        flag = false;
      return flag;
    }
  }
}
