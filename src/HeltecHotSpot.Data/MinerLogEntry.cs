using System;

namespace HeltecHotSpot.Data
{
	public class MinerLogEntry
	{
		public DateTime created { get; set; }
		public byte category { get; set; }
		public string severity { get; set; }
		public string origin { get; set; }
		public string source { get; set; }
		public int code { get; set; }
		public string message { get; set; }
	}
}
