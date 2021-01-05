// Decompiled with JetBrains decompiler
// Type: FistVR.TNH_HoldPointSystemNode
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class TNH_HoldPointSystemNode : MonoBehaviour
  {
    public TNH_HoldPoint HoldPoint;
    public Transform NodeCenter;
    public float ActivationRange = 1f;
    private bool m_hasActivated;
    private bool m_hasInitiatedHold;
    public Text Display;
    private TNH_HoldPointSystemNode.SystemNodeMode m_mode;
    private float m_timeTilTargRotationChange = 1f;
    private float m_changeSpeed = 0.2f;
    private float m_rotateSpeed = 20f;
    private float m_curNodeHeight = 1.5f;
    private float m_tarNodeHeight = 1.5f;
    private float m_curTextHeight = 2f;
    private float m_tarTextHeight = 2.4f;
    private Quaternion m_targRotation;
    public Renderer NodeRenderer;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color Color_Passive;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color Color_Hacking;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color Color_Analyzing;
    [ColorUsage(true, true, 0.0f, 8f, 0.125f, 3f)]
    public Color Color_Indentified;
    public Transform DisplayTrans;
    private float m_activateAmount;
    public AudioEvent AUDEvent_HoldActivate;
    public GameObject VFX_Activate;
    public GameObject VFX_HoldActivate;

    private void Start() => this.m_targRotation = this.NodeCenter.rotation;

    public void SetDisplayString(string s) => this.Display.text = s;

    public void SetNodeMode(TNH_HoldPointSystemNode.SystemNodeMode m)
    {
      this.m_mode = m;
      switch (this.m_mode)
      {
        case TNH_HoldPointSystemNode.SystemNodeMode.Passive:
          this.NodeRenderer.material.SetColor("_RimColor", this.Color_Passive);
          break;
        case TNH_HoldPointSystemNode.SystemNodeMode.Hacking:
          this.NodeRenderer.material.SetColor("_RimColor", this.Color_Hacking);
          break;
        case TNH_HoldPointSystemNode.SystemNodeMode.Analyzing:
          this.NodeRenderer.material.SetColor("_RimColor", this.Color_Analyzing);
          break;
        case TNH_HoldPointSystemNode.SystemNodeMode.Indentified:
          this.NodeRenderer.material.SetColor("_RimColor", this.Color_Indentified);
          break;
      }
    }

    private void Update()
    {
      Vector3 forward = GM.CurrentPlayerBody.Head.position - this.DisplayTrans.position;
      forward.y = 0.0f;
      this.DisplayTrans.rotation = Quaternion.LookRotation(forward, Vector3.up);
      if (!this.m_hasActivated)
      {
        float num1 = Vector3.Distance(this.NodeCenter.position, GM.CurrentPlayerBody.LeftHand.transform.position);
        float num2 = Vector3.Distance(this.NodeCenter.position, GM.CurrentPlayerBody.RightHand.transform.position);
        if ((double) num1 < (double) this.ActivationRange || (double) num2 < (double) this.ActivationRange)
        {
          this.m_hasActivated = true;
          Object.Instantiate<GameObject>(this.VFX_Activate, this.NodeCenter.position, this.NodeCenter.rotation);
          SM.PlayCoreSound(FVRPooledAudioType.GenericLongRange, this.AUDEvent_HoldActivate, this.transform.position);
        }
      }
      if (this.m_hasActivated && !this.m_hasInitiatedHold)
      {
        this.m_activateAmount += Time.deltaTime;
        if ((double) this.m_activateAmount > 0.5)
        {
          this.m_hasInitiatedHold = true;
          Object.Instantiate<GameObject>(this.VFX_HoldActivate, this.NodeCenter.position, this.NodeCenter.rotation);
          this.HoldPoint.BeginHoldChallenge();
        }
      }
      switch (this.m_mode)
      {
        case TNH_HoldPointSystemNode.SystemNodeMode.Passive:
          this.m_changeSpeed = 2f;
          this.m_rotateSpeed = 20f;
          this.m_tarNodeHeight = 1.5f;
          this.m_tarTextHeight = 2f;
          break;
        case TNH_HoldPointSystemNode.SystemNodeMode.Hacking:
          this.m_changeSpeed = 2f;
          this.m_rotateSpeed = 50f;
          this.m_tarNodeHeight = 1.5f;
          this.m_tarTextHeight = 2f;
          break;
        case TNH_HoldPointSystemNode.SystemNodeMode.Analyzing:
          this.m_changeSpeed = 5f;
          this.m_rotateSpeed = 500f;
          this.m_tarNodeHeight = 2.5f;
          this.m_tarTextHeight = 2.4f;
          break;
        case TNH_HoldPointSystemNode.SystemNodeMode.Indentified:
          this.m_changeSpeed = 5f;
          this.m_rotateSpeed = 500f;
          this.m_tarNodeHeight = 2.5f;
          this.m_tarTextHeight = 2.4f;
          break;
      }
      this.m_timeTilTargRotationChange -= Time.deltaTime * this.m_changeSpeed;
      if ((double) this.m_timeTilTargRotationChange <= 0.0)
      {
        this.PickNewTarg();
        this.m_timeTilTargRotationChange = 1f;
      }
      this.m_curNodeHeight = Mathf.Lerp(this.m_curNodeHeight, this.m_tarNodeHeight, Time.deltaTime * 4f);
      this.NodeCenter.localPosition = new Vector3(0.0f, this.m_curNodeHeight, 0.0f);
      this.m_curTextHeight = Mathf.Lerp(this.m_curTextHeight, this.m_tarTextHeight, Time.deltaTime * 4f);
      this.DisplayTrans.localPosition = new Vector3(0.0f, this.m_curTextHeight, 0.0f);
      this.NodeCenter.rotation = Quaternion.RotateTowards(this.NodeCenter.rotation, this.m_targRotation, this.m_rotateSpeed * Time.deltaTime);
    }

    private void PickNewTarg()
    {
      switch (this.m_mode)
      {
        case TNH_HoldPointSystemNode.SystemNodeMode.Passive:
          this.m_targRotation = Quaternion.Slerp(this.NodeCenter.rotation, Random.rotation, 0.3f);
          break;
        case TNH_HoldPointSystemNode.SystemNodeMode.Hacking:
          this.m_targRotation = Quaternion.Slerp(this.NodeCenter.rotation, Random.rotation, 0.4f);
          break;
        case TNH_HoldPointSystemNode.SystemNodeMode.Analyzing:
          this.m_targRotation = Quaternion.Slerp(this.NodeCenter.rotation, Random.rotation, 0.7f);
          break;
        case TNH_HoldPointSystemNode.SystemNodeMode.Indentified:
          this.m_targRotation = Quaternion.Slerp(this.NodeCenter.rotation, Random.rotation, 0.7f);
          break;
      }
    }

    public enum SystemNodeMode
    {
      Passive,
      Hacking,
      Analyzing,
      Indentified,
    }
  }
}
