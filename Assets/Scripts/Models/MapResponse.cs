using System;

namespace Models
{
	[Serializable]
	public class MapResponse
	{
		public string error;

		public string sha256_al;

		public string b64;

		public override string ToString(){
			return UnityEngine.JsonUtility.ToJson (this, true);
		}
	}
}

