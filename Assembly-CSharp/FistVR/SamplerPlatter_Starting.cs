// Decompiled with JetBrains decompiler
// Type: FistVR.SamplerPlatter_Starting
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace FistVR
{
  public class SamplerPlatter_Starting : MonoBehaviour
  {
    public Renderer rend;
    public Texture2D[] Textures;
    public Texture2D[] Textures_Oculus;
    public Texture2D[] Textures_OculusStreamlined;
    public Texture2D[] Textures_Index;
    public Texture2D[] Textures_IndexStreamlined;
    private int m_index;
    public AudioEvent ScreenEvent;
    private ControlMode CMode;
    private bool m_isStreamlined;
    private bool m_hasInit;

    private void Start() => this.UpdateImage();

    private void Update()
    {
      for (int index = 0; index < GM.CurrentMovementManager.Hands.Length; ++index)
      {
        if (GM.CurrentMovementManager.Hands[index].HasInit)
        {
          this.CMode = GM.CurrentMovementManager.Hands[index].CMode;
          this.m_isStreamlined = GM.CurrentMovementManager.Hands[index].IsInStreamlinedMode;
          this.UpdateImage();
          this.m_hasInit = true;
        }
      }
    }

    public void Next()
    {
      SM.PlayGenericSound(this.ScreenEvent, this.transform.position);
      ++this.m_index;
      if (this.m_index >= this.Textures.Length)
        this.m_index = 0;
      this.UpdateImage();
    }

    public void Previous()
    {
      SM.PlayGenericSound(this.ScreenEvent, this.transform.position);
      --this.m_index;
      if (this.m_index < 0)
        this.m_index = this.Textures.Length - 1;
      this.UpdateImage();
    }

    private void UpdateImage()
    {
      switch (this.CMode)
      {
        case ControlMode.Vive:
          this.rend.material.SetTexture("_MainTex", (Texture) this.Textures[this.m_index]);
          this.rend.material.SetTexture("_IncandescenceMap", (Texture) this.Textures[this.m_index]);
          break;
        case ControlMode.Oculus:
          if (this.m_isStreamlined && this.Textures_OculusStreamlined.Length > this.m_index && (Object) this.Textures_OculusStreamlined[this.m_index] != (Object) null)
          {
            this.rend.material.SetTexture("_MainTex", (Texture) this.Textures_OculusStreamlined[this.m_index]);
            this.rend.material.SetTexture("_IncandescenceMap", (Texture) this.Textures_OculusStreamlined[this.m_index]);
            break;
          }
          this.rend.material.SetTexture("_MainTex", (Texture) this.Textures_Oculus[this.m_index]);
          this.rend.material.SetTexture("_IncandescenceMap", (Texture) this.Textures_Oculus[this.m_index]);
          break;
        case ControlMode.WMR:
          this.rend.material.SetTexture("_MainTex", (Texture) this.Textures[this.m_index]);
          this.rend.material.SetTexture("_IncandescenceMap", (Texture) this.Textures[this.m_index]);
          break;
        case ControlMode.Index:
          if (this.m_isStreamlined && this.Textures_IndexStreamlined.Length > this.m_index && (Object) this.Textures_IndexStreamlined[this.m_index] != (Object) null)
          {
            this.rend.material.SetTexture("_MainTex", (Texture) this.Textures_IndexStreamlined[this.m_index]);
            this.rend.material.SetTexture("_IncandescenceMap", (Texture) this.Textures_IndexStreamlined[this.m_index]);
            break;
          }
          this.rend.material.SetTexture("_MainTex", (Texture) this.Textures_Index[this.m_index]);
          this.rend.material.SetTexture("_IncandescenceMap", (Texture) this.Textures_Index[this.m_index]);
          break;
      }
    }
  }
}
