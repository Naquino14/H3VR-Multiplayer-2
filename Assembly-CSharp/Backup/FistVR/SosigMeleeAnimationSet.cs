// Decompiled with JetBrains decompiler
// Type: FistVR.SosigMeleeAnimationSet
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New Melee Anim Set", menuName = "Sosig/MeleeAnimationSet", order = 0)]
  public class SosigMeleeAnimationSet : ScriptableObject
  {
    public List<Vector3> Frames_LocalPos;
    public List<Vector3> Frames_LocalForward;
    public List<Vector3> Frames_LocalUp;

    private Vector3 GetLerp(List<Vector3> v, float f, bool doEXP, bool loop)
    {
      if (v.Count == 0)
        Debug.Log((object) "Frame list is 0 wtf");
      f = Mathf.Clamp(f, 0.0f, 1f);
      if (doEXP)
        f = Mathf.Pow(f, 2f);
      int index1 = Mathf.Clamp(Mathf.FloorToInt(f / 1f * (float) v.Count), 0, v.Count - 1);
      if (index1 < 0)
        index1 = 0;
      int index2 = index1 + 1;
      if (index2 >= v.Count)
        index2 = !loop ? v.Count - 1 : 0;
      Vector3 a = v[index1];
      Vector3 b = v[index2];
      float num = 1f / (float) v.Count;
      float t = f % num * (float) v.Count;
      return Vector3.Slerp(a, b, t);
    }

    public Vector3 GetPos(float f, bool doEXP, bool loop) => this.GetLerp(this.Frames_LocalPos, f, doEXP, loop);

    public Vector3 GetForward(float f, bool doEXP, bool loop) => this.GetLerp(this.Frames_LocalForward, f, doEXP, loop);

    public Vector3 GetUp(float f, bool doEXP, bool loop) => this.GetLerp(this.Frames_LocalUp, f, doEXP, loop);
  }
}
