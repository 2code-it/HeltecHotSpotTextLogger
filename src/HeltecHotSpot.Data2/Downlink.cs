using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeltecHotSpotApi.Data
{
	public class Downlink: Link
	{
		public bool imme { get; set; }
		public bool ipol { get; set; }
		public byte powe { get; set; }
	}
}
