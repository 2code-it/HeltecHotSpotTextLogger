using System.Security;

namespace HeltecTextLoggerApp.Models
{
	public interface ILogInfo
	{
		string HeliumHotSpotAddress { get; set; }
		string LocalIPAddress { get; set; }
		string LoginName { get; set; }
		string LoginPassword { get; set; }
		int LoraLogDownlinkAmount { get; set; }
		int LoraLogIntervalInMinutes { get; set; }
		int LoraLogUplinkAmount { get; set; }
		int MinerLogIntervalInMinutes { get; set; }
		int StatusLogIntervalInMinutes { get; set; }
	}
}