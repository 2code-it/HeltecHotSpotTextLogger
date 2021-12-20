using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace HeltecTextLoggerApp.Models
{
	public class LogInfo : Mvvm.ObservableObject, ILogInfo
	{
		private string _heliumHotSpotAddress;
		private string _localIPAddress;
		private string _loginName;
		private string _loginPassword;
		private int _loraLogUplinkAmount;
		private int _loraLogDownlinkAmount;
		private int _loraLogIntervalInMinutes;
		private int _statusLogIntervalInMinutes;
		private int _minerLogIntervalInMinutes;


		public string HeliumHotSpotAddress { get => _heliumHotSpotAddress; set => Set(ref _heliumHotSpotAddress, value); }
		public string LocalIPAddress { get => _localIPAddress; set => Set(ref _localIPAddress, value); }
		public string LoginName { get => _loginName; set => Set(ref _loginName, value); }
		public string LoginPassword { get => _loginPassword; set => Set(ref _loginPassword, value); }
		public int LoraLogDownlinkAmount { get => _loraLogDownlinkAmount; set => Set(ref _loraLogDownlinkAmount, value); }
		public int LoraLogUplinkAmount { get => _loraLogUplinkAmount; set => Set(ref _loraLogUplinkAmount, value); }
		public int LoraLogIntervalInMinutes { get => _loraLogIntervalInMinutes; set => Set(ref _loraLogIntervalInMinutes, value); }
		public int StatusLogIntervalInMinutes { get => _statusLogIntervalInMinutes; set => Set(ref _statusLogIntervalInMinutes, value); }
		public int MinerLogIntervalInMinutes { get => _minerLogIntervalInMinutes; set => Set(ref _minerLogIntervalInMinutes, value); }


	}
}
