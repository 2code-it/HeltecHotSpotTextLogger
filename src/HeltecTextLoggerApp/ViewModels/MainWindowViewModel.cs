using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HeltecTextLoggerApp.ViewModels
{
	public class MainWindowViewModel: Mvvm.ObservableObject
	{
		public MainWindowViewModel(Models.ILogInfo logInfo, Services.ILogTaskSchedulingService logTaskSchedulingService, Services.IMessengerService messengerService) 
		{
			_logTaskSchedulingService = logTaskSchedulingService;
			_messengerService = messengerService;
			LogInfo = logInfo;
			_messengerService.Subscribe<Models.SystemMessage>(this, x => OnReceiveSystemMessage(x));
		}

		private readonly Services.ILogTaskSchedulingService _logTaskSchedulingService;
		private readonly Services.IMessengerService _messengerService;

		private bool _isRunning;
		

		private ICommand _commandStartLogger;
		private ICommand _commandStopLogger;
		private ICommand _commandExitApplication;
		

		public ICommand CommandStartLogger { get => _commandStartLogger ?? (_commandStartLogger = new Mvvm.RelayCommand(OnStartLogger)); }
		public ICommand CommandStopLogger { get => _commandStopLogger ?? (_commandStopLogger = new Mvvm.RelayCommand(OnStopLogger)); }
		public ICommand CommandExitApplication { get => _commandExitApplication ?? (_commandExitApplication = new Mvvm.RelayCommand(OnExitApplication)); }
		
		public Models.ILogInfo LogInfo { get; private set; }
		public bool IsRunning
		{
			get => _isRunning;
			set => Set(ref _isRunning, value);
		}

		public List<Models.SystemMessage> SystemMessages { get; private set; } = new List<Models.SystemMessage>();

		private void OnStartLogger(object parameter)
		{
			_logTaskSchedulingService.Configure(LogInfo);
			_logTaskSchedulingService.Start();
			IsRunning = true;
		}

		private void OnStopLogger(object parameter)
		{
			_logTaskSchedulingService.Stop();
			IsRunning = false;
		}

		private void OnExitApplication(object parameter)
		{
			if(IsRunning)
				_logTaskSchedulingService.Stop();

			_messengerService.Unsubscribe<Models.SystemMessage>(this);
			App.Current.Shutdown();
		}

		private void OnReceiveSystemMessage(Models.SystemMessage message)
		{
			SystemMessages.Insert(0, message);
			while (SystemMessages.Count > LogInfo.MaxSystemMessages)
			{
				SystemMessages.RemoveAt(SystemMessages.Count - 1);
			}
			OnPropertyChanged(nameof(SystemMessages));
		}
	}
}
