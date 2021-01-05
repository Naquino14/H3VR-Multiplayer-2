// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.HableCurve
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public class HableCurve
  {
    public readonly HableCurve.Segment[] segments = new HableCurve.Segment[3];
    public readonly HableCurve.Uniforms uniforms;

    public HableCurve()
    {
      for (int index = 0; index < 3; ++index)
        this.segments[index] = new HableCurve.Segment();
      this.uniforms = new HableCurve.Uniforms(this);
    }

    public float whitePoint { get; private set; }

    public float inverseWhitePoint { get; private set; }

    public float x0 { get; private set; }

    public float x1 { get; private set; }

    public float Eval(float x)
    {
      float x1 = x * this.inverseWhitePoint;
      return this.segments[(double) x1 >= (double) this.x0 ? ((double) x1 >= (double) this.x1 ? 2 : 1) : 0].Eval(x1);
    }

    public void Init(
      float toeStrength,
      float toeLength,
      float shoulderStrength,
      float shoulderLength,
      float shoulderAngle,
      float gamma)
    {
      HableCurve.DirectParams srcParams = new HableCurve.DirectParams();
      toeLength = Mathf.Pow(Mathf.Clamp01(toeLength), 2.2f);
      toeStrength = Mathf.Clamp01(toeStrength);
      shoulderAngle = Mathf.Clamp01(shoulderAngle);
      shoulderStrength = Mathf.Clamp(shoulderStrength, 1E-05f, 0.99999f);
      shoulderLength = Mathf.Max(0.0f, shoulderLength);
      gamma = Mathf.Max(1E-05f, gamma);
      float num1 = toeLength * 0.5f;
      float num2 = (1f - toeStrength) * num1;
      float num3 = 1f - num2;
      float num4 = num1 + num3;
      float num5 = (1f - shoulderStrength) * num3;
      float num6 = num1 + num5;
      float num7 = num2 + num5;
      float num8 = RuntimeUtilities.Exp2(shoulderLength) - 1f;
      float num9 = num4 + num8;
      srcParams.x0 = num1;
      srcParams.y0 = num2;
      srcParams.x1 = num6;
      srcParams.y1 = num7;
      srcParams.W = num9;
      srcParams.gamma = gamma;
      srcParams.overshootX = srcParams.W * 2f * shoulderAngle * shoulderLength;
      srcParams.overshootY = 0.5f * shoulderAngle * shoulderLength;
      this.InitSegments(srcParams);
    }

    private void InitSegments(HableCurve.DirectParams srcParams)
    {
      HableCurve.DirectParams directParams = srcParams;
      this.whitePoint = srcParams.W;
      this.inverseWhitePoint = 1f / srcParams.W;
      directParams.W = 1f;
      directParams.x0 /= srcParams.W;
      directParams.x1 /= srcParams.W;
      directParams.overshootX = srcParams.overshootX / srcParams.W;
      float m1;
      float b;
      this.AsSlopeIntercept(out m1, out b, directParams.x0, directParams.x1, directParams.y0, directParams.y1);
      float gamma = srcParams.gamma;
      HableCurve.Segment segment1 = this.segments[1];
      segment1.offsetX = (float) -((double) b / (double) m1);
      segment1.offsetY = 0.0f;
      segment1.scaleX = 1f;
      segment1.scaleY = 1f;
      segment1.lnA = gamma * Mathf.Log(m1);
      segment1.B = gamma;
      float m2 = this.EvalDerivativeLinearGamma(m1, b, gamma, directParams.x0);
      float m3 = this.EvalDerivativeLinearGamma(m1, b, gamma, directParams.x1);
      directParams.y0 = Mathf.Max(1E-05f, Mathf.Pow(directParams.y0, directParams.gamma));
      directParams.y1 = Mathf.Max(1E-05f, Mathf.Pow(directParams.y1, directParams.gamma));
      directParams.overshootY = Mathf.Pow(1f + directParams.overshootY, directParams.gamma) - 1f;
      this.x0 = directParams.x0;
      this.x1 = directParams.x1;
      HableCurve.Segment segment2 = this.segments[0];
      segment2.offsetX = 0.0f;
      segment2.offsetY = 0.0f;
      segment2.scaleX = 1f;
      segment2.scaleY = 1f;
      float lnA1;
      float B1;
      this.SolveAB(out lnA1, out B1, directParams.x0, directParams.y0, m2);
      segment2.lnA = lnA1;
      segment2.B = B1;
      HableCurve.Segment segment3 = this.segments[2];
      float x0 = 1f + directParams.overshootX - directParams.x1;
      float y0 = 1f + directParams.overshootY - directParams.y1;
      float lnA2;
      float B2;
      this.SolveAB(out lnA2, out B2, x0, y0, m3);
      segment3.offsetX = 1f + directParams.overshootX;
      segment3.offsetY = 1f + directParams.overshootY;
      segment3.scaleX = -1f;
      segment3.scaleY = -1f;
      segment3.lnA = lnA2;
      segment3.B = B2;
      float num = 1f / this.segments[2].Eval(1f);
      this.segments[0].offsetY *= num;
      this.segments[0].scaleY *= num;
      this.segments[1].offsetY *= num;
      this.segments[1].scaleY *= num;
      this.segments[2].offsetY *= num;
      this.segments[2].scaleY *= num;
    }

    private void SolveAB(out float lnA, out float B, float x0, float y0, float m)
    {
      B = m * x0 / y0;
      lnA = Mathf.Log(y0) - B * Mathf.Log(x0);
    }

    private void AsSlopeIntercept(
      out float m,
      out float b,
      float x0,
      float x1,
      float y0,
      float y1)
    {
      float num1 = y1 - y0;
      float num2 = x1 - x0;
      m = (double) num2 != 0.0 ? num1 / num2 : 1f;
      b = y0 - x0 * m;
    }

    private float EvalDerivativeLinearGamma(float m, float b, float g, float x) => g * m * Mathf.Pow(m * x + b, g - 1f);

    public class Segment
    {
      public float offsetX;
      public float offsetY;
      public float scaleX;
      public float scaleY;
      public float lnA;
      public float B;

      public float Eval(float x)
      {
        float f = (x - this.offsetX) * this.scaleX;
        float num = 0.0f;
        if ((double) f > 0.0)
          num = Mathf.Exp(this.lnA + this.B * Mathf.Log(f));
        return num * this.scaleY + this.offsetY;
      }
    }

    private struct DirectParams
    {
      internal float x0;
      internal float y0;
      internal float x1;
      internal float y1;
      internal float W;
      internal float overshootX;
      internal float overshootY;
      internal float gamma;
    }

    public class Uniforms
    {
      private HableCurve parent;

      internal Uniforms(HableCurve parent) => this.parent = parent;

      public Vector4 curve => new Vector4(this.parent.inverseWhitePoint, this.parent.x0, this.parent.x1, 0.0f);

      public Vector4 toeSegmentA
      {
        get
        {
          HableCurve.Segment segment = this.parent.segments[0];
          return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
        }
      }

      public Vector4 toeSegmentB
      {
        get
        {
          HableCurve.Segment segment = this.parent.segments[0];
          return new Vector4(segment.lnA, segment.B, 0.0f, 0.0f);
        }
      }

      public Vector4 midSegmentA
      {
        get
        {
          HableCurve.Segment segment = this.parent.segments[1];
          return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
        }
      }

      public Vector4 midSegmentB
      {
        get
        {
          HableCurve.Segment segment = this.parent.segments[1];
          return new Vector4(segment.lnA, segment.B, 0.0f, 0.0f);
        }
      }

      public Vector4 shoSegmentA
      {
        get
        {
          HableCurve.Segment segment = this.parent.segments[2];
          return new Vector4(segment.offsetX, segment.offsetY, segment.scaleX, segment.scaleY);
        }
      }

      public Vector4 shoSegmentB
      {
        get
        {
          HableCurve.Segment segment = this.parent.segments[2];
          return new Vector4(segment.lnA, segment.B, 0.0f, 0.0f);
        }
      }
    }
  }
}
