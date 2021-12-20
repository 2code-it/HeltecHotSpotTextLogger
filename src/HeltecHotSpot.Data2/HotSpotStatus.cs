using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeltecHotSpotApi.Data
{
	public class HotSpotStatus
	{
		public string addr { get; set; }
		public int block { get; set; }
		public double disk_G { get; set; }
		public byte disk_p { get; set; }
		public string eth { get; set; }
		public string firmware { get; set; }
		public int latest_block { get; set; }
		public string latest_firmware { get; set; }
		public string latest_miner { get; set; }
		public string listenaddr { get; set; }
		public double mem { get; set; }
		public byte mem_p { get; set; }
		public string miner { get; set; }
		public string name { get; set; }
		public string nettype { get; set; }
		public string region { get; set; }
		public string sn { get; set; }
		public byte temperature { get; set; }
		public string updating_miner { get; set; }
		public string wlan { get; set; }
	}
}
