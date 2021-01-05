// Decompiled with JetBrains decompiler
// Type: FistVR.OmniGrid
// Assembly: Assembly-CSharp, Version=1.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0E76EF54-A563-4796-92B9-C7AE2338A28D
// Assembly location: D:\SteamLibrary\steamapps\common\H3VR\h3vr_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
  public class OmniGrid : MonoBehaviour
  {
    public OmniSpawnDef_Grid.GridSize Size;
    public OmniGridCell[] Cells;
    public OmniGridEquations[] EquationLists;
    private OmniSpawner_Grid m_spawner;
    private Vector3 m_startPos;
    private Vector3 m_endPos;
    private Quaternion m_startRot;
    private Quaternion m_endRot;
    private bool m_isMovingIntoPosition;
    private bool m_isMovingAway;
    private float m_moveLerp;
    private OmniSpawnDef_Grid.GridConfiguration m_config;
    private List<OmniSpawnDef_Grid.GridInstruction> m_instructions;
    private int m_instructionIndex;
    public Text InstructionText;
    private List<int> m_desiredNumbers = new List<int>();
    private int scorePoint_Large = 30;
    private int scorePoint_Small = 10;
    private int scorePoint_Incorrect = -15;
    private OmniGrid.GridCellState m_gridCellState = OmniGrid.GridCellState.Growing;
    private float m_cellScale;
    private Vector3 m_cellScale_Large = new Vector3(0.01f, 0.01f, 0.01f);
    private Vector3 m_cellScale_Small = new Vector3(0.0001f, 0.0001f, 0.0001f);
    private int m_number1;
    private int m_number2;

    public void Init(
      OmniSpawner_Grid spawner,
      Vector3 startPos,
      Vector3 endPos,
      Quaternion startRot,
      Quaternion endRot,
      OmniSpawnDef_Grid.GridConfiguration config,
      List<OmniSpawnDef_Grid.GridInstruction> instructions)
    {
      this.m_spawner = spawner;
      this.m_startPos = startPos;
      this.m_endPos = endPos;
      this.m_startRot = startRot;
      this.m_endRot = endRot;
      this.m_isMovingIntoPosition = true;
      this.m_config = config;
      this.m_instructions = instructions;
      this.m_isMovingIntoPosition = true;
      for (int index = 0; index < this.Cells.Length; ++index)
      {
        this.Cells[index].gameObject.SetActive(false);
        this.Cells[index].transform.localScale = this.m_cellScale_Small;
      }
    }

    private void Update()
    {
      if (this.m_isMovingIntoPosition)
      {
        if ((double) this.m_moveLerp < 1.0)
          this.m_moveLerp += Time.deltaTime * 5f;
        else
          this.m_moveLerp = 1f;
        this.transform.position = Vector3.Lerp(this.m_startPos, this.m_endPos, this.m_moveLerp);
        this.transform.rotation = Quaternion.Slerp(this.m_startRot, this.m_endRot, this.m_moveLerp);
        if ((double) this.m_moveLerp >= 1.0)
        {
          this.m_isMovingIntoPosition = false;
          this.SetCellConfiguration(this.m_config);
          this.ConfigureForInstruction(this.m_instructions[0]);
        }
      }
      if (this.m_isMovingAway)
      {
        if ((double) this.m_moveLerp < 1.0)
          this.m_moveLerp += Time.deltaTime * 5f;
        else
          this.m_moveLerp = 1f;
        this.transform.position = Vector3.Lerp(this.m_endPos, this.m_startPos, this.m_moveLerp);
        this.transform.rotation = Quaternion.Slerp(this.m_endRot, this.m_startRot, this.m_moveLerp);
        if ((double) this.m_moveLerp >= 1.0)
        {
          this.m_isMovingAway = false;
          Object.Destroy((Object) this.gameObject);
        }
      }
      switch (this.m_gridCellState)
      {
        case OmniGrid.GridCellState.Growing:
          bool flag1 = false;
          if ((double) this.m_cellScale < 1.0)
          {
            this.m_cellScale += Time.deltaTime * 5f;
          }
          else
          {
            this.m_cellScale = 1f;
            flag1 = true;
          }
          for (int index = 0; index < this.Cells.Length; ++index)
          {
            this.Cells[index].transform.localScale = Vector3.Lerp(this.m_cellScale_Small, this.m_cellScale_Large, this.m_cellScale);
            if (flag1)
              this.Cells[index].SetCanBeShot(true);
          }
          if (!flag1)
            break;
          this.m_gridCellState = OmniGrid.GridCellState.Active;
          break;
        case OmniGrid.GridCellState.Shrinking:
          bool flag2 = false;
          if ((double) this.m_cellScale > 0.0)
          {
            this.m_cellScale -= Time.deltaTime * 5f;
          }
          else
          {
            this.m_cellScale = 0.0f;
            flag2 = true;
          }
          for (int index = 0; index < this.Cells.Length; ++index)
            this.Cells[index].transform.localScale = Vector3.Lerp(this.m_cellScale_Small, this.m_cellScale_Large, this.m_cellScale);
          if (!flag2)
            break;
          this.m_gridCellState = OmniGrid.GridCellState.Growing;
          break;
      }
    }

    public bool InputNumber(int i, OmniGridCell cell)
    {
      int num = 9;
      switch (this.Size)
      {
        case OmniSpawnDef_Grid.GridSize.To9:
          num = 9;
          break;
        case OmniSpawnDef_Grid.GridSize.To16:
          num = 16;
          break;
        case OmniSpawnDef_Grid.GridSize.To25:
          num = 25;
          break;
      }
      bool flag1 = false;
      bool flag2 = false;
      switch (this.m_instructions[this.m_instructionIndex])
      {
        case OmniSpawnDef_Grid.GridInstruction.CountUp:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Small);
            flag1 = true;
            if (this.m_desiredNumbers.Contains(num))
            {
              flag2 = true;
              break;
            }
            List<int> desiredNumbers;
            (desiredNumbers = this.m_desiredNumbers)[0] = desiredNumbers[0] + 1;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.CountDown:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Small);
            flag1 = true;
            if (this.m_desiredNumbers.Contains(1))
            {
              flag2 = true;
              break;
            }
            List<int> desiredNumbers;
            (desiredNumbers = this.m_desiredNumbers)[0] = desiredNumbers[0] - 1;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Addition:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Large);
            flag1 = true;
            flag2 = true;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Subtraction:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Large);
            flag1 = true;
            flag2 = true;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Multiplication:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Large);
            flag1 = true;
            flag2 = true;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Division:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Large);
            flag1 = true;
            flag2 = true;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Odds:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Small);
            flag1 = true;
            this.m_desiredNumbers.Remove(i);
          }
          if (this.m_desiredNumbers.Count == 0)
          {
            flag2 = true;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Evens:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Small);
            flag1 = true;
            this.m_desiredNumbers.Remove(i);
          }
          if (this.m_desiredNumbers.Count == 0)
          {
            flag2 = true;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Multiples3:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Small);
            flag1 = true;
            this.m_desiredNumbers.Remove(i);
          }
          if (this.m_desiredNumbers.Count == 0)
          {
            flag2 = true;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Multiples4:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Small);
            flag1 = true;
            this.m_desiredNumbers.Remove(i);
          }
          if (this.m_desiredNumbers.Count == 0)
          {
            flag2 = true;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Primes:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Small);
            flag1 = true;
            this.m_desiredNumbers.Remove(i);
          }
          if (this.m_desiredNumbers.Count == 0)
          {
            flag2 = true;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.GreaterThan:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Small);
            flag1 = true;
            this.m_desiredNumbers.Remove(i);
          }
          if (this.m_desiredNumbers.Count == 0)
          {
            flag2 = true;
            break;
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.LessThan:
          if (this.m_desiredNumbers.Contains(i))
          {
            this.m_spawner.AddPoints(this.scorePoint_Small);
            flag1 = true;
            this.m_desiredNumbers.Remove(i);
          }
          if (this.m_desiredNumbers.Count == 0)
          {
            flag2 = true;
            break;
          }
          break;
      }
      if (flag1)
      {
        this.m_spawner.Invoke("PlaySuccessSound", 0.15f);
        cell.gameObject.SetActive(false);
      }
      else
      {
        this.m_spawner.Invoke("PlayFailureSound", 0.15f);
        this.m_spawner.AddPoints(this.scorePoint_Incorrect);
      }
      if (flag2)
        this.AdvanceToNextInstruction();
      return flag1;
    }

    private void SetCellConfiguration(OmniSpawnDef_Grid.GridConfiguration config)
    {
      for (int index = 0; index < this.Cells.Length; ++index)
        this.Cells[index].gameObject.SetActive(true);
      int length = 9;
      switch (this.Size)
      {
        case OmniSpawnDef_Grid.GridSize.To9:
          length = 9;
          break;
        case OmniSpawnDef_Grid.GridSize.To16:
          length = 16;
          break;
        case OmniSpawnDef_Grid.GridSize.To25:
          length = 25;
          break;
      }
      switch (config)
      {
        case OmniSpawnDef_Grid.GridConfiguration.Ascending:
          for (int index = 0; index < length; ++index)
            this.Cells[index].SetState(index + 1, (index + 1).ToString());
          break;
        case OmniSpawnDef_Grid.GridConfiguration.Descending:
          for (int index = 0; index < length; ++index)
            this.Cells[index].SetState(length - index, (length - index).ToString());
          break;
        case OmniSpawnDef_Grid.GridConfiguration.Shuffled:
          int[] nums = new int[length];
          for (int index = 0; index < length; ++index)
            nums[index] = index + 1;
          int[] numArray = this.ShuffleNums(nums);
          for (int index = 0; index < length; ++index)
            this.Cells[index].SetState(numArray[index], numArray[index].ToString());
          break;
      }
    }

    private void AdvanceToNextInstruction()
    {
      if (this.m_instructionIndex < this.m_instructions.Count - 1)
      {
        ++this.m_instructionIndex;
        this.SetCellConfiguration(this.m_config);
        this.ConfigureForInstruction(this.m_instructions[this.m_instructionIndex]);
        this.m_gridCellState = OmniGrid.GridCellState.Shrinking;
        for (int index = 0; index < this.Cells.Length; ++index)
          this.Cells[index].SetCanBeShot(false);
      }
      else
      {
        this.m_spawner.GridIsFinished();
        this.DespawnGrid();
      }
    }

    private void ConfigureForInstruction(OmniSpawnDef_Grid.GridInstruction instruction)
    {
      this.m_desiredNumbers.Clear();
      int num = 9;
      int max = 1;
      switch (this.Size)
      {
        case OmniSpawnDef_Grid.GridSize.To9:
          num = 9;
          max = 1;
          break;
        case OmniSpawnDef_Grid.GridSize.To16:
          num = 16;
          max = 2;
          break;
        case OmniSpawnDef_Grid.GridSize.To25:
          num = 25;
          max = 3;
          break;
      }
      switch (instruction)
      {
        case OmniSpawnDef_Grid.GridInstruction.CountUp:
          this.m_desiredNumbers.Add(1);
          break;
        case OmniSpawnDef_Grid.GridInstruction.CountDown:
          this.m_desiredNumbers.Add(num);
          break;
        case OmniSpawnDef_Grid.GridInstruction.Addition:
          this.m_number1 = Random.Range(1, num - 2);
          this.m_number2 = Random.Range(1, num - this.m_number1 + 1);
          this.m_desiredNumbers.Add(this.m_number1 + this.m_number2);
          break;
        case OmniSpawnDef_Grid.GridInstruction.Subtraction:
          this.m_number1 = Random.Range(3, num - 1);
          this.m_number2 = Random.Range(1, this.m_number1 - 1 + 1);
          this.m_desiredNumbers.Add(this.m_number1 - this.m_number2);
          break;
        case OmniSpawnDef_Grid.GridInstruction.Multiplication:
          List<Vector3> multiplicationEquations1 = this.EquationLists[Random.Range(0, max)].MultiplicationEquations;
          Vector3 vector3_1 = multiplicationEquations1[Random.Range(0, multiplicationEquations1.Count)];
          if ((double) Random.value > 0.5)
          {
            this.m_number1 = (int) vector3_1.x;
            this.m_number2 = (int) vector3_1.y;
          }
          else
          {
            this.m_number1 = (int) vector3_1.y;
            this.m_number2 = (int) vector3_1.x;
          }
          this.m_desiredNumbers.Add((int) vector3_1.z);
          break;
        case OmniSpawnDef_Grid.GridInstruction.Division:
          List<Vector3> multiplicationEquations2 = this.EquationLists[Random.Range(0, max)].MultiplicationEquations;
          Vector3 vector3_2 = multiplicationEquations2[Random.Range(0, multiplicationEquations2.Count)];
          this.m_number1 = (int) vector3_2.z;
          if ((double) Random.value > 0.5)
          {
            this.m_number2 = (int) vector3_2.x;
            this.m_desiredNumbers.Add((int) vector3_2.y);
            break;
          }
          this.m_number2 = (int) vector3_2.y;
          this.m_desiredNumbers.Add((int) vector3_2.x);
          break;
        case OmniSpawnDef_Grid.GridInstruction.Odds:
          for (int index = 1; index <= num; ++index)
          {
            if (index % 2 == 1)
              this.m_desiredNumbers.Add(index);
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Evens:
          for (int index = 1; index <= num; ++index)
          {
            if (index % 2 == 0)
              this.m_desiredNumbers.Add(index);
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Multiples3:
          for (int index1 = 0; index1 < max; ++index1)
          {
            for (int index2 = 0; index2 < this.EquationLists[index1].MultiplesOf3.Count; ++index2)
              this.m_desiredNumbers.Add(this.EquationLists[index1].MultiplesOf3[index2]);
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Multiples4:
          for (int index1 = 0; index1 < max; ++index1)
          {
            for (int index2 = 0; index2 < this.EquationLists[index1].MultiplesOf4.Count; ++index2)
              this.m_desiredNumbers.Add(this.EquationLists[index1].MultiplesOf4[index2]);
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.Primes:
          for (int index1 = 0; index1 < max; ++index1)
          {
            for (int index2 = 0; index2 < this.EquationLists[index1].Primes.Count; ++index2)
              this.m_desiredNumbers.Add(this.EquationLists[index1].Primes[index2]);
          }
          break;
        case OmniSpawnDef_Grid.GridInstruction.GreaterThan:
          this.m_number1 = Random.Range((int) ((double) num * 0.300000011920929), (int) ((double) num * 0.5));
          for (int index = this.m_number1 + 1; index <= num; ++index)
            this.m_desiredNumbers.Add(index);
          break;
        case OmniSpawnDef_Grid.GridInstruction.LessThan:
          this.m_number1 = Random.Range((int) ((double) num * 0.5), (int) ((double) num * 0.699999988079071));
          for (int index = 1; index < this.m_number1; ++index)
            this.m_desiredNumbers.Add(index);
          break;
      }
      switch (instruction)
      {
        case OmniSpawnDef_Grid.GridInstruction.CountUp:
          this.InstructionText.text = "COUNT UP!";
          break;
        case OmniSpawnDef_Grid.GridInstruction.CountDown:
          this.InstructionText.text = "COUNT DOWN!";
          break;
        case OmniSpawnDef_Grid.GridInstruction.Addition:
          this.InstructionText.text = this.m_number1.ToString() + " + " + this.m_number2.ToString() + " = ?";
          break;
        case OmniSpawnDef_Grid.GridInstruction.Subtraction:
          this.InstructionText.text = this.m_number1.ToString() + " - " + this.m_number2.ToString() + " = ?";
          break;
        case OmniSpawnDef_Grid.GridInstruction.Multiplication:
          this.InstructionText.text = this.m_number1.ToString() + " * " + this.m_number2.ToString() + " = ?";
          break;
        case OmniSpawnDef_Grid.GridInstruction.Division:
          this.InstructionText.text = this.m_number1.ToString() + " / " + this.m_number2.ToString() + " = ?";
          break;
        case OmniSpawnDef_Grid.GridInstruction.Odds:
          this.InstructionText.text = "ODDS!";
          break;
        case OmniSpawnDef_Grid.GridInstruction.Evens:
          this.InstructionText.text = "EVENS!";
          break;
        case OmniSpawnDef_Grid.GridInstruction.Multiples3:
          this.InstructionText.text = "MULTIPLES OF 3";
          break;
        case OmniSpawnDef_Grid.GridInstruction.Multiples4:
          this.InstructionText.text = "MULTIPLES OF 4";
          break;
        case OmniSpawnDef_Grid.GridInstruction.Primes:
          this.InstructionText.text = "PRIMES!";
          break;
        case OmniSpawnDef_Grid.GridInstruction.GreaterThan:
          this.InstructionText.text = "ALL GREATER THAN " + this.m_number1.ToString();
          break;
        case OmniSpawnDef_Grid.GridInstruction.LessThan:
          this.InstructionText.text = "ALL LESS THAN " + this.m_number1.ToString();
          break;
      }
    }

    public void DespawnGrid()
    {
      if (this.m_isMovingAway)
        return;
      this.InstructionText.text = string.Empty;
      for (int index = 0; index < this.Cells.Length; ++index)
      {
        if ((Object) this.Cells[index] != (Object) null)
          this.Cells[index].gameObject.SetActive(false);
      }
      this.m_moveLerp = 0.0f;
      this.m_isMovingAway = true;
    }

    private int[] ShuffleNums(int[] nums)
    {
      for (int max = nums.Length - 1; max > 0; --max)
      {
        int index = Random.Range(0, max);
        int num = nums[max];
        nums[max] = nums[index];
        nums[index] = num;
      }
      return nums;
    }

    public enum GridCellState
    {
      Active,
      Growing,
      Shrinking,
    }
  }
}
