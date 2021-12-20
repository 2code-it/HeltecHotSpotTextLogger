using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeltecHotSpotApi.Data
{
	public class Uplink: Link
	{
		public byte chan { get; set; }
		public byte stat { get; set; }
		public double lsnr { get; set; }
		public short rssi { get; set; }
	}
}
