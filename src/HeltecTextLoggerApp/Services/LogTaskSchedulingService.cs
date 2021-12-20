using HeltecHotSpot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeltecTextLoggerApp.Services
{
	public class LogTaskSchedulingService : ILogTaskSchedulingService
	{
		public LogTaskSchedulingService(
				IFileSystemService fileSystemService,
				IHeltecHotSpotApiFactory heltecHotSpotApiFactory,
				ObjectMappingService objectMappingService,
				IMessengerService messengerService
		)
		{
			_fileSystemService = fileSystemService;
			_heltecHotSpotApiFactory = heltecHotSpotApiFactory;
			_objectMappingService = objectMappingService;
			_messengerService = messengerService;
			OnLoad();
		}

		private Timer _timer;
		private Models.ILogInfo _logInfo;
		private readonly IFileSystemService _fileSystemService;
		private readonly IHeltecHotSpotApiFactory _heltecHotSpotApiFactory;
		private readonly ObjectMappingService _objectMappingService;
		private readonly IMessengerService _messengerService;
		private IHeltecHotSpotApi _heltecHotSpotApi;

		private static readonly string _fileNameLoraLog = "_lora.txt";
		private static readonly string _fileNameMinerLog = "_miner.txt";
		private static readonly string _fileNameStatusLog = "_status.txt";

		private string _pathLoraLog;
		private string _pathMinerLog;
		private string _pathStatusLog;

		private DateTime _lastLoraLogUpdate = DateTime.MinValue;
		private DateTime _lastMinerLogUpdate = DateTime.MinValue;


		public void Configure(Models.ILogInfo logInfo)
		{
			_logInfo = logInfo;
			_heltecHotSpotApi = _heltecHotSpotApiFactory.GetHeltecHotSpotApi(_logInfo.LocalIPAddress, _logInfo.LoginName, _logInfo.LoginPassword);
		}

		public void Start()
		{
			if (_timer != null) Stop();

			

			int dueTimeInSeconds = 60 - ((int)Math.Round(DateTime.Now.TimeOfDay.TotalSeconds) % 60);
			_timer = new Timer(new TimerCallback(OnTimer), null, dueTimeInSeconds * 1000, 60000);
		}

		public void Stop()
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}
		private void OnLoad()
		{
			_pathLoraLog = _fileSystemService.PathCombine(_fileSystemService.GetBaseDirectory(), _fileNameLoraLog);
			_pathMinerLog = _fileSystemService.PathCombine(_fileSystemService.GetBaseDirectory(), _fileNameMinerLog);
			_pathStatusLog = _fileSystemService.PathCombine(_fileSystemService.GetBaseDirectory(), _fileNameStatusLog);

			if (_fileSystemService.FileExists(_pathLoraLog)) _lastLoraLogUpdate = _fileSystemService.FileGetLastWriteTime(_pathLoraLog);
			if (_fileSystemService.FileExists(_pathMinerLog)) _lastMinerLogUpdate = _fileSystemService.FileGetLastWriteTime(_pathMinerLog);
		}

		private  void OnTimer(object state)
		{
			int minuteOfDay = (int)Math.Round(DateTime.Now.TimeOfDay.TotalSeconds / 60);
			bool runHotSpotStatus = _logInfo.StatusLogIntervalInMinutes > 0 && minuteOfDay % _logInfo.StatusLogIntervalInMinutes == 0;
			bool runLoraLog = _logInfo.LoraLogIntervalInMinutes > 0 && minuteOfDay % _logInfo.LoraLogIntervalInMinutes == 0;
			bool runMinerLog = _logInfo.MinerLogIntervalInMinutes > 0 && minuteOfDay % _logInfo.MinerLogIntervalInMinutes == 0;

			if (runHotSpotStatus) Task.Run(() => SaveHotSpotStatus());
			if (runLoraLog) Task.Run(() => SaveLoraLog());
			if (runMinerLog) Task.Run(() => SaveMinerLog());
		}

		private void SaveHotSpotStatus()
		{
			HotSpotStatus status = null;
			try
			{
				status = _heltecHotSpotApi.GetStatus();
			}
			catch(Exception x)
			{
				_messengerService.Publish(new Models.SystemMessage("error", "Failed to get hotspot status: " + x.Message));
				return;
			}
			var hotSpotStatus = _objectMappingService.Map<Models.HotSpotStatus, HotSpotStatus>(status);
			hotSpotStatus.created = DateTime.Now;
			SerializeAndSaveObjects(new[] { hotSpotStatus }, _pathStatusLog);
			_messengerService.Publish(new Models.SystemMessage("info", "Hotspot status log updated, new items: 1"));
		}

		private void SaveLoraLog()
		{
			List<Models.LoraLink> result = new List<Models.LoraLink>();

			if (_logInfo.LoraLogDownlinkAmount > 0)
			{
				try
				{
					var downLinks = _heltecHotSpotApi.GetLatestDownlinks(_logInfo.LoraLogDownlinkAmount);
					result.AddRange(downLinks.Select(x => _objectMappingService.Map<Models.LoraLink, Downlink>(x)));
				}
				catch (Exception x)
				{
					_messengerService.Publish(new Models.SystemMessage("error", "Failed to get lora downlinks: " + x.Message));
					return;
				}
			}

			if (_logInfo.LoraLogUplinkAmount > 0)
			{
				try
				{
					var uplinks = _heltecHotSpotApi.GetLatestUplinks(_logInfo.LoraLogUplinkAmount);
					result.AddRange(uplinks.Select(x => _objectMappingService.Map<Models.LoraLink, Uplink>(x)));
				}
				catch (Exception x)
				{
					_messengerService.Publish(new Models.SystemMessage("error", "Failed to get lora uplinks: " + x.Message));
					return;
				}
			}
			result = result.Where(x => x.created > _lastLoraLogUpdate).ToList();
			SerializeAndSaveObjects(result, _pathLoraLog);
			_lastLoraLogUpdate = DateTime.Now;
			_messengerService.Publish(new Models.SystemMessage("info", "Lora log updated, new items: " + result.Count.ToString()));
		}

		private void SaveMinerLog()
		{
			MinerLogEntry[] logEntries = null;
			try
			{
				logEntries = _heltecHotSpotApi.GetMinerLogEntries();
			}
			catch (Exception x)
			{
				_messengerService.Publish(new Models.SystemMessage("error", "Failed to get miner log: " + x.Message));
				return;
			}

			logEntries = logEntries.Where(x => x.created > _lastMinerLogUpdate).OrderBy(x=>x.created).ToArray();
			SerializeAndSaveObjects(logEntries, _pathMinerLog);
			_lastMinerLogUpdate = DateTime.Now;
			_messengerService.Publish(new Models.SystemMessage("info", "Miner log updated, new items: " + logEntries.Length.ToString()));
		}

		private void SerializeAndSaveObjects<T>(IEnumerable<T> objects, string pathToFile)
		{
			if (objects.Any())
			{
				var properties = typeof(T).GetProperties().Where(x => x.CanRead).ToArray();
				var lines = objects.Select(x => string.Join('\t', properties.Select(p => GetStringValueFromProperty(p, x))));
				_fileSystemService.FileAppendAllLines(pathToFile, lines);
			}
		}

		private string GetStringValueFromProperty(PropertyInfo propertyInfo, object data)
		{
			if (propertyInfo.PropertyType == typeof(DateTime))
			{
				DateTime value = (DateTime)propertyInfo.GetValue(data);
				return value.ToString("yyyy-MM-dd HH:mm:ss");
			}
			else
			{
				object value = propertyInfo.GetValue(data);
				return Convert.ToString(value);
			}
		}
	}
}
