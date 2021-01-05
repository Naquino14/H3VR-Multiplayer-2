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

		public List<string> HexColors = new List<string>
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
			m_def = Def as OmniSpawnDef_Shape;
			m_instructions = m_def.Instructions;
			m_shapeAmount = m_def.ShapeAmount;
		}

		public override void BeginSpawning()
		{
			base.BeginSpawning();
			m_canPresent = true;
		}

		public override void EndSpawning()
		{
			base.EndSpawning();
			m_canPresent = false;
		}

		public override void Activate()
		{
			base.Activate();
		}

		public override int Deactivate()
		{
			DespawnActiveShapes();
			Object.Destroy(m_panel.gameObject);
			m_panel = null;
			return base.Deactivate();
		}

		private void Update()
		{
			UpdateMe();
		}

		private void UpdateMe()
		{
			if (m_isConfigured)
			{
				switch (m_state)
				{
				case SpawnerState.Activated:
					SpawningLoop();
					break;
				case SpawnerState.Activating:
					Activating();
					break;
				case SpawnerState.Deactivating:
					Deactivating();
					break;
				}
			}
		}

		public void ShapeStruck(bool wasCorrect)
		{
			if (wasCorrect)
			{
				m_correctShapesLeft--;
				AddPoints(100);
				AudSource.PlayOneShot(AudClip_Success, 1f);
			}
			else
			{
				m_incorrectShapesLeft--;
				AddPoints(-120);
				AudSource.PlayOneShot(AudClip_Failure, 1f);
			}
		}

		private void DespawnActiveShapes()
		{
			for (int i = 0; i < m_activeShapes.Count; i++)
			{
				if (m_activeShapes[i] != null)
				{
					m_activeShapes[i].TurnOff();
				}
			}
			m_activeShapes.Clear();
		}

		private void SpawningLoop()
		{
			if (m_canPresent && m_correctShapesLeft == 0)
			{
				DespawnActiveShapes();
				if (m_curInstruction >= m_def.Instructions.Count)
				{
					m_isDoneSpawning = true;
					m_isReadyForWaveEnd = true;
				}
				else
				{
					SpawnInstructions();
					SpawnShapeSet();
				}
			}
		}

		private void SpawnInstructions()
		{
			if (m_panel == null)
			{
				GameObject gameObject = Object.Instantiate(position: new Vector3(0f, 3f, GetRange()), original: InstructionPrefab, rotation: Quaternion.identity);
				m_panel = gameObject.GetComponent<OmniShapeInstructionPanel>();
			}
		}

		private void SpawnShapeSet()
		{
			int num = 1;
			AudSource.PlayOneShot(AudClip_Spawn, 1f);
			bool flag = false;
			OmniSpawnDef_Shape.OmniShapeColor omniShapeColor = OmniSpawnDef_Shape.OmniShapeColor.Red;
			List<OmniSpawnDef_Shape.OmniShapeColor> list = new List<OmniSpawnDef_Shape.OmniShapeColor>();
			bool flag2 = false;
			OmniSpawnDef_Shape.OmniShapeType omniShapeType = OmniSpawnDef_Shape.OmniShapeType.Circle;
			List<OmniSpawnDef_Shape.OmniShapeType> list2 = new List<OmniSpawnDef_Shape.OmniShapeType>();
			bool flag3 = false;
			for (int i = 0; i < 8; i++)
			{
				list.Add((OmniSpawnDef_Shape.OmniShapeColor)i);
				list2.Add((OmniSpawnDef_Shape.OmniShapeType)i);
			}
			switch (m_instructions[m_curInstruction])
			{
			case OmniSpawnDef_Shape.ShapeInstruction.ShootTheColor:
				flag = true;
				omniShapeColor = list[Random.Range(0, list.Count)];
				list.Remove(omniShapeColor);
				num = 1;
				break;
			case OmniSpawnDef_Shape.ShapeInstruction.ShootTheShape:
				flag2 = true;
				omniShapeType = list2[Random.Range(0, list2.Count)];
				list2.Remove(omniShapeType);
				num = 1;
				break;
			case OmniSpawnDef_Shape.ShapeInstruction.ShootTheColorShape:
				flag = true;
				flag2 = true;
				omniShapeColor = list[Random.Range(0, list.Count)];
				list.Remove(omniShapeColor);
				omniShapeType = list2[Random.Range(0, list2.Count)];
				list2.Remove(omniShapeType);
				num = 1;
				break;
			case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheColor:
				flag = true;
				omniShapeColor = list[Random.Range(0, list.Count)];
				list.Remove(omniShapeColor);
				num = m_shapeAmount / 2;
				break;
			case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheShape:
				flag2 = true;
				omniShapeType = list2[Random.Range(0, list2.Count)];
				list2.Remove(omniShapeType);
				num = m_shapeAmount / 2;
				break;
			case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheColorShape:
				flag = true;
				flag2 = true;
				omniShapeColor = list[Random.Range(0, list.Count)];
				list.Remove(omniShapeColor);
				omniShapeType = list2[Random.Range(0, list2.Count)];
				list2.Remove(omniShapeType);
				num = m_shapeAmount / 2;
				break;
			case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheNotColor:
				flag = true;
				flag3 = true;
				omniShapeColor = list[Random.Range(0, list.Count)];
				list.Remove(omniShapeColor);
				num = m_shapeAmount / 2;
				break;
			case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheNotShape:
				flag2 = true;
				flag3 = true;
				omniShapeType = list2[Random.Range(0, list2.Count)];
				list2.Remove(omniShapeType);
				num = m_shapeAmount / 2;
				break;
			case OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheNotColorShape:
				flag = true;
				flag2 = true;
				flag3 = true;
				omniShapeColor = list[Random.Range(0, list.Count)];
				list.Remove(omniShapeColor);
				omniShapeType = list2[Random.Range(0, list2.Count)];
				list2.Remove(omniShapeType);
				num = m_shapeAmount / 2;
				break;
			}
			for (int j = 0; j < m_shapeAmount; j++)
			{
				bool flag4 = false;
				OmniSpawnDef_Shape.OmniShapeType omniShapeType2 = list2[Random.Range(0, list2.Count)];
				OmniSpawnDef_Shape.OmniShapeColor color = list[Random.Range(0, list.Count)];
				if (flag2 && j < num)
				{
					omniShapeType2 = omniShapeType;
					flag4 = true;
				}
				if (flag && j < num)
				{
					color = omniShapeColor;
					flag4 = true;
				}
				GameObject gameObject = Object.Instantiate(ShapePrefabs[(int)omniShapeType2], base.transform.position, Quaternion.identity);
				OmniShape component = gameObject.GetComponent<OmniShape>();
				if (flag3)
				{
					component.Init(this, !flag4, color);
				}
				else
				{
					component.Init(this, flag4, color);
				}
				m_activeShapes.Add(component);
			}
			if (flag3)
			{
				num = m_shapeAmount - m_shapeAmount / 2;
			}
			int num2 = Random.Range(1, 4);
			for (int k = 0; k < num2; k++)
			{
				for (int num3 = m_activeShapes.Count - 1; num3 > 0; num3--)
				{
					int index = Random.Range(0, num3);
					OmniShape value = m_activeShapes[num3];
					m_activeShapes[num3] = m_activeShapes[index];
					m_activeShapes[index] = value;
				}
			}
			for (int l = 0; l < m_activeShapes.Count; l++)
			{
				Vector3 zero = Vector3.zero;
				zero.z = GetRange();
				zero.y = 1.25f;
				float num4 = (float)(m_shapeAmount - 1) * 2.2f;
				zero.x = (float)l * 2.2f - num4 * 0.5f;
				m_activeShapes[l].transform.position = zero;
				m_activeShapes[l].transform.rotation = Quaternion.LookRotation(-Vector3.forward, Vector3.up);
			}
			string text = "Shoot ";
			bool flag5 = false;
			if (m_instructions[m_curInstruction] == OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheColor || m_instructions[m_curInstruction] == OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheColorShape || m_instructions[m_curInstruction] == OmniSpawnDef_Shape.ShapeInstruction.ShootAllTheShape)
			{
				text += "all of the ";
				flag5 = true;
			}
			else
			{
				text += "the ";
				flag5 = false;
			}
			if (flag3)
			{
				if (flag && flag2)
				{
					string text2 = HexColors[(int)omniShapeColor];
					string text3 = HexColors[(int)list[Random.Range(0, list.Count)]];
					string text4 = HexColors[(int)list[Random.Range(0, list.Count)]];
					switch (Random.Range(0, 3))
					{
					case 0:
					{
						text += "Shapes that are not ";
						string text5 = text;
						text = text5 + "<color=#" + text3 + ">" + omniShapeColor.ToString() + "</color> ";
						text5 = text;
						text = text5 + "<color=#" + text2 + ">" + omniShapeType.ToString() + "s </color> ";
						break;
					}
					case 1:
					{
						text += "Shapes that are not ";
						string text5 = text;
						text = text5 + "<color=#" + text2 + ">" + omniShapeColor.ToString() + "</color> ";
						text5 = text;
						text = text5 + "<color=#" + text3 + ">" + omniShapeType.ToString() + "s </color> ";
						break;
					}
					case 2:
					{
						text += "Shapes that are not ";
						string text5 = text;
						text = text5 + "<color=#" + text3 + ">" + omniShapeColor.ToString() + "</color> ";
						text5 = text;
						text = text5 + "<color=#" + text4 + ">" + omniShapeType.ToString() + "s </color> ";
						break;
					}
					}
				}
				else if (flag)
				{
					int num5 = Random.Range(0, 3);
					string text6 = HexColors[(int)omniShapeColor];
					string text7 = HexColors[(int)list[Random.Range(0, list.Count)]];
					string text8 = HexColors[(int)list[Random.Range(0, list.Count)]];
					switch (num5)
					{
					case 0:
					{
						text = text + "<color=#" + text6 + ">Shapes </color> ";
						text += "that are not ";
						string text5 = text;
						text = text5 + "<color=#" + text7 + ">" + omniShapeColor.ToString() + "</color>";
						break;
					}
					case 1:
					{
						text = text + "<color=#" + text7 + ">Shapes </color> ";
						text += "that are not ";
						string text5 = text;
						text = text5 + "<color=#" + text6 + ">" + omniShapeColor.ToString() + "</color>";
						break;
					}
					case 2:
					{
						text = text + "<color=#" + text8 + ">Shapes </color> ";
						text += "that are not ";
						string text5 = text;
						text = text5 + "<color=#" + text7 + ">" + omniShapeColor.ToString() + "</color>";
						break;
					}
					}
				}
				else if (flag2)
				{
					string text9 = HexColors[(int)list[Random.Range(0, list.Count)]];
					string text10 = HexColors[(int)list[Random.Range(0, list.Count)]];
					switch (Random.Range(0, 2))
					{
					case 0:
					{
						text = text + "<color=#" + text9 + ">Shapes </color> ";
						text += "that are not ";
						string text5 = text;
						text = text5 + "<color=#" + text9 + ">" + omniShapeType.ToString() + "s</color> ";
						break;
					}
					case 1:
					{
						text = text + "<color=#" + text9 + ">Shapes </color> ";
						text += "that are not ";
						string text5 = text;
						text = text5 + "<color=#" + text10 + ">" + omniShapeType.ToString() + "s</color> ";
						break;
					}
					}
				}
			}
			else if (flag && flag2)
			{
				string text11 = HexColors[(int)omniShapeColor];
				string text12 = HexColors[(int)list[Random.Range(0, list.Count)]];
				string text13 = HexColors[(int)list[Random.Range(0, list.Count)]];
				string text14 = HexColors[(int)list[Random.Range(0, list.Count)]];
				switch (Random.Range(0, 6))
				{
				case 0:
				{
					string text5 = text;
					text = text5 + "<color=#" + text11 + ">" + omniShapeColor.ToString() + "</color> ";
					if (flag5)
					{
						text5 = text;
						text = text5 + "<color=#" + text12 + ">" + omniShapeType.ToString() + "s</color> ";
					}
					else
					{
						text5 = text;
						text = text5 + "<color=#" + text12 + ">" + omniShapeType.ToString() + "</color> ";
					}
					break;
				}
				case 1:
				{
					string text5 = text;
					text = text5 + "<color=#" + text12 + ">" + omniShapeColor.ToString() + "</color> ";
					if (flag5)
					{
						text5 = text;
						text = text5 + "<color=#" + text11 + ">" + omniShapeType.ToString() + "s</color> ";
					}
					else
					{
						text5 = text;
						text = text5 + "<color=#" + text11 + ">" + omniShapeType.ToString() + "</color> ";
					}
					break;
				}
				case 2:
				{
					string text5 = text;
					text = text5 + "<color=#" + text12 + ">" + omniShapeColor.ToString() + "</color> ";
					if (flag5)
					{
						text5 = text;
						text = text5 + "<color=#" + text13 + ">" + omniShapeType.ToString() + "s</color> ";
					}
					else
					{
						text5 = text;
						text = text5 + "<color=#" + text13 + ">" + omniShapeType.ToString() + "</color> ";
					}
					break;
				}
				case 3:
				{
					string text5;
					if (flag5)
					{
						text5 = text;
						text = text5 + "<color=#" + text11 + ">" + omniShapeType.ToString() + "s </color> ";
						text += "that are ";
					}
					else
					{
						text5 = text;
						text = text5 + "<color=#" + text11 + ">" + omniShapeType.ToString() + " </color> ";
						text += "that is ";
					}
					text5 = text;
					text = text5 + "<color=#" + text12 + ">" + omniShapeColor.ToString() + "</color> ";
					break;
				}
				case 4:
				{
					string text5;
					if (flag5)
					{
						text5 = text;
						text = text5 + "<color=#" + text12 + ">" + omniShapeType.ToString() + "s </color> ";
						text += "that are ";
					}
					else
					{
						text5 = text;
						text = text5 + "<color=#" + text12 + ">" + omniShapeType.ToString() + " </color> ";
						text += "that is ";
					}
					text5 = text;
					text = text5 + "<color=#" + text11 + ">" + omniShapeColor.ToString() + "</color> ";
					break;
				}
				case 5:
				{
					string text5;
					if (flag5)
					{
						text5 = text;
						text = text5 + "<color=#" + text12 + ">" + omniShapeType.ToString() + "s </color> ";
						text += "that are ";
					}
					else
					{
						text5 = text;
						text = text5 + "<color=#" + text12 + ">" + omniShapeType.ToString() + " </color> ";
						text += "that is ";
					}
					text5 = text;
					text = text5 + "<color=#" + text13 + ">" + omniShapeColor.ToString() + "</color> ";
					break;
				}
				}
			}
			else if (flag)
			{
				string text15 = HexColors[(int)omniShapeColor];
				string text16 = HexColors[(int)list[Random.Range(0, list.Count)]];
				string text17 = HexColors[(int)list[Random.Range(0, list.Count)]];
				switch (Random.Range(0, 7))
				{
				case 0:
				{
					string text5 = text;
					text = text5 + "<color=#" + text15 + ">" + omniShapeColor.ToString() + "</color> ";
					text = ((!flag5) ? (text + "Shape") : (text + "Shapes"));
					break;
				}
				case 1:
				{
					string text5 = text;
					text = text5 + "<color=#" + text15 + ">" + omniShapeColor.ToString() + "</color> ";
					text = ((!flag5) ? (text + "<color=#" + text16 + ">Shape</color> ") : (text + "<color=#" + text16 + ">Shapes</color> "));
					break;
				}
				case 2:
				{
					string text5 = text;
					text = text5 + "<color=#" + text16 + ">" + omniShapeColor.ToString() + "</color> ";
					text = ((!flag5) ? (text + "<color=#" + text15 + ">Shape</color> ") : (text + "<color=#" + text15 + ">Shapes</color> "));
					break;
				}
				case 3:
				{
					string text5 = text;
					text = text5 + "<color=#" + text16 + ">" + omniShapeColor.ToString() + "</color> ";
					text = ((!flag5) ? (text + "<color=#" + text17 + ">Shape</color> ") : (text + "<color=#" + text17 + ">Shapes</color> "));
					break;
				}
				case 4:
				{
					if (flag5)
					{
						text = text + "<color=#" + text16 + ">Shapes </color> ";
						text += "that are ";
					}
					else
					{
						text = text + "<color=#" + text16 + ">Shape </color> ";
						text += "that is ";
					}
					string text5 = text;
					text = text5 + "<color=#" + text15 + ">" + omniShapeColor.ToString() + "</color>";
					break;
				}
				case 5:
				{
					if (flag5)
					{
						text = text + "<color=#" + text15 + ">Shapes </color> ";
						text += "that are ";
					}
					else
					{
						text = text + "<color=#" + text15 + ">Shape </color> ";
						text += "that is ";
					}
					string text5 = text;
					text = text5 + "<color=#" + text16 + ">" + omniShapeColor.ToString() + "</color>";
					break;
				}
				case 6:
				{
					if (flag5)
					{
						text = text + "<color=#" + text17 + ">Shapes </color> ";
						text += "that are ";
					}
					else
					{
						text = text + "<color=#" + text17 + ">Shape </color> ";
						text += "that is ";
					}
					string text5 = text;
					text = text5 + "<color=#" + text16 + ">" + omniShapeColor.ToString() + "</color>";
					break;
				}
				}
			}
			else if (flag2)
			{
				string text18 = HexColors[(int)list[Random.Range(0, list.Count)]];
				string text19 = HexColors[(int)list[Random.Range(0, list.Count)]];
				switch (Random.Range(0, 2))
				{
				case 0:
					if (flag5)
					{
						string text5 = text;
						text = text5 + "<color=#" + text18 + ">" + omniShapeType.ToString() + "s</color> ";
					}
					else
					{
						string text5 = text;
						text = text5 + "<color=#" + text18 + ">" + omniShapeType.ToString() + "</color> ";
					}
					break;
				case 1:
					if (flag5)
					{
						text = text + "<color=#" + text18 + ">Shapes </color> ";
						text += "that are ";
						string text5 = text;
						text = text5 + "<color=#" + text19 + ">" + omniShapeType.ToString() + "s</color> ";
					}
					else
					{
						text = text + "<color=#" + text18 + ">Shape </color> ";
						text += "that is a ";
						string text5 = text;
						text = text5 + "<color=#" + text19 + ">" + omniShapeType.ToString() + "</color> ";
					}
					break;
				}
			}
			m_panel.instructiontext.text = text;
			m_correctShapesLeft = num;
			m_incorrectShapesLeft = m_shapeAmount - m_correctShapesLeft;
			m_curInstruction++;
		}
	}
}
