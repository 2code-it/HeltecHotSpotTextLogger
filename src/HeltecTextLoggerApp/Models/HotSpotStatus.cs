using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeltecTextLoggerApp.Models
{
	public class HotSpotStatus: HeltecHotSpot.Data.HotSpotStatus
	{
		public DateTime created { get; set; }
	}
}
