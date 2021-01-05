// Decompiled with JetBrains decompiler
// Type: FistVR.OmniSpawner_Shapes
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace FistVR
{
  public class OmniSpawner_Shapes : OmniSpawner
  {
    private OmniSpawnDef_Shape m_def;
    public GameObject[] ShapePrefabs;
    public GameObject InstructionPrefab;
    private bool m_canPresent;
    private int m_curInstruction;
    private int m_correctShapesLeft;
    private int m_incorrectShapesLeft;
    private List<OmniShape> m_activeShapes = new List<OmniShape>();
    private List<OmniSpawnDef_Shape.ShapeInstruction> m_instructions;
    private int m_shapeAmount = 3;
    private OmniShapeInstructionPanel m_panel;
    public List<string> HexColors = new List<string>()
    {
      "FF0000FF",
      "FF9900FF",
      "FFFF00FF",
      "33CC33FF",
      "0066FFFF",
      "990099FF",
      "FF1294FF",
      "A15229FF"
    };

    public override void Configure(OmniSpawnDef Def, OmniWaveEngagementRange Range)
    {
      base.Configure(Def, Range);
      this.m_def = Def as OmniSpawnDef_Shape;
      this.m_instructions = this.m_def.Instructions;
      this.m_shapeAmount = this.m_def.ShapeAmount;
    }

    public override void BeginSpawning()
    {
      base.BeginSpawning();
      this.m_canPresent = true;
    }

    public override void EndSpawning()
    {
      base.EndSpawning();
      this.m_canPresent = false;
    }

    public override void Activate() => base.Activate();

    public override int Deactivate()
    {
      this.DespawnActiveShapes();
      Object.Destroy((Object) this.m_panel.gameObject);
      this.m_panel = (OmniShapeInstructionPanel) null;
      return base.Deactivate();
    }

    private void Update() => this.UpdateMe();

    private void UpdateMe()
    {
      if (!this.m_isConfigured)
        return;
      switch (this.m_state)
      {
        case OmniSpawner.SpawnerState.Deactivating:
          this.Deactivating();
          break;
        case OmniSpawner.SpawnerState.Activated:
          this.SpawningLoop();
          break;
        case OmniSpawner.SpawnerState.Activating:
          this.Activating();
          break;
      }
    }

    public void ShapeStruck(bool wasCorrect)
    {
      if (wasCorrect)
      {
        --this.m_correctShapesLeft;
        this.AddPoints(100);
        this.AudSource.PlayOneShot(this.AudClip_Success, 1f);
      }
      else
      {
        --this.m_incorrectShapesLeft;
        this.AddPoints(-120);
        this.AudSource.PlayOneShot(this.AudClip_Failure, 1f);
      }
    }

    private void DespawnActiveShapes()
    {
      for (int index = 0; index < this.m_activeShapes.Count; ++index)
      {
        if ((Object) this.m_activeShapes[index] != (Object) null)
          this.m_activeShapes[index].TurnOff();
      }
      this.m_activeShapes.Clear();
    }

    private void SpawningLoop()
    {
      if (!this.m_canPresent || this.m_correctShapesLeft != 0)
        return;
      this.DespawnActiveShapes();
      if (this.m_curInstruction >= this.m_def.Instructions.Count)
      {
        this.m_isDoneSpawning = true;
        this.m_isReadyForWaveEnd = true;
      }
      else
      {
        this.SpawnInstructions();
        this.SpawnShapeSet();
      }
    }

    private void SpawnInstructions()
    {
      if (!((Object) this.m_panel == (Object) null))
        return;
      this.m_panel = Object.Instantiate<GameObject>(this.InstructionPrefab, new Vector3(0.0f, 3f, this.GetRange()), Quaternion.identity).GetComponent<OmniShapeInstructionPanel>();
    }

    private void SpawnShapeSet()
    {
      int num1 = 1;
      this.AudSource.PlayOneShot(this.AudClip_Spawn, 1f);
      bool flag1 = false;
      OmniSpawnDef_Shape.OmniShapeColor omniShapeColor = OmniSpawnDef_Shape.OmniShapeColor.Red;
      List<OmniSpawnDef_Shape.OmniShapeColor> omniShapeColorList = new List<OmniSpawnDef_Shape.OmniShapeColor>();
      bool flag2 = false;
      OmniSpawnDef_Shape.OmniShapeType omniShapeType1 = OmniSpawnDef_Shape.OmniShapeType.Circle;
      List<OmniSpawnDef_Shape.OmniShapeType> omniShapeTypeList = new List<OmniSpawnDef_Shape.OmniShapeType>();
      bool flag3 = false;
      for (int index = 0; index < 8; ++index)
      {
        omniShapeColorList.Add((OmniSpawnDef_Shape.OmniShapeColor) index);
        omniShapeTypeList.Add((OmniSpawnDef_Shape.OmniShapeType) index);
      }
      switch (this.m_instructions[this.m_curInstruction])
      {
        case OmniSpawnDef_Shape.ShapeInstruction.ShootTheColor:
          flag1 = true;
          omniShapeColor = omniShapeColorList[Random.Range(0, omniShapeColorList.Count)];
          omniShapeColorList.Remove(omniShapeColor);
          num1 = 1;
          break;
        case OmniSpawnDef_Shape.ShapeInstruction.ShootTheShape:
          flag2 = true;
          omniShapeType1 = omniShapeTypeList[Random.Range(0, omniShapeTypeList.Count)];
          omniShapeTypeList.Remove(omniShapeType1);
          num1 = 1;
          break;
        case OmniSpawnDef_Shape.ShapeInstruction.ShootTheColorShape:
          flag1 = true;
          flag2 = true;
          omniShapeColor = omniShapeColorList[Random.Range(0, omniShapeColorList.Count)];
          omniShapeColorList.Remove(omniShapeColor);
          omniShapeType1 = omniShapeTypeList[Random.Range(0, omniShapeTypeList.Count)];
          omniShapeTypeList.Remove(omniShapeType1);
          num1 = 1;
          break;
        case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheColor:
          flag1 = true;
          omniShapeColor = omniShapeColorList[Random.Range(0, omniShapeColorList.Count)];
          omniShapeColorList.Remove(omniShapeColor);
          num1 = this.m_shapeAmount / 2;
          break;
        case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheShape:
          flag2 = true;
          omniShapeType1 = omniShapeTypeList[Random.Range(0, omniShapeTypeList.Count)];
          omniShapeTypeList.Remove(omniShapeType1);
          num1 = this.m_shapeAmount / 2;
          break;
        case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheColorShape:
          flag1 = true;
          flag2 = true;
          omniShapeColor = omniShapeColorList[Random.Range(0, omniShapeColorList.Count)];
          omniShapeColorList.Remove(omniShapeColor);
          omniShapeType1 = omniShapeTypeList[Random.Range(0, omniShapeTypeList.Count)];
          omniShapeTypeList.Remove(omniShapeType1);
          num1 = this.m_shapeAmount / 2;
          break;
        case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheNotColor:
          flag1 = true;
          flag3 = true;
          omniShapeColor = omniShapeColorList[Random.Range(0, omniShapeColorList.Count)];
          omniShapeColorList.Remove(omniShapeColor);
          num1 = this.m_shapeAmount / 2;
          break;
        case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheNotShape:
          flag2 = true;
          flag3 = true;
          omniShapeType1 = omniShapeTypeList[Random.Range(0, omniShapeTypeList.Count)];
          omniShapeTypeList.Remove(omniShapeType1);
          num1 = this.m_shapeAmount / 2;
          break;
        case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheNotColorShape:
          flag1 = true;
          flag2 = true;
          flag3 = true;
          omniShapeColor = omniShapeColorList[Random.Range(0, omniShapeColorList.Count)];
          omniShapeColorList.Remove(omniShapeColor);
          omniShapeType1 = omniShapeTypeList[Random.Range(0, omniShapeTypeList.Count)];
          omniShapeTypeList.Remove(omniShapeType1);
          num1 = this.m_shapeAmount / 2;
          break;
      }
      for (int index = 0; index < this.m_shapeAmount; ++index)
      {
        bool isCorrectTarget = false;
        OmniSpawnDef_Shape.OmniShapeType omniShapeType2 = omniShapeTypeList[Random.Range(0, omniShapeTypeList.Count)];
        OmniSpawnDef_Shape.OmniShapeColor color = omniShapeColorList[Random.Range(0, omniShapeColorList.Count)];
        if (flag2 && index < num1)
        {
          omniShapeType2 = omniShapeType1;
          isCorrectTarget = true;
        }
        if (flag1 && index < num1)
        {
          color = omniShapeColor;
          isCorrectTarget = true;
        }
        OmniShape component = Object.Instantiate<GameObject>(this.ShapePrefabs[(int) omniShapeType2], this.transform.position, Quaternion.identity).GetComponent<OmniShape>();
        if (flag3)
          component.Init(this, !isCorrectTarget, color);
        else
          component.Init(this, isCorrectTarget, color);
        this.m_activeShapes.Add(component);
      }
      if (flag3)
        num1 = this.m_shapeAmount - this.m_shapeAmount / 2;
      int num2 = Random.Range(1, 4);
      for (int index1 = 0; index1 < num2; ++index1)
      {
        for (int index2 = this.m_activeShapes.Count - 1; index2 > 0; --index2)
        {
          int index3 = Random.Range(0, index2);
          OmniShape activeShape = this.m_activeShapes[index2];
          this.m_activeShapes[index2] = this.m_activeShapes[index3];
          this.m_activeShapes[index3] = activeShape;
        }
      }
      for (int index = 0; index < this.m_activeShapes.Count; ++index)
      {
        Vector3 zero = Vector3.zero;
        zero.z = this.GetRange();
        zero.y = 1.25f;
        float num3 = (float) (this.m_shapeAmount - 1) * 2.2f;
        zero.x = (float) ((double) index * 2.20000004768372 - (double) num3 * 0.5);
        this.m_activeShapes[index].transform.position = zero;
        this.m_activeShapes[index].transform.rotation = Quaternion.LookRotation(-Vector3.forward, Vector3.up);
      }
      string str1 = "Shoot ";
      string str2;
      bool flag4;
      if (this.m_instructions[this.m_curInstruction] == OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheColor || this.m_instructions[this.m_curInstruction] == OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheColorShape || this.m_instructions[this.m_curInstruction] == OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheShape)
      {
        str2 = str1 + "all of the ";
        flag4 = true;
      }
      else
      {
        str2 = str1 + "the ";
        flag4 = false;
      }
      if (flag3)
      {
        if (flag1 && flag2)
        {
          string hexColor1 = this.HexColors[(int) omniShapeColor];
          string hexColor2 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
          string hexColor3 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
          switch (Random.Range(0, 3))
          {
            case 0:
              str2 = str2 + "Shapes that are not " + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color> " + "<color=#" + hexColor1 + ">" + omniShapeType1.ToString() + "s </color> ";
              break;
            case 1:
              str2 = str2 + "Shapes that are not " + "<color=#" + hexColor1 + ">" + omniShapeColor.ToString() + "</color> " + "<color=#" + hexColor2 + ">" + omniShapeType1.ToString() + "s </color> ";
              break;
            case 2:
              str2 = str2 + "Shapes that are not " + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color> " + "<color=#" + hexColor3 + ">" + omniShapeType1.ToString() + "s </color> ";
              break;
          }
        }
        else if (flag1)
        {
          int num3 = Random.Range(0, 3);
          string hexColor1 = this.HexColors[(int) omniShapeColor];
          string hexColor2 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
          string hexColor3 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
          switch (num3)
          {
            case 0:
              str2 = str2 + "<color=#" + hexColor1 + ">Shapes </color> " + "that are not " + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color>";
              break;
            case 1:
              str2 = str2 + "<color=#" + hexColor2 + ">Shapes </color> " + "that are not " + "<color=#" + hexColor1 + ">" + omniShapeColor.ToString() + "</color>";
              break;
            case 2:
              str2 = str2 + "<color=#" + hexColor3 + ">Shapes </color> " + "that are not " + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color>";
              break;
          }
        }
        else if (flag2)
        {
          string hexColor1 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
          string hexColor2 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
          switch (Random.Range(0, 2))
          {
            case 0:
              str2 = str2 + "<color=#" + hexColor1 + ">Shapes </color> " + "that are not " + "<color=#" + hexColor1 + ">" + omniShapeType1.ToString() + "s</color> ";
              break;
            case 1:
              str2 = str2 + "<color=#" + hexColor1 + ">Shapes </color> " + "that are not " + "<color=#" + hexColor2 + ">" + omniShapeType1.ToString() + "s</color> ";
              break;
          }
        }
      }
      else if (flag1 && flag2)
      {
        string hexColor1 = this.HexColors[(int) omniShapeColor];
        string hexColor2 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
        string hexColor3 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
        string hexColor4 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
        switch (Random.Range(0, 6))
        {
          case 0:
            string str3 = str2 + "<color=#" + hexColor1 + ">" + omniShapeColor.ToString() + "</color> ";
            if (flag4)
            {
              str2 = str3 + "<color=#" + hexColor2 + ">" + omniShapeType1.ToString() + "s</color> ";
              break;
            }
            str2 = str3 + "<color=#" + hexColor2 + ">" + omniShapeType1.ToString() + "</color> ";
            break;
          case 1:
            string str4 = str2 + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color> ";
            if (flag4)
            {
              str2 = str4 + "<color=#" + hexColor1 + ">" + omniShapeType1.ToString() + "s</color> ";
              break;
            }
            str2 = str4 + "<color=#" + hexColor1 + ">" + omniShapeType1.ToString() + "</color> ";
            break;
          case 2:
            string str5 = str2 + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color> ";
            if (flag4)
            {
              str2 = str5 + "<color=#" + hexColor3 + ">" + omniShapeType1.ToString() + "s</color> ";
              break;
            }
            str2 = str5 + "<color=#" + hexColor3 + ">" + omniShapeType1.ToString() + "</color> ";
            break;
          case 3:
            string str6;
            if (flag4)
              str6 = str2 + "<color=#" + hexColor1 + ">" + omniShapeType1.ToString() + "s </color> " + "that are ";
            else
              str6 = str2 + "<color=#" + hexColor1 + ">" + omniShapeType1.ToString() + " </color> " + "that is ";
            str2 = str6 + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color> ";
            break;
          case 4:
            string str7;
            if (flag4)
              str7 = str2 + "<color=#" + hexColor2 + ">" + omniShapeType1.ToString() + "s </color> " + "that are ";
            else
              str7 = str2 + "<color=#" + hexColor2 + ">" + omniShapeType1.ToString() + " </color> " + "that is ";
            str2 = str7 + "<color=#" + hexColor1 + ">" + omniShapeColor.ToString() + "</color> ";
            break;
          case 5:
            string str8;
            if (flag4)
              str8 = str2 + "<color=#" + hexColor2 + ">" + omniShapeType1.ToString() + "s </color> " + "that are ";
            else
              str8 = str2 + "<color=#" + hexColor2 + ">" + omniShapeType1.ToString() + " </color> " + "that is ";
            str2 = str8 + "<color=#" + hexColor3 + ">" + omniShapeColor.ToString() + "</color> ";
            break;
        }
      }
      else if (flag1)
      {
        string hexColor1 = this.HexColors[(int) omniShapeColor];
        string hexColor2 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
        string hexColor3 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
        switch (Random.Range(0, 7))
        {
          case 0:
            string str3 = str2 + "<color=#" + hexColor1 + ">" + omniShapeColor.ToString() + "</color> ";
            str2 = !flag4 ? str3 + "Shape" : str3 + "Shapes";
            break;
          case 1:
            string str4 = str2 + "<color=#" + hexColor1 + ">" + omniShapeColor.ToString() + "</color> ";
            str2 = !flag4 ? str4 + "<color=#" + hexColor2 + ">Shape</color> " : str4 + "<color=#" + hexColor2 + ">Shapes</color> ";
            break;
          case 2:
            string str5 = str2 + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color> ";
            str2 = !flag4 ? str5 + "<color=#" + hexColor1 + ">Shape</color> " : str5 + "<color=#" + hexColor1 + ">Shapes</color> ";
            break;
          case 3:
            string str6 = str2 + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color> ";
            str2 = !flag4 ? str6 + "<color=#" + hexColor3 + ">Shape</color> " : str6 + "<color=#" + hexColor3 + ">Shapes</color> ";
            break;
          case 4:
            str2 = (!flag4 ? str2 + "<color=#" + hexColor2 + ">Shape </color> " + "that is " : str2 + "<color=#" + hexColor2 + ">Shapes </color> " + "that are ") + "<color=#" + hexColor1 + ">" + omniShapeColor.ToString() + "</color>";
            break;
          case 5:
            str2 = (!flag4 ? str2 + "<color=#" + hexColor1 + ">Shape </color> " + "that is " : str2 + "<color=#" + hexColor1 + ">Shapes </color> " + "that are ") + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color>";
            break;
          case 6:
            str2 = (!flag4 ? str2 + "<color=#" + hexColor3 + ">Shape </color> " + "that is " : str2 + "<color=#" + hexColor3 + ">Shapes </color> " + "that are ") + "<color=#" + hexColor2 + ">" + omniShapeColor.ToString() + "</color>";
            break;
        }
      }
      else if (flag2)
      {
        string hexColor1 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
        string hexColor2 = this.HexColors[(int) omniShapeColorList[Random.Range(0, omniShapeColorList.Count)]];
        switch (Random.Range(0, 2))
        {
          case 0:
            if (flag4)
            {
              str2 = str2 + "<color=#" + hexColor1 + ">" + omniShapeType1.ToString() + "s</color> ";
              break;
            }
            str2 = str2 + "<color=#" + hexColor1 + ">" + omniShapeType1.ToString() + "</color> ";
            break;
          case 1:
            if (flag4)
            {
              str2 = str2 + "<color=#" + hexColor1 + ">Shapes </color> " + "that are " + "<color=#" + hexColor2 + ">" + omniShapeType1.ToString() + "s</color> ";
              break;
            }
            str2 = str2 + "<color=#" + hexColor1 + ">Shape </color> " + "that is a " + "<color=#" + hexColor2 + ">" + omniShapeType1.ToString() + "</color> ";
            break;
        }
      }
      this.m_panel.instructiontext.text = str2;
      this.m_correctShapesLeft = num1;
      this.m_incorrectShapesLeft = this.m_shapeAmount - this.m_correctShapesLeft;
      ++this.m_curInstruction;
    }
  }
}
