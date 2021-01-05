// Decompiled with JetBrains decompiler
// Type: ScopeCam
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using FistVR;
using UnityEngine;
using UnityEngine.VR;

public class ScopeCam : MonoBehaviour
{
  public Material PostMaterial;
  public GameObject Reticule;
  public Camera ScopeCamera;
  public float Magnification = 5f;
  public int Resolution = 512;
  public float AngleBlurStrength = 0.5f;
  public float CutoffSoftness = 0.05f;
  public float AngularOccludeSensitivity = 0.5f;
  public float ReticuleScale = 1f;
  public bool MagnificationEnabledAtStart;
  [Range(0.0f, 1f)]
  public float LensSpaceDistortion = 0.075f;
  [Range(0.0f, 5f)]
  public float LensChromaticDistortion = 0.075f;
  private Renderer m_renderer;
  private MaterialPropertyBlock m_block;
  private Vector3 m_reticuleSize = new Vector3(0.1f, 0.1f, 0.1f);
  private RenderTexture m_mainTex;
  private RenderTexture m_blurTex;
  private bool m_magnifcationEnabled;

  public bool MagnificationEnabled
  {
    get => this.m_magnifcationEnabled;
    set
    {
      if (this.m_magnifcationEnabled == value)
        return;
      this.m_magnifcationEnabled = value;
      if (this.m_magnifcationEnabled)
      {
        if ((Object) this.Reticule != (Object) null)
          this.Reticule.SetActive(true);
      }
      else if ((Object) this.Reticule != (Object) null)
        this.Reticule.SetActive(false);
      if (this.m_magnifcationEnabled || !((Object) this.m_mainTex != (Object) null))
        return;
      this.ClearToBlack(this.m_mainTex);
      this.ClearToBlack(this.m_blurTex);
    }
  }

  private void ClearToBlack(RenderTexture tex)
  {
    if (!((Object) tex != (Object) null))
      return;
    RenderTexture.active = tex;
    GL.Clear(false, true, Color.black);
    RenderTexture.active = (RenderTexture) null;
  }

  private void OnEnable()
  {
    RenderTexture renderTexture = new RenderTexture(this.Resolution, this.Resolution, 24, RenderTextureFormat.DefaultHDR, RenderTextureReadWrite.sRGB);
    renderTexture.wrapMode = TextureWrapMode.Clamp;
    this.m_mainTex = renderTexture;
    this.m_blurTex = new RenderTexture(this.Resolution, this.Resolution, 0, this.m_mainTex.format, RenderTextureReadWrite.sRGB);
    this.m_mainTex.Create();
    this.m_blurTex.Create();
    this.ScopeCamera.enabled = false;
    this.ScopeCamera.allowHDR = true;
    this.ScopeCamera.allowMSAA = false;
    this.m_renderer = this.GetComponent<Renderer>();
    this.m_block = new MaterialPropertyBlock();
    if ((Object) this.Reticule != (Object) null)
      this.m_reticuleSize = this.Reticule.transform.localScale;
    this.MagnificationEnabled = this.MagnificationEnabledAtStart;
  }

