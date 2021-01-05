// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_PlayArea
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

namespace Valve.VR
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (MeshRenderer), typeof (MeshFilter))]
  public class SteamVR_PlayArea : MonoBehaviour
  {
    public float borderThickness = 0.15f;
    public float wireframeHeight = 2f;
    public bool drawWireframeWhenSelectedOnly;
    public bool drawInGame = true;
    public SteamVR_PlayArea.Size size;
    public Color color = Color.cyan;
    [HideInInspector]
    public Vector3[] vertices;

    public static bool GetBounds(SteamVR_PlayArea.Size size, ref HmdQuad_t pRect)
    {
      if (size == SteamVR_PlayArea.Size.Calibrated)
      {
        bool flag1 = false;
        if (Application.isEditor && !Application.isPlaying)
          flag1 = SteamVR.InitializeTemporarySession();
        CVRChaperone chaperone = OpenVR.Chaperone;
        bool flag2 = chaperone != null && chaperone.GetPlayAreaRect(ref pRect);
        if (!flag2)
          UnityEngine.Debug.LogWarning((object) "<b>[SteamVR]</b> Failed to get Calibrated Play Area bounds!  Make sure you have tracking first, and that your space is calibrated.");
        if (flag1)
          SteamVR.ExitTemporarySession();
        return flag2;
      }
      try
      {
        string[] strArray = size.ToString().Substring(1).Split(new char[1]
        {
          'x'
        }, 2);
        float num1 = float.Parse(strArray[0]) / 200f;
        float num2 = float.Parse(strArray[1]) / 200f;
        pRect.vCorners0.v0 = num1;
        pRect.vCorners0.v1 = 0.0f;
        pRect.vCorners0.v2 = -num2;
        pRect.vCorners1.v0 = -num1;
        pRect.vCorners1.v1 = 0.0f;
        pRect.vCorners1.v2 = -num2;
        pRect.vCorners2.v0 = -num1;
        pRect.vCorners2.v1 = 0.0f;
        pRect.vCorners2.v2 = num2;
        pRect.vCorners3.v0 = num1;
        pRect.vCorners3.v1 = 0.0f;
        pRect.vCorners3.v2 = num2;
        return true;
      }
      catch
      {
      }
      return false;
    }

    public void BuildMesh()
    {
      HmdQuad_t pRect = new HmdQuad_t();
      if (!SteamVR_PlayArea.GetBounds(this.size, ref pRect))
        return;
      HmdVector3_t[] hmdVector3TArray = new HmdVector3_t[4]
      {
        pRect.vCorners0,
        pRect.vCorners1,
        pRect.vCorners2,
        pRect.vCorners3
      };
      this.vertices = new Vector3[hmdVector3TArray.Length * 2];
      for (int index = 0; index < hmdVector3TArray.Length; ++index)
      {
        HmdVector3_t hmdVector3T = hmdVector3TArray[index];
        this.vertices[index] = new Vector3(hmdVector3T.v0, 0.01f, hmdVector3T.v2);
      }
      if ((double) this.borderThickness == 0.0)
      {
        this.GetComponent<MeshFilter>().mesh = (Mesh) null;
      }
      else
      {
        for (int index1 = 0; index1 < hmdVector3TArray.Length; ++index1)
        {
          int index2 = (index1 + 1) % hmdVector3TArray.Length;
          int index3 = (index1 + hmdVector3TArray.Length - 1) % hmdVector3TArray.Length;
          Vector3 normalized1 = (this.vertices[index2] - this.vertices[index1]).normalized;
          Vector3 normalized2 = (this.vertices[index3] - this.vertices[index1]).normalized;
          Vector3 vector3 = this.vertices[index1] + Vector3.Cross(normalized1, Vector3.up) * this.borderThickness + Vector3.Cross(normalized2, Vector3.down) * this.borderThickness;
          this.vertices[hmdVector3TArray.Length + index1] = vector3;
        }
        int[] numArray = new int[24]
        {
          0,
          4,
          1,
          1,
          4,
          5,
          1,
          5,
          2,
          2,
          5,
          6,
          2,
          6,
          3,
          3,
          6,
          7,
          3,
          7,
          0,
          0,
          7,
          4
        };
        Vector2[] vector2Array = new Vector2[8]
        {
          new Vector2(0.0f, 0.0f),
          new Vector2(1f, 0.0f),
          new Vector2(0.0f, 0.0f),
          new Vector2(1f, 0.0f),
          new Vector2(0.0f, 1f),
          new Vector2(1f, 1f),
          new Vector2(0.0f, 1f),
          new Vector2(1f, 1f)
        };
        Color[] colorArray = new Color[8]
        {
          this.color,
          this.color,
          this.color,
          this.color,
          new Color(this.color.r, this.color.g, this.color.b, 0.0f),
          new Color(this.color.r, this.color.g, this.color.b, 0.0f),
          new Color(this.color.r, this.color.g, this.color.b, 0.0f),
          new Color(this.color.r, this.color.g, this.color.b, 0.0f)
        };
        Mesh mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = this.vertices;
        mesh.uv = vector2Array;
        mesh.colors = colorArray;
        mesh.triangles = numArray;
        MeshRenderer component = this.GetComponent<MeshRenderer>();
        component.material = new Material(Shader.Find("Sprites/Default"));
        component.reflectionProbeUsage = ReflectionProbeUsage.Off;
        component.shadowCastingMode = ShadowCastingMode.Off;
        component.receiveShadows = false;
        component.lightProbeUsage = LightProbeUsage.Off;
      }
    }

    private void OnDrawGizmos()
    {
      if (this.drawWireframeWhenSelectedOnly)
        return;
      this.DrawWireframe();
    }

    private void OnDrawGizmosSelected()
    {
      if (!this.drawWireframeWhenSelectedOnly)
        return;
      this.DrawWireframe();
    }

    public void DrawWireframe()
    {
      if (this.vertices == null || this.vertices.Length == 0)
        return;
      Vector3 vector3_1 = this.transform.TransformVector(Vector3.up * this.wireframeHeight);
      for (int index1 = 0; index1 < 4; ++index1)
      {
        int index2 = (index1 + 1) % 4;
        Vector3 from = this.transform.TransformPoint(this.vertices[index1]);
        Vector3 vector3_2 = from + vector3_1;
        Vector3 to1 = this.transform.TransformPoint(this.vertices[index2]);
        Vector3 to2 = to1 + vector3_1;
        Gizmos.DrawLine(from, vector3_2);
        Gizmos.DrawLine(from, to1);
        Gizmos.DrawLine(vector3_2, to2);
      }
    }

    public void OnEnable()
    {
      if (!Application.isPlaying)
        return;
      this.GetComponent<MeshRenderer>().enabled = this.drawInGame;
      this.enabled = false;
      if (!this.drawInGame || this.size != SteamVR_PlayArea.Size.Calibrated)
        return;
      this.StartCoroutine(this.UpdateBounds());
    }

    [DebuggerHidden]
    private IEnumerator UpdateBounds() => (IEnumerator) new SteamVR_PlayArea.\u003CUpdateBounds\u003Ec__Iterator0()
    {
      \u0024this = this
    };

    public enum Size
    {
      Calibrated,
      _400x300,
      _300x225,
      _200x150,
    }
  }
}
