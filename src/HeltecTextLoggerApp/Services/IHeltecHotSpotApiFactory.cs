using HeltecHotSpot.Data;
using System.Security;

namespace HeltecTextLoggerApp.Services
{
	public interface IHeltecHotSpotApiFactory
	{
		IHeltecHotSpotApi GetHeltecHotSpotApi(string ipAddres, string user, string password);
	}
}