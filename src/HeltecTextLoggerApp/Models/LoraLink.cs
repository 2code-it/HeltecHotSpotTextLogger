using HeltecHotSpot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeltecTextLoggerApp.Models
{
	public class LoraLink: Link
	{
		//uplink
		public byte chan { get; set; }
		public byte stat { get; set; }
		public double lsnr { get; set; }
		public short rssi { get; set; }

		//downlink
		public bool imme { get; set; }
		public bool ipol { get; set; }
		public byte powe { get; set; }
	}
}
