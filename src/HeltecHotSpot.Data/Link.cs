using System;

namespace HeltecHotSpot.Data
{
	public class Link
	{
		public DateTime created { get; set; }

		public string modu { get; set; }
		public string datr { get; set; }
		public string codr { get; set; }
		public double freq { get; set; }
		public byte rfch { get; set; }
		public long tmst { get; set; }
		public short size { get; set; }
		public string data { get; set; }
	}
}
