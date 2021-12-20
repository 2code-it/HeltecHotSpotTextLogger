using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace HeltecTextLoggerApp
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
			Services.IAppIniFileService appIniFileService = new Services.AppIniFileService();
			Models.ILogInfo logInfo = appIniFileService.GetObjectFromSection<Models.LogInfo>("logger");
			Services.ObjectMappingService objectMappingService = new Services.ObjectMappingService();
			Services.IMessengerService messengerService = new Services.MessengerService(); 
			Services.IFileSystemService fileSystemService = new Services.FileSystemService();
			Services.IHeltecHotSpotApiFactory heltecHotSpotApiFactory = new Services.HeltecHotSpotApiFactory();
			Services.ILogTaskSchedulingService logTaskSchedulingService = new Services.LogTaskSchedulingService(fileSystemService, heltecHotSpotApiFactory, objectMappingService, messengerService);

			ViewModels.MainWindowViewModel mainWindowViewModel = new ViewModels.MainWindowViewModel(logInfo, logTaskSchedulingService, messengerService);

			Views.MainWindow mainWindow = new Views.MainWindow();
			mainWindow.DataContext = mainWindowViewModel;
			mainWindow.Show();

			base.OnStartup(e);
		}
	}
}
