// Decompiled with JetBrains decompiler
// Type: WFX_Demo
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WFX_Demo : MonoBehaviour
{
  public float cameraSpeed = 10f;
  public bool orderedSpawns = true;
  public float step = 1f;
  public float range = 5f;
  private float order = -5f;
  public GameObject walls;
  public GameObject bulletholes;
  public GameObject[] ParticleExamples;
  private int exampleIndex;
  private string randomSpawnsDelay = "0.5";
  private bool randomSpawns;
  private bool slowMo;
  private bool rotateCam = true;
  public Material wood;
  public Material concrete;
  public Material metal;
  public Material checker;
  public Material woodWall;
  public Material concreteWall;
  public Material metalWall;
  public Material checkerWall;
  private string groundTextureStr = "Checker";
  private List<string> groundTextures = new List<string>((IEnumerable<string>) new string[4]
  {
    "Concrete",
    "Wood",
    "Metal",
    "Checker"
  });
  public GameObject m4;
  public GameObject m4fps;
  private bool rotate_m4 = true;

  private void OnMouseDown()
  {
    RaycastHit hitInfo = new RaycastHit();
    if (!this.GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 9999f))
      return;
    GameObject gameObject = this.spawnParticle();
    if (gameObject.name.StartsWith("WFX_MF"))
      return;
    gameObject.transform.position = hitInfo.point + gameObject.transform.position;
  }

  public GameObject spawnParticle()
  {
    GameObject gameObject = Object.Instantiate<GameObject>(this.ParticleExamples[this.exampleIndex]);
    if (gameObject.name.StartsWith("WFX_MF"))
    {
      gameObject.transform.parent = this.ParticleExamples[this.exampleIndex].transform.parent;
      gameObject.transform.localPosition = this.ParticleExamples[this.exampleIndex].transform.localPosition;
      gameObject.transform.localRotation = this.ParticleExamples[this.exampleIndex].transform.localRotation;
    }
    else if (gameObject.name.Contains("Hole"))
      gameObject.transform.parent = this.bulletholes.transform;
    this.SetActiveCrossVersions(gameObject, true);
    return gameObject;
  }

  private void SetActiveCrossVersions(GameObject obj, bool active)
  {
    obj.SetActive(active);
    for (int index = 0; index < obj.transform.childCount; ++index)
      obj.transform.GetChild(index).gameObject.SetActive(active);
  }

  private void OnGUI()
  {
    GUILayout.BeginArea(new Rect(5f, 20f, (float) (Screen.width - 10), 60f));
    GUILayout.BeginHorizontal();
    GUILayout.Label("Effect: " + this.ParticleExamples[this.exampleIndex].name, GUILayout.Width(280f));
    if (GUILayout.Button("<", GUILayout.Width(30f)))
      this.prevParticle();
    if (GUILayout.Button(">", GUILayout.Width(30f)))
      this.nextParticle();
    GUILayout.FlexibleSpace();
    GUILayout.Label("Click on the ground to spawn the selected effect");
    GUILayout.FlexibleSpace();
    if (GUILayout.Button(!this.rotateCam ? "Rotate Camera" : "Pause Camera", GUILayout.Width(110f)))
      this.rotateCam = !this.rotateCam;
    if (GUILayout.Button(!this.GetComponent<Renderer>().enabled ? "Show Ground" : "Hide Ground", GUILayout.Width(90f)))
      this.GetComponent<Renderer>().enabled = !this.GetComponent<Renderer>().enabled;
    if (GUILayout.Button(!this.slowMo ? "Slow Motion" : "Normal Speed", GUILayout.Width(100f)))
    {
      this.slowMo = !this.slowMo;
      Time.timeScale = !this.slowMo ? 1f : 0.33f;
    }
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.Label("Ground texture: " + this.groundTextureStr, GUILayout.Width(160f));
    if (GUILayout.Button("<", GUILayout.Width(30f)))
      this.prevTexture();
    if (GUILayout.Button(">", GUILayout.Width(30f)))
      this.nextTexture();
    GUILayout.EndHorizontal();
    GUILayout.EndArea();
    if (!this.m4.GetComponent<Renderer>().enabled)
      return;
    GUILayout.BeginArea(new Rect(5f, (float) (Screen.height - 100), (float) (Screen.width - 10), 90f));
    this.rotate_m4 = GUILayout.Toggle((this.rotate_m4 ? 1 : 0) != 0, "AutoRotate Weapon", GUILayout.Width(250f));
    GUI.enabled = !this.rotate_m4;
    float x1 = this.m4.transform.localEulerAngles.x;
    float num = (double) x1 <= 90.0 ? x1 : x1 - 180f;
    float y1 = this.m4.transform.localEulerAngles.y;
    float z1 = this.m4.transform.localEulerAngles.z;
    float x2 = GUILayout.HorizontalSlider(num, 0.0f, 179f, GUILayout.Width(256f));
    float y2 = GUILayout.HorizontalSlider(y1, 0.0f, 359f, GUILayout.Width(256f));
    float z2 = GUILayout.HorizontalSlider(z1, 0.0f, 359f, GUILayout.Width(256f));
    if (GUI.changed)
    {
      if ((double) x2 > 90.0)
        x2 += 180f;
      this.m4.transform.localEulerAngles = new Vector3(x2, y2, z2);
      UnityEngine.Debug.Log((object) x2);
    }
    GUILayout.EndArea();
  }

  [DebuggerHidden]
  private IEnumerator RandomSpawnsCoroutine() => (IEnumerator) new WFX_Demo.\u003CRandomSpawnsCoroutine\u003Ec__Iterator0()
  {
    \u0024this = this
  };

  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.LeftArrow))
      this.prevParticle();
    else if (Input.GetKeyDown(KeyCode.RightArrow))
      this.nextParticle();
    if (this.rotateCam)
      Camera.main.transform.RotateAround(Vector3.zero, Vector3.up, this.cameraSpeed * Time.deltaTime);
    if (!this.rotate_m4)
      return;
    this.m4.transform.Rotate(new Vector3(0.0f, 40f, 0.0f) * Time.deltaTime, Space.World);
  }

  private void prevTexture()
  {
    int index = this.groundTextures.IndexOf(this.groundTextureStr) - 1;
    if (index < 0)
      index = this.groundTextures.Count - 1;
    this.groundTextureStr = this.groundTextures[index];
    this.selectMaterial();
  }

  private void nextTexture()
  {
    int index = this.groundTextures.IndexOf(this.groundTextureStr) + 1;
    if (index >= this.groundTextures.Count)
      index = 0;
    this.groundTextureStr = this.groundTextures[index];
    this.selectMaterial();
  }

  private void selectMaterial()
  {
    switch (this.groundTextureStr)
    {
      case "Concrete":
        this.GetComponent<Renderer>().material = this.concrete;
        this.walls.transform.GetChild(0).GetComponent<Renderer>().material = this.concreteWall;
        this.walls.transform.GetChild(1).GetComponent<Renderer>().material = this.concreteWall;
        break;
      case "Wood":
        this.GetComponent<Renderer>().material = this.wood;
        this.walls.transform.GetChild(0).GetComponent<Renderer>().material = this.woodWall;
        this.walls.transform.GetChild(1).GetComponent<Renderer>().material = this.woodWall;
        break;
      case "Metal":
        this.GetComponent<Renderer>().material = this.metal;
        this.walls.transform.GetChild(0).GetComponent<Renderer>().material = this.metalWall;
        this.walls.transform.GetChild(1).GetComponent<Renderer>().material = this.metalWall;
        break;
      case "Checker":
        this.GetComponent<Renderer>().material = this.checker;
        this.walls.transform.GetChild(0).GetComponent<Renderer>().material = this.checkerWall;
        this.walls.transform.GetChild(1).GetComponent<Renderer>().material = this.checkerWall;
        break;
    }
  }

  private void prevParticle()
  {
    --this.exampleIndex;
    if (this.exampleIndex < 0)
      this.exampleIndex = this.ParticleExamples.Length - 1;
    this.showHideStuff();
  }

  private void nextParticle()
  {
    ++this.exampleIndex;
    if (this.exampleIndex >= this.ParticleExamples.Length)
      this.exampleIndex = 0;
    this.showHideStuff();
  }

  private void showHideStuff()
  {
    if (this.ParticleExamples[this.exampleIndex].name.StartsWith("WFX_MF Spr"))
      this.m4.GetComponent<Renderer>().enabled = true;
    else
      this.m4.GetComponent<Renderer>().enabled = false;
    if (this.ParticleExamples[this.exampleIndex].name.StartsWith("WFX_MF FPS"))
      this.m4fps.GetComponent<Renderer>().enabled = true;
    else
      this.m4fps.GetComponent<Renderer>().enabled = false;
    if (this.ParticleExamples[this.exampleIndex].name.StartsWith("WFX_BImpact"))
    {
      this.SetActiveCrossVersions(this.walls, true);
      foreach (Renderer componentsInChild in this.bulletholes.GetComponentsInChildren<Renderer>())
        componentsInChild.enabled = true;
    }
    else
    {
      this.SetActiveCrossVersions(this.walls, false);
      foreach (Renderer componentsInChild in this.bulletholes.GetComponentsInChildren<Renderer>())
        componentsInChild.enabled = false;
    }
    if (this.ParticleExamples[this.exampleIndex].name.Contains("Wood"))
    {
      this.groundTextureStr = "Wood";
      this.selectMaterial();
    }
    else if (this.ParticleExamples[this.exampleIndex].name.Contains("Concrete"))
    {
      this.groundTextureStr = "Concrete";
      this.selectMaterial();
    }
    else if (this.ParticleExamples[this.exampleIndex].name.Contains("Metal"))
    {
      this.groundTextureStr = "Metal";
      this.selectMaterial();
    }
    else if (this.ParticleExamples[this.exampleIndex].name.Contains("Dirt") || this.ParticleExamples[this.exampleIndex].name.Contains("Sand") || this.ParticleExamples[this.exampleIndex].name.Contains("SoftBody"))
    {
      this.groundTextureStr = "Checker";
      this.selectMaterial();
    }
    else
    {
      if (!(this.ParticleExamples[this.exampleIndex].name == "WFX_Explosion"))
        return;
      this.groundTextureStr = "Checker";
      this.selectMaterial();
    }
  }
}