  private void OnWillRenderObject()
  {
    if ((Object) Camera.current == (Object) this.ScopeCamera || !this.MagnificationEnabled)
      return;
    Vector3 min = this.m_renderer.bounds.min;
    Vector3 max = this.m_renderer.bounds.max;
    Vector3 position1 = this.m_renderer.transform.position - Camera.current.transform.right * this.m_renderer.transform.localScale.x - Camera.current.transform.up * this.m_renderer.transform.localScale.x;
    Vector3 position2 = this.m_renderer.transform.position + Camera.current.transform.right * this.m_renderer.transform.localScale.x + Camera.current.transform.up * this.m_renderer.transform.localScale.x;
    Vector3 viewportPoint1 = Camera.current.WorldToViewportPoint(position1);
    Vector3 viewportPoint2 = Camera.current.WorldToViewportPoint(position2);
    float num1 = Mathf.Abs(Mathf.Clamp01(viewportPoint2.x) - Mathf.Clamp01(viewportPoint1.x));
    float num2 = Mathf.Abs(Mathf.Clamp01(viewportPoint2.y) - Mathf.Clamp01(viewportPoint1.y));
    this.ScopeCamera.fieldOfView = (float) ((double) Camera.current.fieldOfView * (double) Mathf.Sqrt((float) ((double) num1 * (double) num1 + (double) num2 * (double) num2)) / ((double) this.Magnification * 3.14159274101257 * 0.5));
    Vector3 vector3_1;
    if (!Camera.current.stereoEnabled)
    {
      vector3_1 = Camera.current.transform.position;
      this.m_block.SetFloat("_EyeIndex", 0.0f);
    }
    else
    {
      Vector3 position3 = GM.CurrentPlayerBody.Head.position;
      Vector3 vector3_2 = position3 + GM.CurrentPlayerBody.Head.right * -0.022f;
      Vector3 vector3_3 = position3 + GM.CurrentPlayerBody.Head.right * 0.022f;
      Vector3 to1 = vector3_2 - this.transform.position;
      Vector3 to2 = vector3_3 - this.transform.position;
      float num3 = Vector3.Angle(this.transform.forward, to1);
      VRNode node = (double) Vector3.Angle(this.transform.forward, to2) >= (double) num3 ? VRNode.RightEye : VRNode.LeftEye;
      Vector3 v1 = Quaternion.Inverse(InputTracking.GetLocalRotation(node)) * InputTracking.GetLocalPosition(node);
      Matrix4x4 cameraToWorldMatrix = Camera.current.cameraToWorldMatrix;
      Vector3 v2 = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.Head)) * InputTracking.GetLocalPosition(VRNode.Head);
      vector3_1 = cameraToWorldMatrix.MultiplyPoint(v1) + (Camera.current.transform.position - cameraToWorldMatrix.MultiplyPoint(v2));
      this.m_block.SetFloat("_EyeIndex", node != VRNode.LeftEye ? 1f : 0.0f);
    }
    if ((double) (this.ScopeCamera.transform.position - vector3_1).magnitude >= (double) Mathf.Epsilon)
    {
      this.ScopeCamera.targetTexture = this.m_mainTex;
      if ((Object) this.Reticule != (Object) null)
      {
        Transform transform = this.Reticule.transform;
        transform.position = this.ScopeCamera.transform.position + this.transform.forward * 0.1f;
        transform.rotation = this.transform.rotation;
      }
      this.ScopeCamera.Render();
      this.PostMaterial.SetVector("_CamPos", (Vector4) vector3_1);
      this.PostMaterial.SetMatrix("_ScopeVisualToWorld", this.transform.localToWorldMatrix);
      this.PostMaterial.SetVector("_Forward", (Vector4) this.ScopeCamera.transform.forward);
      this.PostMaterial.SetVector("_Offset", (Vector4) (Vector2.right * this.AngleBlurStrength * 0.01f));
      Graphics.Blit((Texture) this.m_mainTex, this.m_blurTex, this.PostMaterial);
      this.PostMaterial.SetVector("_Offset", (Vector4) (Vector2.up * this.AngleBlurStrength * 0.01f));
      Graphics.Blit((Texture) this.m_blurTex, this.m_mainTex, this.PostMaterial);
    }
    this.m_block.SetVector("_TubeCenter", (Vector4) this.transform.position);
    this.m_block.SetVector("_TubeForward", (Vector4) this.transform.forward);
    this.m_block.SetFloat("_TubeRadius", this.transform.localScale.x);
    this.m_block.SetFloat("_TubeLength", (this.ScopeCamera.transform.position - this.transform.position).magnitude * Mathf.Lerp(1f, this.Magnification, this.AngularOccludeSensitivity));
    this.m_block.SetFloat("_CutoffSoftness", this.CutoffSoftness);
    this.m_block.SetFloat("_LensDistortion", 1f - this.LensSpaceDistortion);
    this.m_block.SetFloat("_Chroma", this.LensChromaticDistortion);
    this.m_block.SetTexture("_MainTex0", (Texture) this.m_mainTex);
    this.m_renderer.SetPropertyBlock(this.m_block);
  }

  public void PointTowards(Vector3 p) => this.ScopeCamera.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(p - this.ScopeCamera.transform.position, this.transform.right), this.transform.up);

  private void RenderScopeTex(VRNode node, RenderTexture tex)
  {
    Vector3 v1 = Quaternion.Inverse(InputTracking.GetLocalRotation(node)) * InputTracking.GetLocalPosition(node);
    Matrix4x4 cameraToWorldMatrix = Camera.current.cameraToWorldMatrix;
    Vector3 v2 = Quaternion.Inverse(InputTracking.GetLocalRotation(VRNode.Head)) * InputTracking.GetLocalPosition(VRNode.Head);
    Vector3 vector3_1 = cameraToWorldMatrix.MultiplyPoint(v1) + (Camera.current.transform.position - cameraToWorldMatrix.MultiplyPoint(v2));
    Vector3 vector3_2 = this.ScopeCamera.transform.position - vector3_1;
    if ((double) vector3_2.magnitude < (double) Mathf.Epsilon)
      return;
    Quaternion.LookRotation(vector3_2.normalized, this.transform.up);
    this.ScopeCamera.targetTexture = tex;
    if ((Object) this.Reticule != (Object) null)
    {
      Transform transform = this.Reticule.transform;
      transform.position = this.ScopeCamera.transform.position + this.transform.forward * 0.1f;
      transform.rotation = this.transform.rotation;
    }
    this.ScopeCamera.Render();
    this.PostMaterial.SetVector("_CamPos", (Vector4) vector3_1);
    this.PostMaterial.SetMatrix("_ScopeVisualToWorld", this.transform.localToWorldMatrix);
    this.PostMaterial.SetVector("_Forward", (Vector4) this.ScopeCamera.transform.forward);
    this.PostMaterial.SetVector("_Offset", (Vector4) (Vector2.right * this.AngleBlurStrength * 0.01f));
    Graphics.Blit((Texture) tex, this.m_blurTex, this.PostMaterial);
    this.PostMaterial.SetVector("_Offset", (Vector4) (Vector2.up * this.AngleBlurStrength * 0.01f));
    Graphics.Blit((Texture) this.m_blurTex, tex, this.PostMaterial);
  }

  private void OnDisable()
  {
    Object.DestroyImmediate((Object) this.m_mainTex);
    Object.DestroyImmediate((Object) this.m_blurTex);
  }
}
