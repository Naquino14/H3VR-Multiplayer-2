using System.Collections.Generic;

namespace Assets.Rust.Lodestone
{
	public interface IRequester
	{
		void HandleResponse(string endpointName, float startTime, List<KeyValuePair<string, string>> fieldTypes, List<Dictionary<string, object>> fieldValues);
	}
}
