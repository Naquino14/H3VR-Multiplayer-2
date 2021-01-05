using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubeSpawnWall : MonoBehaviour
{
	public enum SpawnWallType
	{
		Center,
		Up,
		Down,
		Boss
	}

	public Transform[] Spawns_Center;

	public Transform[] Spawns_Up;

	public Transform[] Spawns_Down;

	public Transform[] Spawns_Boss;

	public Text WaveText;

	public Text TimeText;

	public Text ScoreText;

	public GameObject TargetWarning;

	public List<Transform> GetSpawns(int num, SpawnWallType type)
	{
		Transform[] array = new Transform[0];
		switch (type)
		{
		case SpawnWallType.Center:
			array = Spawns_Center;
			break;
		case SpawnWallType.Up:
			array = Spawns_Up;
			break;
		case SpawnWallType.Down:
			array = Spawns_Down;
			break;
		case SpawnWallType.Boss:
			array = Spawns_Boss;
			break;
		}
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < Mathf.Min(num, array.Length); i++)
		{
			Transform item = array[Random.Range(0, array.Length)];
			if (!list.Contains(item))
			{
				list.Add(item);
			}
			else
			{
				i--;
			}
		}
		return list;
	}
}
