using UnityEngine;

public interface IMGSpawnIntoAble
{
	bool CanBeSpawnedInto();

	void PlaceObjectInto(GameObject obj);
}
