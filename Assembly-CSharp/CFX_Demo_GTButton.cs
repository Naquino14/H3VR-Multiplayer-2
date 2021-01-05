// Decompiled with JetBrains decompiler
// Type: CFX_Demo_GTButton
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class CFX_Demo_GTButton : MonoBehaviour
{
  public Color NormalColor = (Color) new Color32((byte) 128, (byte) 128, (byte) 128, (byte) 128);
  public Color HoverColor = (Color) new Color32((byte) 128, (byte) 128, (byte) 128, (byte) 128);
  public string Callback;
  public GameObject Receiver;
  private Rect CollisionRect;
  private bool Over;

  private void Awake() => this.CollisionRect = this.GetComponent<GUITexture>().GetScreenRect(Camera.main);

  private void Update()
  {
    if (this.CollisionRect.Contains(Input.mousePosition))
    {
      this.GetComponent<GUITexture>().color = this.HoverColor;
      if (!Input.GetMouseButtonDown(0))
        return;
      this.OnClick();
    }
    else
      this.GetComponent<GUITexture>().color = this.NormalColor;
  }

  private void OnClick() => this.Receiver.SendMessage(this.Callback);
}
