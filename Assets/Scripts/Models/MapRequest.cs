using System;

namespace Models
{
	[Serializable]
	public class MapRequest
	{
		public int id;

		public string token;

		public override string ToString(){
			return UnityEngine.JsonUtility.ToJson (this, true);
		}
	}
}

