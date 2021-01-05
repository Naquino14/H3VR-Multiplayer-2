// Decompiled with JetBrains decompiler
// Type: Valve.VR.SteamVR_Behaviour_SkeletonCustom
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

namespace Valve.VR
{
  public class SteamVR_Behaviour_SkeletonCustom : SteamVR_Behaviour_Skeleton
  {
    [SerializeField]
    protected Transform _wrist;
    [SerializeField]
    protected Transform _thumbMetacarpal;
    [SerializeField]
    protected Transform _thumbProximal;
    [SerializeField]
    protected Transform _thumbMiddle;
    [SerializeField]
    protected Transform _thumbDistal;
    [SerializeField]
    protected Transform _thumbTip;
    [SerializeField]
    protected Transform _thumbAux;
    [SerializeField]
    protected Transform _indexMetacarpal;
    [SerializeField]
    protected Transform _indexProximal;
    [SerializeField]
    protected Transform _indexMiddle;
    [SerializeField]
    protected Transform _indexDistal;
    [SerializeField]
    protected Transform _indexTip;
    [SerializeField]
    protected Transform _indexAux;
    [SerializeField]
    protected Transform _middleMetacarpal;
    [SerializeField]
    protected Transform _middleProximal;
    [SerializeField]
    protected Transform _middleMiddle;
    [SerializeField]
    protected Transform _middleDistal;
    [SerializeField]
    protected Transform _middleTip;
    [SerializeField]
    protected Transform _middleAux;
    [SerializeField]
    protected Transform _ringMetacarpal;
    [SerializeField]
    protected Transform _ringProximal;
    [SerializeField]
    protected Transform _ringMiddle;
    [SerializeField]
    protected Transform _ringDistal;
    [SerializeField]
    protected Transform _ringTip;
    [SerializeField]
    protected Transform _ringAux;
    [SerializeField]
    protected Transform _pinkyMetacarpal;
    [SerializeField]
    protected Transform _pinkyProximal;
    [SerializeField]
    protected Transform _pinkyMiddle;
    [SerializeField]
    protected Transform _pinkyDistal;
    [SerializeField]
    protected Transform _pinkyTip;
    [SerializeField]
    protected Transform _pinkyAux;

    protected override void AssignBonesArray()
    {
      this.bones[1] = this._wrist;
      this.bones[2] = this._thumbProximal;
      this.bones[3] = this._thumbMiddle;
      this.bones[4] = this._thumbDistal;
      this.bones[5] = this._thumbTip;
      this.bones[26] = this._thumbAux;
      this.bones[7] = this._indexProximal;
      this.bones[8] = this._indexMiddle;
      this.bones[9] = this._indexDistal;
      this.bones[10] = this._indexTip;
      this.bones[27] = this._indexAux;
      this.bones[12] = this._middleProximal;
      this.bones[13] = this._middleMiddle;
      this.bones[14] = this._middleDistal;
      this.bones[15] = this._middleTip;
      this.bones[28] = this._middleAux;
      this.bones[17] = this._ringProximal;
      this.bones[18] = this._ringMiddle;
      this.bones[19] = this._ringDistal;
      this.bones[20] = this._ringTip;
      this.bones[29] = this._ringAux;
      this.bones[22] = this._pinkyProximal;
      this.bones[23] = this._pinkyMiddle;
      this.bones[24] = this._pinkyDistal;
      this.bones[25] = this._pinkyTip;
      this.bones[30] = this._pinkyAux;
    }
  }
}
