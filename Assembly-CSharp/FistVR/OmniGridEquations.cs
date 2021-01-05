// Decompiled with JetBrains decompiler
// Type: FistVR.OmniGridEquations
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  [CreateAssetMenu(fileName = "New GridEquation List", menuName = "OmniSequencer/GridEquationList", order = 0)]
  public class OmniGridEquations : ScriptableObject
  {
    public List<Vector3> MultiplicationEquations;
    public List<int> MultiplesOf3;
    public List<int> MultiplesOf4;
    public List<int> Primes;
  }
}
