// Decompiled with JetBrains decompiler
// Type: Valve.VR.InteractionSystem.Sample.TargetMeasurement
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

namespace Valve.VR.InteractionSystem.Sample
{
  public class TargetMeasurement : MonoBehaviour
  {
    public GameObject visualWrapper;
    public Transform measurementTape;
    public Transform endPoint;
    public Text measurementTextM;
    public Text measurementTextFT;
    public float maxDistanceToDraw = 6f;
    public bool drawTape;
    private float lastDistance;

    private void Update()
    {
      if (!((Object) Camera.main != (Object) null))
        return;
      Vector3 position = Camera.main.transform.position;
      position.y = this.endPoint.position.y;
      float y = Vector3.Distance(position, this.endPoint.position);
      this.transform.position = Vector3.Lerp(position, this.endPoint.position, 0.5f);
      this.transform.forward = this.endPoint.position - position;
      this.measurementTape.localScale = new Vector3(0.05f, y, 0.05f);
      if ((double) Mathf.Abs(y - this.lastDistance) > 0.00999999977648258)
      {
        this.measurementTextM.text = y.ToString("00.0m");
        this.measurementTextFT.text = ((double) y * 3.28084).ToString("00.0ft");
        this.lastDistance = y;
      }
      if (this.drawTape)
        this.visualWrapper.SetActive((double) y < (double) this.maxDistanceToDraw);
      else
        this.visualWrapper.SetActive(false);
    }
  }
}
