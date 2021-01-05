// Decompiled with JetBrains decompiler
// Type: FistVR.KeyboardKey
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class KeyboardKey : FVRInteractiveObject
  {
    [Header("Key Options")]
    public Keyboard Keyboard;
    public Text Text;
    public KeyboardKey.KeyBoardKeyType KeyType;
    public string LowerCase;
    public string UpperCase;
    private float timeSinceHit = 1f;
    public float reHitThreshold = 0.1f;

    protected override void Awake()
    {
      base.Awake();
      this.Text = this.GetComponent<Text>();
    }

    public override void Poke(FVRViveHand hand)
    {
      base.Poke(hand);
      if ((double) this.timeSinceHit < (double) this.reHitThreshold)
        return;
      this.timeSinceHit = 0.0f;
      this.Keyboard.KeyInput(this.KeyType, this.LowerCase, this.UpperCase);
    }

    protected override void FVRUpdate()
    {
      base.FVRUpdate();
      if ((double) this.timeSinceHit >= 1.0)
        return;
      this.timeSinceHit += Time.deltaTime;
    }

    public void Press() => this.Keyboard.KeyInput(this.KeyType, this.LowerCase, this.UpperCase);

    public enum KeyBoardKeyType
    {
      AlphaNumeric,
      Space,
      Tab,
      Shift,
      Caps,
      Backspace,
      Enter,
    }
  }
}
