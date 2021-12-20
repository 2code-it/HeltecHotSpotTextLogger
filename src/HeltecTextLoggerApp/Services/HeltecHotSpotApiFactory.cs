using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HeltecTextLoggerApp.Services
{
	public class HeltecHotSpotApiFactory : IHeltecHotSpotApiFactory
	{
		public HeltecHotSpot.Data.IHeltecHotSpotApi GetHeltecHotSpotApi(string ipAddres, string user, string password)
		{
			return new Services.HeltecHotSpotProxyApi(ipAddres, user, password);
		}

	}
}
