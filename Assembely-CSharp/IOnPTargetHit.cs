using System.Collections.Generic;

public interface IOnPTargetHit
{
	void OnTargetHit(List<OnHitInfo> bulletHits);
}
