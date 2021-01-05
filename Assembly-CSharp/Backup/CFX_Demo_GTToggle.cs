// Decompiled with JetBrains decompiler
// Type: CFX_Demo_GTToggle
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

public class CFX_Demo_GTToggle : MonoBehaviour
{
  public Texture Normal;
  public Texture Hover;
  public Color NormalColor = (Color) new Color32((byte) 128, (byte) 128, (byte) 128, (byte) 128);
  public Color DisabledColor = (Color) new Color32((byte) 128, (byte) 128, (byte) 128, (byte) 48);
  public bool State = true;
  public string Callback;
  public GameObject Receiver;
  private Rect CollisionRect;
  private bool Over;
  private GUIText Label;

  private void Awake()
  {
    this.CollisionRect = this.GetComponent<GUITexture>().GetScreenRect(Camera.main);
    this.Label = this.GetComponentInChildren<GUIText>();
    this.UpdateTexture();
  }

  private void Update()
  {
    if (this.CollisionRect.Contains(Input.mousePosition))
    {
      this.Over = true;
      if (Input.GetMouseButtonDown(0))
        this.OnClick();
    }
    else
    {
      this.Over = false;
      this.GetComponent<GUITexture>().color = this.NormalColor;
    }
    this.UpdateTexture();
  }

  private void OnClick()
  {
    this.State = !this.State;
    this.Receiver.SendMessage(this.Callback);
  }

  private void UpdateTexture()
  {
    Color color = !this.State ? this.DisabledColor : this.NormalColor;
    if (this.Over)
      this.GetComponent<GUITexture>().texture = this.Hover;
    else
      this.GetComponent<GUITexture>().texture = this.Normal;
    this.GetComponent<GUITexture>().color = color;
    if (!((Object) this.Label != (Object) null))
      return;
    this.Label.color = color * 1.75f;
  }
}
