using UnityEngine;

public interface IPTargetUnsafeFunctions
{
	void ApplyLabelMesh(int label, Mesh mesh);

	int GetCurrentLabelFromLastLabel(int lastLabel);

	Rect GetLabelRect(int label);

	bool IsAttached(int label);
}
