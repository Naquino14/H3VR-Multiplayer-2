using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FistVR
{
	public class OmniGrid : MonoBehaviour
	{
		public enum GridCellState
		{
			Active,
			Growing,
			Shrinking
		}

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

		private GridCellState m_gridCellState = GridCellState.Growing;

		private float m_cellScale;

		private Vector3 m_cellScale_Large = new Vector3(0.01f, 0.01f, 0.01f);

		private Vector3 m_cellScale_Small = new Vector3(0.0001f, 0.0001f, 0.0001f);

		private int m_number1;

		private int m_number2;

		public void Init(OmniSpawner_Grid spawner, Vector3 startPos, Vector3 endPos, Quaternion startRot, Quaternion endRot, OmniSpawnDef_Grid.GridConfiguration config, List<OmniSpawnDef_Grid.GridInstruction> instructions)
		{
			m_spawner = spawner;
			m_startPos = startPos;
			m_endPos = endPos;
			m_startRot = startRot;
			m_endRot = endRot;
			m_isMovingIntoPosition = true;
			m_config = config;
			m_instructions = instructions;
			m_isMovingIntoPosition = true;
			for (int i = 0; i < Cells.Length; i++)
			{
				Cells[i].gameObject.SetActive(value: false);
				Cells[i].transform.localScale = m_cellScale_Small;
			}
		}

		private void Update()
		{
			if (m_isMovingIntoPosition)
			{
				if (m_moveLerp < 1f)
				{
					m_moveLerp += Time.deltaTime * 5f;
				}
				else
				{
					m_moveLerp = 1f;
				}
				base.transform.position = Vector3.Lerp(m_startPos, m_endPos, m_moveLerp);
				base.transform.rotation = Quaternion.Slerp(m_startRot, m_endRot, m_moveLerp);
				if (m_moveLerp >= 1f)
				{
					m_isMovingIntoPosition = false;
					SetCellConfiguration(m_config);
					ConfigureForInstruction(m_instructions[0]);
				}
			}
			if (m_isMovingAway)
			{
				if (m_moveLerp < 1f)
				{
					m_moveLerp += Time.deltaTime * 5f;
				}
				else
				{
					m_moveLerp = 1f;
				}
				base.transform.position = Vector3.Lerp(m_endPos, m_startPos, m_moveLerp);
				base.transform.rotation = Quaternion.Slerp(m_endRot, m_startRot, m_moveLerp);
				if (m_moveLerp >= 1f)
				{
					m_isMovingAway = false;
					Object.Destroy(base.gameObject);
				}
			}
			switch (m_gridCellState)
			{
			case GridCellState.Growing:
			{
				bool flag2 = false;
				if (m_cellScale < 1f)
				{
					m_cellScale += Time.deltaTime * 5f;
				}
				else
				{
					m_cellScale = 1f;
					flag2 = true;
				}
				for (int j = 0; j < Cells.Length; j++)
				{
					Cells[j].transform.localScale = Vector3.Lerp(m_cellScale_Small, m_cellScale_Large, m_cellScale);
					if (flag2)
					{
						Cells[j].SetCanBeShot(b: true);
					}
				}
				if (flag2)
				{
					m_gridCellState = GridCellState.Active;
				}
				break;
			}
			case GridCellState.Shrinking:
			{
				bool flag = false;
				if (m_cellScale > 0f)
				{
					m_cellScale -= Time.deltaTime * 5f;
				}
				else
				{
					m_cellScale = 0f;
					flag = true;
				}
				for (int i = 0; i < Cells.Length; i++)
				{
					Cells[i].transform.localScale = Vector3.Lerp(m_cellScale_Small, m_cellScale_Large, m_cellScale);
				}
				if (flag)
				{
					m_gridCellState = GridCellState.Growing;
				}
				break;
			}
			}
		}

		public bool InputNumber(int i, OmniGridCell cell)
		{
			int item = 9;
			switch (Size)
			{
			case OmniSpawnDef_Grid.GridSize.To9:
				item = 9;
				break;
			case OmniSpawnDef_Grid.GridSize.To16:
				item = 16;
				break;
			case OmniSpawnDef_Grid.GridSize.To25:
				item = 25;
				break;
			}
			bool flag = false;
			bool flag2 = false;
			switch (m_instructions[m_instructionIndex])
			{
			case OmniSpawnDef_Grid.GridInstruction.Addition:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Large);
					flag = true;
					flag2 = true;
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.Subtraction:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Large);
					flag = true;
					flag2 = true;
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.Multiplication:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Large);
					flag = true;
					flag2 = true;
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.Division:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Large);
					flag = true;
					flag2 = true;
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.CountUp:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Small);
					flag = true;
					if (m_desiredNumbers.Contains(item))
					{
						flag2 = true;
					}
					else
					{
						m_desiredNumbers[0]++;
					}
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.CountDown:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Small);
					flag = true;
					if (m_desiredNumbers.Contains(1))
					{
						flag2 = true;
					}
					else
					{
						m_desiredNumbers[0]--;
					}
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.Odds:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Small);
					flag = true;
					m_desiredNumbers.Remove(i);
				}
				if (m_desiredNumbers.Count == 0)
				{
					flag2 = true;
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.Evens:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Small);
					flag = true;
					m_desiredNumbers.Remove(i);
				}
				if (m_desiredNumbers.Count == 0)
				{
					flag2 = true;
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.Multiples3:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Small);
					flag = true;
					m_desiredNumbers.Remove(i);
				}
				if (m_desiredNumbers.Count == 0)
				{
					flag2 = true;
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.Multiples4:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Small);
					flag = true;
					m_desiredNumbers.Remove(i);
				}
				if (m_desiredNumbers.Count == 0)
				{
					flag2 = true;
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.Primes:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Small);
					flag = true;
					m_desiredNumbers.Remove(i);
				}
				if (m_desiredNumbers.Count == 0)
				{
					flag2 = true;
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.GreaterThan:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Small);
					flag = true;
					m_desiredNumbers.Remove(i);
				}
				if (m_desiredNumbers.Count == 0)
				{
					flag2 = true;
				}
				break;
			case OmniSpawnDef_Grid.GridInstruction.LessThan:
				if (m_desiredNumbers.Contains(i))
				{
					m_spawner.AddPoints(scorePoint_Small);
					flag = true;
					m_desiredNumbers.Remove(i);
				}
				if (m_desiredNumbers.Count == 0)
				{
					flag2 = true;
				}
				break;
			}
			if (flag)
			{
				m_spawner.Invoke("PlaySuccessSound", 0.15f);
				cell.gameObject.SetActive(value: false);
			}
			else
			{
				m_spawner.Invoke("PlayFailureSound", 0.15f);
				m_spawner.AddPoints(scorePoint_Incorrect);
			}
			if (flag2)
			{
				AdvanceToNextInstruction();
			}
			return flag;
		}

		private void SetCellConfiguration(OmniSpawnDef_Grid.GridConfiguration config)
		{
			for (int i = 0; i < Cells.Length; i++)
			{
				Cells[i].gameObject.SetActive(value: true);
			}
			int num = 9;
			switch (Size)
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
			switch (config)
			{
			case OmniSpawnDef_Grid.GridConfiguration.Ascending:
			{
				for (int l = 0; l < num; l++)
				{
					Cells[l].SetState(l + 1, (l + 1).ToString());
				}
				break;
			}
			case OmniSpawnDef_Grid.GridConfiguration.Descending:
			{
				for (int m = 0; m < num; m++)
				{
					Cells[m].SetState(num - m, (num - m).ToString());
				}
				break;
			}
			case OmniSpawnDef_Grid.GridConfiguration.Shuffled:
			{
				int[] array = new int[num];
				for (int j = 0; j < num; j++)
				{
					array[j] = j + 1;
				}
				array = ShuffleNums(array);
				for (int k = 0; k < num; k++)
				{
					Cells[k].SetState(array[k], array[k].ToString());
				}
				break;
			}
			}
		}

		private void AdvanceToNextInstruction()
		{
			if (m_instructionIndex < m_instructions.Count - 1)
			{
				m_instructionIndex++;
				SetCellConfiguration(m_config);
				ConfigureForInstruction(m_instructions[m_instructionIndex]);
				m_gridCellState = GridCellState.Shrinking;
				for (int i = 0; i < Cells.Length; i++)
				{
					Cells[i].SetCanBeShot(b: false);
				}
			}
			else
			{
				m_spawner.GridIsFinished();
				DespawnGrid();
			}
		}

		private void ConfigureForInstruction(OmniSpawnDef_Grid.GridInstruction instruction)
		{
			m_desiredNumbers.Clear();
			int num = 9;
			int num2 = 1;
			int num3 = 1;
			int num4 = 1;
			switch (Size)
			{
			case OmniSpawnDef_Grid.GridSize.To9:
				num = 9;
				num4 = 1;
				break;
			case OmniSpawnDef_Grid.GridSize.To16:
				num = 16;
				num4 = 2;
				break;
			case OmniSpawnDef_Grid.GridSize.To25:
				num = 25;
				num4 = 3;
				break;
			}
			switch (instruction)
			{
			case OmniSpawnDef_Grid.GridInstruction.CountUp:
				m_desiredNumbers.Add(1);
				break;
			case OmniSpawnDef_Grid.GridInstruction.CountDown:
				m_desiredNumbers.Add(num);
				break;
			case OmniSpawnDef_Grid.GridInstruction.Addition:
				m_number1 = Random.Range(1, num - 2);
				num2 = 1;
				num3 = num - m_number1;
				m_number2 = Random.Range(num2, num3 + 1);
				m_desiredNumbers.Add(m_number1 + m_number2);
				break;
			case OmniSpawnDef_Grid.GridInstruction.Subtraction:
				m_number1 = Random.Range(3, num - 1);
				num2 = 1;
				num3 = m_number1 - 1;
				m_number2 = Random.Range(num2, num3 + 1);
				m_desiredNumbers.Add(m_number1 - m_number2);
				break;
			case OmniSpawnDef_Grid.GridInstruction.Multiplication:
			{
				List<Vector3> multiplicationEquations2 = EquationLists[Random.Range(0, num4)].MultiplicationEquations;
				Vector3 vector2 = multiplicationEquations2[Random.Range(0, multiplicationEquations2.Count)];
				if (Random.value > 0.5f)
				{
					m_number1 = (int)vector2.x;
					m_number2 = (int)vector2.y;
				}
				else
				{
					m_number1 = (int)vector2.y;
					m_number2 = (int)vector2.x;
				}
				m_desiredNumbers.Add((int)vector2.z);
				break;
			}
			case OmniSpawnDef_Grid.GridInstruction.Division:
			{
				List<Vector3> multiplicationEquations = EquationLists[Random.Range(0, num4)].MultiplicationEquations;
				Vector3 vector = multiplicationEquations[Random.Range(0, multiplicationEquations.Count)];
				m_number1 = (int)vector.z;
				if (Random.value > 0.5f)
				{
					m_number2 = (int)vector.x;
					m_desiredNumbers.Add((int)vector.y);
				}
				else
				{
					m_number2 = (int)vector.y;
					m_desiredNumbers.Add((int)vector.x);
				}
				break;
			}
			case OmniSpawnDef_Grid.GridInstruction.Odds:
			{
				for (int num8 = 1; num8 <= num; num8++)
				{
					if (num8 % 2 == 1)
					{
						m_desiredNumbers.Add(num8);
					}
				}
				break;
			}
			case OmniSpawnDef_Grid.GridInstruction.Evens:
			{
				for (int num7 = 1; num7 <= num; num7++)
				{
					if (num7 % 2 == 0)
					{
						m_desiredNumbers.Add(num7);
					}
				}
				break;
			}
			case OmniSpawnDef_Grid.GridInstruction.Multiples3:
			{
				for (int num5 = 0; num5 < num4; num5++)
				{
					for (int num6 = 0; num6 < EquationLists[num5].MultiplesOf3.Count; num6++)
					{
						m_desiredNumbers.Add(EquationLists[num5].MultiplesOf3[num6]);
					}
				}
				break;
			}
			case OmniSpawnDef_Grid.GridInstruction.Multiples4:
			{
				for (int m = 0; m < num4; m++)
				{
					for (int n = 0; n < EquationLists[m].MultiplesOf4.Count; n++)
					{
						m_desiredNumbers.Add(EquationLists[m].MultiplesOf4[n]);
					}
				}
				break;
			}
			case OmniSpawnDef_Grid.GridInstruction.Primes:
			{
				for (int k = 0; k < num4; k++)
				{
					for (int l = 0; l < EquationLists[k].Primes.Count; l++)
					{
						m_desiredNumbers.Add(EquationLists[k].Primes[l]);
					}
				}
				break;
			}
			case OmniSpawnDef_Grid.GridInstruction.GreaterThan:
			{
				m_number1 = Random.Range((int)((float)num * 0.3f), (int)((float)num * 0.5f));
				for (int j = m_number1 + 1; j <= num; j++)
				{
					m_desiredNumbers.Add(j);
				}
				break;
			}
			case OmniSpawnDef_Grid.GridInstruction.LessThan:
			{
				m_number1 = Random.Range((int)((float)num * 0.5f), (int)((float)num * 0.7f));
				for (int i = 1; i < m_number1; i++)
				{
					m_desiredNumbers.Add(i);
				}
				break;
			}
			}
			switch (instruction)
			{
			case OmniSpawnDef_Grid.GridInstruction.CountUp:
				InstructionText.text = "COUNT UP!";
				break;
			case OmniSpawnDef_Grid.GridInstruction.CountDown:
				InstructionText.text = "COUNT DOWN!";
				break;
			case OmniSpawnDef_Grid.GridInstruction.Addition:
				InstructionText.text = m_number1 + " + " + m_number2 + " = ?";
				break;
			case OmniSpawnDef_Grid.GridInstruction.Subtraction:
				InstructionText.text = m_number1 + " - " + m_number2 + " = ?";
				break;
			case OmniSpawnDef_Grid.GridInstruction.Multiplication:
				InstructionText.text = m_number1 + " * " + m_number2 + " = ?";
				break;
			case OmniSpawnDef_Grid.GridInstruction.Division:
				InstructionText.text = m_number1 + " / " + m_number2 + " = ?";
				break;
			case OmniSpawnDef_Grid.GridInstruction.Odds:
				InstructionText.text = "ODDS!";
				break;
			case OmniSpawnDef_Grid.GridInstruction.Evens:
				InstructionText.text = "EVENS!";
				break;
			case OmniSpawnDef_Grid.GridInstruction.Multiples3:
				InstructionText.text = "MULTIPLES OF 3";
				break;
			case OmniSpawnDef_Grid.GridInstruction.Multiples4:
				InstructionText.text = "MULTIPLES OF 4";
				break;
			case OmniSpawnDef_Grid.GridInstruction.Primes:
				InstructionText.text = "PRIMES!";
				break;
			case OmniSpawnDef_Grid.GridInstruction.GreaterThan:
				InstructionText.text = "ALL GREATER THAN " + m_number1;
				break;
			case OmniSpawnDef_Grid.GridInstruction.LessThan:
				InstructionText.text = "ALL LESS THAN " + m_number1;
				break;
			}
		}

		public void DespawnGrid()
		{
			if (m_isMovingAway)
			{
				return;
			}
			InstructionText.text = string.Empty;
			for (int i = 0; i < Cells.Length; i++)
			{
				if (Cells[i] != null)
				{
					Cells[i].gameObject.SetActive(value: false);
				}
			}
			m_moveLerp = 0f;
			m_isMovingAway = true;
		}

		private int[] ShuffleNums(int[] nums)
		{
			for (int num = nums.Length - 1; num > 0; num--)
			{
				int num2 = Random.Range(0, num);
				int num3 = nums[num];
				nums[num] = nums[num2];
				nums[num2] = num3;
			}
			return nums;
		}
	}
}
