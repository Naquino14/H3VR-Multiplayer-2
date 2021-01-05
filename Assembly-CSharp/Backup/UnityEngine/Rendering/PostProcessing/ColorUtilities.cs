// Decompiled with JetBrains decompiler
// Type: UnityEngine.Rendering.PostProcessing.ColorUtilities
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

namespace UnityEngine.Rendering.PostProcessing
{
  public static class ColorUtilities
  {
    private const float logC_cut = 0.011361f;
    private const float logC_a = 5.555556f;
    private const float logC_b = 0.047996f;
    private const float logC_c = 0.244161f;
    private const float logC_d = 0.386036f;
    private const float logC_e = 5.301883f;
    private const float logC_f = 0.092819f;

    public static float StandardIlluminantY(float x) => (float) (2.86999988555908 * (double) x - 3.0 * (double) x * (double) x - 0.275095075368881);

    public static Vector3 CIExyToLMS(float x, float y)
    {
      float num1 = 1f;
      float num2 = num1 * x / y;
      float num3 = num1 * (1f - x - y) / y;
      return new Vector3((float) (0.732800006866455 * (double) num2 + 0.42960000038147 * (double) num1 - 0.162400007247925 * (double) num3), (float) (-0.703599989414215 * (double) num2 + 1.69749999046326 * (double) num1 + 0.00609999988228083 * (double) num3), (float) (3.0 / 1000.0 * (double) num2 + 0.0136000001803041 * (double) num1 + 0.983399987220764 * (double) num3));
    }

    public static Vector3 ComputeColorBalance(float temperature, float tint)
    {
      float num1 = temperature / 60f;
      float num2 = tint / 60f;
      float x = (float) (0.312709987163544 - (double) num1 * ((double) num1 >= 0.0 ? 0.0500000007450581 : 0.100000001490116));
      float y = ColorUtilities.StandardIlluminantY(x) + num2 * 0.05f;
      Vector3 vector3 = new Vector3(0.949237f, 1.03542f, 1.08728f);
      Vector3 lms = ColorUtilities.CIExyToLMS(x, y);
      return new Vector3(vector3.x / lms.x, vector3.y / lms.y, vector3.z / lms.z);
    }

    public static Vector3 ColorToLift(Vector4 color)
    {
      Vector3 vector3 = new Vector3(color.x, color.y, color.z);
      float num = (float) ((double) vector3.x * 0.212599992752075 + (double) vector3.y * 0.715200006961823 + (double) vector3.z * 0.0722000002861023);
      vector3 = new Vector3(vector3.x - num, vector3.y - num, vector3.z - num);
      float w = color.w;
      return new Vector3(vector3.x + w, vector3.y + w, vector3.z + w);
    }

    public static Vector3 ColorToInverseGamma(Vector4 color)
    {
      Vector3 vector3 = new Vector3(color.x, color.y, color.z);
      float num1 = (float) ((double) vector3.x * 0.212599992752075 + (double) vector3.y * 0.715200006961823 + (double) vector3.z * 0.0722000002861023);
      vector3 = new Vector3(vector3.x - num1, vector3.y - num1, vector3.z - num1);
      float num2 = color.w + 1f;
      return new Vector3(1f / Mathf.Max(vector3.x + num2, 1f / 1000f), 1f / Mathf.Max(vector3.y + num2, 1f / 1000f), 1f / Mathf.Max(vector3.z + num2, 1f / 1000f));
    }

    public static Vector3 ColorToGain(Vector4 color)
    {
      Vector3 vector3 = new Vector3(color.x, color.y, color.z);
      float num1 = (float) ((double) vector3.x * 0.212599992752075 + (double) vector3.y * 0.715200006961823 + (double) vector3.z * 0.0722000002861023);
      vector3 = new Vector3(vector3.x - num1, vector3.y - num1, vector3.z - num1);
      float num2 = color.w + 1f;
      return new Vector3(vector3.x + num2, vector3.y + num2, vector3.z + num2);
    }

    public static float LogCToLinear(float x) => (double) x > 0.15305370092392 ? (float) (((double) Mathf.Pow(10f, (float) (((double) x - 0.38603600859642) / 0.244160994887352)) - 0.047995999455452) / 5.55555582046509) : (float) (((double) x - 0.0928189978003502) / 5.30188322067261);

    public static float LinearToLogC(float x) => (double) x > 0.0113610001280904 ? (float) (0.244160994887352 * (double) Mathf.Log10((float) (5.55555582046509 * (double) x + 0.047995999455452)) + 0.38603600859642) : (float) (5.30188322067261 * (double) x + 0.0928189978003502);

    public static uint ToHex(Color c) => (uint) ((int) (uint) ((double) c.a * (double) byte.MaxValue) << 24 | (int) (uint) ((double) c.r * (double) byte.MaxValue) << 16 | (int) (uint) ((double) c.g * (double) byte.MaxValue) << 8) | (uint) ((double) c.b * (double) byte.MaxValue);

    public static Color ToRGBA(uint hex) => new Color((float) (hex >> 16 & (uint) byte.MaxValue) / (float) byte.MaxValue, (float) (hex >> 8 & (uint) byte.MaxValue) / (float) byte.MaxValue, (float) (hex & (uint) byte.MaxValue) / (float) byte.MaxValue, (float) (hex >> 24 & (uint) byte.MaxValue) / (float) byte.MaxValue);
  }
}
